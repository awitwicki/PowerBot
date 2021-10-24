using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PowerBot.Web.Helpers;

namespace PowerBot.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {

        }

        public void OnPost(string password)
        {
            //check password
            var secret = _configuration["password"];

            if (secret == password)
            {
                // Hash password
                string passwordHash = CryptoHelper.GetHash(password);

                //add cookie, and redirect
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30)
                };

                Response.Cookies.Append("accessToken", passwordHash, cookieOptions);
            }

            Response.Redirect("/");
        }
    }
}
