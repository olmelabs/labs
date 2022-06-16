using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CognitoUserManager.Contracts;
using CognitoUserManager.Contracts.DTO;
using CognitoUserManager.Contracts.DTO.Refresh_Token;
using CognitoUserManager.Contracts.Repositories;
using CognitoUserManager.Contracts.Services;
using Microsoft.AspNetCore.Authorization;

namespace CognitoUserManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userService;
        private readonly IPersistService _cache;

        public HomeController(IUserRepository userService, IPersistService cache, ILogger<HomeController> logger)
        {
            _userService = userService;
            _logger = logger;
            _cache = cache;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> RefreshTokens()
        {
            var userId = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First();
            var email = User.Claims.Where(x => x.Type == ClaimTypes.Email).First();

            var tokens = _cache.Get<TokenModel>($"{userId.Value}_{UserController.Session_TokenKey}");

            var model = new RefreshTokenModel
            {
                UserId = userId.Value,
                EmailAddress = email.Value,
                RefreshToken = tokens.RefreshToken,
            };

            var response = await _userService.RefreshTokens(model);

            if (response.IsSuccess)
            {
                _cache.Set<TokenModel>($"{response.UserId}_{UserController.Session_TokenKey}", response.Tokens);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
