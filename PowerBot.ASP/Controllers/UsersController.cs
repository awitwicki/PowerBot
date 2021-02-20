using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerBot.ASP.Models;
using PowerBot.Core;
using PowerBot.Core.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PowerBot.ASP.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        public IActionResult Users()
        {
            var vm = new UsersViewModel();
            vm.Users = UserManager.GetUsers();

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
