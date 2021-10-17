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
    public class UserService
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _httpContext;

        private const string USER_ROLE = "USER";

        public UserService(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = db;
            _mapper = mapper;
        }

        public IEnumerable<CompetitionDto> GetAllAvailableCompetitions()
        {
            var competitions = _context.Competitions
                .Include(c => c.CreatedBy)
                .Where(x => !x.Finished);
            return _mapper.Map<IQueryable<Competition>, IEnumerable<CompetitionDto>>(competitions);
        }

        public CarDto GetCar(IdentityUser user)
        {
            var car = _context.Cars
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Owner.Email == user.Email).Result;
            return _mapper.Map<CarDto>(car);
        }

        public CarDto GetCarById(int? id) => _mapper.Map<CarDto>(
            _context.Cars.Include(x => x.Owner).FirstOrDefault(c => c.Id == id)
        );

        public void SetCar(CarDto carDto)
        {
            var car = _mapper.Map<Car>(carDto);
            var carToRemove = _context.Cars.FirstOrDefault(x => x.Owner.Email == car.Owner.Email);
            if (carToRemove is not null)
                _context.Cars.Remove(carToRemove);
            _context.Cars.Add(car);
            _context.SaveChanges();
        }
    }
}