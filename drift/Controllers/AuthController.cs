using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using drift.Models;
using drift.Models.Request;
using drift.Service;
using drift.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace drift.Controllers
{
    public class AuthController : Controller
    {
        private AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            try
            {
                var authModel = await _authService.Login(model);

                await Authenticate(authModel.Email, authModel.Role, authModel.UserId);

                // удалить после добавления
                // var med = new RegisterRequest()
                // {
                //  Email = "med@med.ru", Password = "123", RoleEnum = UserRoleEnum.MEDICAL_COMMISSION
                // };
                // var tech = new RegisterRequest()
                // {
                //  Email = "tech@tech.ru", Password = "123", RoleEnum = UserRoleEnum.TECH_COMMISSION
                // };
                // await _authService.Register(med);
                // await _authService.Register(tech);

                if (authModel.Role == UserRoleEnum.USER.ToString())
                    return RedirectToAction("Index", "User");
                if (authModel.Role == UserRoleEnum.ORGANIZER.ToString())
                    return RedirectToAction("GetCreatedCompetitions", "Competition");
                if (authModel.Role == UserRoleEnum.TECH_COMMISSION.ToString())
                    return RedirectToAction("Index", "Tech");
                if (authModel.Role == UserRoleEnum.MEDICAL_COMMISSION.ToString())
                    return RedirectToAction("Index", "Medical");

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return ReturnWithError(nameof(Login), $"Авторизация для {model.Email} не удалась.");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            try
            {
                await _authService.Register(model);
                return Redirect("Login");
            }
            catch
            {
                return ReturnWithError(nameof(Register),
                    $"Регистрация для {model.UserName} не удалась. Саня всё сломал.");
            }
        }

        private IActionResult ReturnWithError(string action, string error)
        {
            return View("CustomError",
                new CustomError()
                {
                    Action = nameof(Login), Controller = this.GetCurrentControllerName(nameof(AuthController)),
                    Header = error
                });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        private async Task Authenticate(string email, string role, string userId)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, userId),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}