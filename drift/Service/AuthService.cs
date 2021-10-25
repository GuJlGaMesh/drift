using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using drift.Models.Request;
using drift.Models.Template;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace drift.Service
{
    public class AuthService
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private HttpContext _httpContext;

        private const string USER_ROLE = "USER";

        public AuthService(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IMapper mapper
            , IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = db;
            _mapper = mapper;
            _httpContext = httpContext.HttpContext;
            _userManager.PasswordValidators.Clear();
        }

        public async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(_httpContext?.User.Identity.Name);
        }

        public async Task<IdentityUser> Register(RegisterRequest request)
        {
            if (_userManager.FindByEmailAsync(request.Email).Result != null)
            {
                throw new Exception("User exists");
            }

            var user = new IdentityUser()
            {
                Email = request.Email, UserName = request.Email, EmailConfirmed = true
            };
            var userResult = await _userManager.CreateAsync(user, request.Password);
            if (!userResult.Succeeded)
            {
                throw new Exception("Error creating user");
            }

            var userRole = Enum.GetName(request.Role) ?? USER_ROLE;

            if (!_roleManager.RoleExistsAsync(userRole).Result)
            {
                await _roleManager.CreateAsync(new IdentityRole(userRole));
            }

            await _userManager.AddToRoleAsync(user, userRole);
            return user;
        }

        public async Task<UserCredentialsTemplate> Login(LoginRequest loginRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            var result = await _signInManager.PasswordSignInAsync(existingUser.Email, loginRequest.Password, true, false);
            if (!result.Succeeded)
            {
                throw new Exception("Error logging in");
            }
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            Console.WriteLine(role);

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
            return new UserCredentialsTemplate(user.Email, role, user.Id);
        }

        public IEnumerable<CompetitionDto> GetAllAvailableCompetitions()
        {
            var competitions = _context.Competitions
                .Include(c => c.CreatedBy)
                .Where(x => !x.Finished);
            return _mapper.Map<IQueryable<Competition>, IEnumerable<CompetitionDto>>(competitions);
        }
    }
}