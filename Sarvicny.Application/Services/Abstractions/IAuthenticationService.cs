using Sarvicny.Contracts;
using Sarvicny.Contracts.Authentication;
using Sarvicny.Domain.Entities.Users;


namespace Sarvicny.Application.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<Response<string>> Register(User user, string role,string userType, string password);

        Task<Response<Object>> Login(LoginRequestDto loginRequestDto);
        
        Task<Response<string>> ConfirmEmailAsync(string token, string email);
    }

}
