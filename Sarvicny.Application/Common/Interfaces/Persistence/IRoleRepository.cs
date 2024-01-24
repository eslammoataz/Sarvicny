namespace Sarvicny.Application.Common.Interfaces.Persistence
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string role);
    }
}
