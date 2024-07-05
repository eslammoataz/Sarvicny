using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sarvicny.Application.Common.Interfaces.Authentication;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Application.Services.Abstractions;
using Sarvicny.Application.Services.Email;
using Sarvicny.Application.Services.Specifications.ServiceProviderSpecifications;
using Sarvicny.Contracts;
using Sarvicny.Contracts.Authentication;
using Sarvicny.Domain.Entities.Emails;
using Sarvicny.Domain.Entities.Users;
using System.Data;
using System.Security.Claims;


namespace Sarvicny.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public IJwtTokenGenerator JwtTokenGenerator { get; }
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceProviderRepository _serviceProviderRepository;
        private readonly UserManager<User> _userManager;



        public AuthenticationService(IConfiguration config, IEmailService emailService,
                IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator,
                IJwtTokenGenerator jwtTokenGenerator, IRoleRepository roleRepository, IUserRepository userRepository
                , IUnitOfWork unitOfWork, ILogger<AuthenticationService> logger, ICustomerRepository customerRepository, IServiceProviderRepository serviceProviderRepository, UserManager<User> userManager)
        {
            JwtTokenGenerator = jwtTokenGenerator;
            _config = config;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _userManager = userManager;
        }


        public async Task<Response<string>> Register(User user, string role, string userType, string password)
        {
            var userExist = await _userRepository.GetUserByEmailAsync(user.Email!);

            var roleExist = await _roleRepository.RoleExistsAsync(role);

            var response = new Response<string>();
            if (userExist != null)
            {
                response.Status = "Failed";
                response.isError = true;
                response.Errors.Add("This email is already registered");
                return response;

            }

            if (!roleExist)
            {
                response.Status = "Failed";
                response.isError = true;
                response.Errors.Add("This Role does not Exist");
                return response;
            }

            // it stores the password hashed already without using bcrypt
            IdentityResult result = await _userRepository.AddUserAsync(user, password);


            // User Registered 
            if (!result.Succeeded)
            {
                response.Status = "Failed";
                response.isError = true;
                response.Errors = result.Errors.Select(error => error.Description).ToList();
                return response;
            }

            await _userRepository.AddUserToRoleAsync(user, role);
            if (role == "Customer")
            {
                var created = _customerRepository.CreateCart(user.Id);
                if (!created)
                {
                    response.Status = "Failed";
                    response.isError = true;
                    response.Errors.Add("Failed to create Cart");
                    return response;
                }
                _unitOfWork.Commit();
            }

            await _userRepository.AddUserClaims(user, new()
                {
                    new Claim("UserType", userType)
                });


            List<Claim> claims = new()
            {
                  new Claim("userId", user.Id),
                  new Claim("Role", role),
            };

            var roles = await _userRepository.GetRolesAsync(user);

            foreach (var r in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }



            var tokenString = JwtTokenGenerator.GenerateToken(claims, _config);
            SetAuthenticationCookie(tokenString);

            //Add Token to Verify the email....
            var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);


            var confirmationLink = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext!, "ConfirmEmail", "Authentication", new { token, email = user.Email });
            var message = new EmailDto(user.Email!, "Sarvicny: Confirmation email link", "Sarvicny Account Confirmation Link : " + confirmationLink!);

            _emailService.SendEmail(message);

            response.Message = $"User Registered Successfully , Verification Email sent to {user.Email} ";
            response.Payload = tokenString;

            return response;

        }


        public async Task<Response<object>> Login(LoginRequestDto loginRequestDto)
        {

            var user = await _userRepository.GetUserByEmailAsync(loginRequestDto.Email);

            var isValidPassword = user != null && await _userRepository.CheckPasswordAsync(user, loginRequestDto.password);

            var response = new Response<object>();

            if (user is null || !isValidPassword)
            {
                response.Status = "Failed";
                response.isError = true;
                response.Errors.Add("Invalid Credentials");
                return response;
            }


            var roles = await _userRepository.GetRolesAsync(user);

            if (roles.FirstOrDefault() == "ServiceProvider")
            {
                var spec = new ProviderWithAvailabilitesSpecification(user.Id);
                var provider = await _serviceProviderRepository.FindByIdAsync(spec);
                if (provider.IsVerified == false || provider.IsBlocked == true)
                {
                    response.Status = "Failed";
                    response.isError = true;
                    response.Errors.Add("not verified or blocked");
                    response.Message = "User is not verified ";
                    response.Payload = null;
                    return response;
                }

            }

            List<Claim> claims = new()
            {
                new Claim("userId", user.Id),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenString = JwtTokenGenerator.GenerateToken(claims, _config);
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigninKey"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    issuer: _config["JWT:ValidIssuer"],
            //    audience: _config["JWT:ValidAudience"],
            //    claims: claims,
            //    expires: DateTime.Now.AddMinutes(60),
            //    signingCredentials: creds);

            //var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            SetAuthenticationCookie(tokenString);


            var payload = new
            {
                tokenString,
                user.Id,
                role = roles.FirstOrDefault(),

            };

            response.Message = "User logged in Successfully";
            response.Payload = payload;
            return response;
        }

        public async Task<Response<string>> ConfirmEmailAsync(string token, string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            var response = new Response<string>();

            if (user != null)
            {
                var result = await _userRepository.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {

                    response.Status = "Success";
                    response.Message = "Email Verified Successfully";
                    return response;
                }
            }

            response.isError = true;
            response.Errors.Add("This User Does not exist!");
            return response;
        }



        private void SetAuthenticationCookie(string tokenString)
        {
            _httpContextAccessor.HttpContext?.Response.Headers.Add("x-Authorization", $"Bearer {tokenString}");

            // Set the token in a cookie
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("Auth", tokenString, GetCookieOptions());
        }

        private CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Use "true" in production to ensure the cookie is only sent over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(120) // Set the cookie expiration time
            };
        }
    }
}
