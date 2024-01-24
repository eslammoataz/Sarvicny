using Microsoft.AspNetCore.Identity;
using Sarvicny.Application.Common.Interfaces.Persistence;

namespace Sarvicny.Infrastructure.Persistence;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public RoleRepository( RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public async Task<bool> RoleExistsAsync(string role)
    {
        return _roleManager.Roles.Any(r => r.Name == role);
    }
}