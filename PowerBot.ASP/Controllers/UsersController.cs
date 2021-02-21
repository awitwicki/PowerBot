using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerBot.ASP.Models;
using PowerBot.Core;
using PowerBot.Core.Managers;
using PowerBot.Core.Models;
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

        [HttpPost]
        public IActionResult GrantUserAccess(int userId, UserAccess userAccess)
        {
            var usr = UserManager.GetUser(userId);

            if (usr == null)
                return new NotFoundResult();

            usr.UserAccess = userAccess;
            UserManager.UpdateUser(usr);

            return RedirectToAction(nameof(Users));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
