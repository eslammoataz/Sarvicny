using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sarvicny.Application.Common.Interfaces.Persistence;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;
using Sarvicny.Infrastructure.Data;

namespace Sarvicny.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly ICustomerRepository _customerRepository;

    public UserRepository(UserManager<User> userManager, AppDbContext context, ICustomerRepository customerRepository)
    {
        _context = context;
        _userManager = userManager;
        _customerRepository = customerRepository;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user;
    }

    public async Task<IdentityResult> AddUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        return result;
    }

    public async Task<IdentityResult> AddUserToRoleAsync(User user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);
        return result;

    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        return token;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var result = await _userManager.CheckPasswordAsync(user, password);
        return result;
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result;
    }

    public async Task<ICollection<Customer>> GetAllCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers;
    }

    //public async Task<ICollection<Provider>> GetAllServiceProviders()
    //{
    //    var providers = await _context.Provider.ToListAsync();
    //    return providers;
    //}
    //public async Task<ICollection<Provider>> GetProvidersRegistrationRequest()
    //{
    //    var providers = await _context.Provider
    //        .Where(p => p.isVerified == false)
    //        .ToListAsync();

    //    return providers;
    //}
}