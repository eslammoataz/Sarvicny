using Microsoft.AspNetCore.Http;

namespace Sarvicny.Application.Services.Email
{
    public interface IEmailConfirmService
    {
        Task<bool> ConfirmEmailAsync(string token, string email);

        public string GenerateConfirmationLink(string token, string email, HttpContext httpContext = null);


    }
}
