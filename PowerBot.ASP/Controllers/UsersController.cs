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
        private readonly PowerBotHostedService _powerBot;

        public UsersController(ILogger<UsersController> logger, PowerBotHostedService powerBot)
        {
            _logger = logger;
            _powerBot = powerBot;
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

        public IActionResult GetUser(int id)
        {
            var usr = UserManager.GetUser(id);

            if (usr == null)
                return new NotFoundResult();

            var vm = new UserViewModel();
            vm.User = usr;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int userId, string text)
        {
            var usr = UserManager.GetUser(userId);

            if (usr == null)
                return new NotFoundResult();

            var vm = new UserViewModel();
            vm.User = usr;

            //TODO: rework to logic layer with sendMessageResults
            try
            {
                await _powerBot.BotClient.SendTextMessageAsync(usr.TelegramId, text);
            }
            catch (Exception ex)
            {
                vm.SendMessageError = ex.Message;
            }

            return View(nameof(GetUser), vm);
        }
    }
}
