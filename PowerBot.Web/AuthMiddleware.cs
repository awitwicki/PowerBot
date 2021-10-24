using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PowerBot.Web.Helpers;
using System.Threading.Tasks;

namespace WebApi.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            // Get toket hash from request
            var token = context.Request.Cookies["accessToken"];
            var isAuthorized = CheckToken(token);

            // Logout endpoint
            if (context.Request.Path.Value == ("/logout") && _configuration["password"] != null)
            {
                context.Response.Cookies.Delete("accessToken");
                isAuthorized = false;
            }

            if (!isAuthorized && context.Request.Path.Value != ("/login"))
            {
                // Redirect to login
                context.Response.Redirect("/login");
            }
            
            await _next(context);
        }

        private bool CheckToken(string token)
        {
            var secret = _configuration["password"];

            if (secret != null)
                return CryptoHelper.GetHash(secret) == token;
            else
                return true;
        }
    }
}
