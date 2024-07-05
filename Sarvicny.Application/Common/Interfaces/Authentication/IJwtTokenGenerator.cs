using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Sarvicny.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateToken(List<Claim> claims, IConfiguration config);
}