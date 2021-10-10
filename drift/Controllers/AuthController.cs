using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using drift.Models;
using drift.Models.Request;
using drift.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace drift.Controllers
{
    public class AuthController : Controller
    {
        private UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var authModel = await _userService.Login(model);
            
            await Authenticate(authModel.Email, authModel.Role, authModel.UserId);


            if (authModel.Role == UserRole.USER.ToString())
                return RedirectToAction("Index", "User");
            // if (authModel.Role == UserRole.ORGANIZER.ToString())
            // return RedirectToAction("Index", "Competition");
            // if (authModel.Role == UserRole.TECH_COMMISSION.ToString())
            // return RedirectToAction("Index", "Tech");
            // if (authModel.Role == UserRole.MEDICAL_COMMISSION.ToString())
            // return RedirectToAction("Index", "Medical");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            await _userService.Register(model);
            return Redirect("Login");
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