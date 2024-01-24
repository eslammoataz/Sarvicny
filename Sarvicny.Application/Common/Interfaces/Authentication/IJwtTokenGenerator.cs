using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Sarvicny.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateToken(List<Claim> claims, IConfiguration config);
}