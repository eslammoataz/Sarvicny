﻿using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Sarvicny.Domain.Entities.Users;
using Sarvicny.Domain.Entities.Users.ServicProviders;

namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserNameAsync(string userName);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);



        Task<IdentityResult> AddUserToRoleAsync(User user, string role);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IList<string>> GetRolesAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<ICollection<Customer>> GetAllCustomers();

        //Task<ICollection<Provider>> GetAllServiceProviders();         //// Fe service provider repo 3shan a7tgt spec
        //Task<ICollection<Provider>> GetProvidersRegistrationRequest();
        Task AddUserClaims(User user, List<Claim> claims);
        Task<ICollection<Claim>> GetClaims(User user);
    }
}
