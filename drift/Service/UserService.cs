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
        private readonly AuthService _authService;
        private IHttpContextAccessor _httpContext;

        private const string USER_ROLE = "USER";

        public UserService(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IMapper mapper, AuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = db;
            _mapper = mapper;
            _authService = authService;
        }
        public UserDto findById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            return new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName
            };
        }

        public IEnumerable<CompetitionDto> GetAllAvailableCompetitions()
        {
            var competitions = _context.Competitions
                .Include(c => c.CreatedBy)
                .Where(x => !x.Finished);
            return _mapper.Map<IQueryable<Competition>, IEnumerable<CompetitionDto>>(competitions);
        }

        public bool IsAlreadyParticipate()
        {
	        var user = _authService.GetCurrentUserAsync().Result;
	        var count = _context.CompetitionApplications.Count(x => x.ApplicantId == user.Id);
	        return count > 0;
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

        public CompetitionDto GetCompetition(int? id)
        {
            var competition = _context.Competitions.Find(id);
            return _mapper.Map<CompetitionDto>(competition);
        }

        public bool CheckAvailabilityOfParticipantNumber(int dtoParticipantNumber)
        {
            if (_context.CompetitionApplications.Count(x => x.ParticipantNumber == dtoParticipantNumber) > 0)
                return false;
            return true;
        }

        public void SaveNewCompetitionApplication(CompetitionApplicationDto dto)
        {
            var competitionApplication = _mapper.Map<CompetitionApplication>(dto);
            var user = _authService.GetCurrentUserAsync().Result;
            competitionApplication.IdentityUser = user;
            competitionApplication.CarId = GetCar(user).Id;
            competitionApplication.ApplicantId = user.Id;
            _context.CompetitionApplications.Add(competitionApplication);
            _context.SaveChanges();
        }

        public bool IsApplicantApproved()
        {
	        var user = _authService.GetCurrentUserAsync().Result;
	        var application = _context.CompetitionApplications.FirstOrDefault(x => x.ApplicantId== user.Id) ?? new CompetitionApplication {ApprovedByMedics = false, ApprovedByOrganizer = false, ApprovedByTech = false};
	        return application.ApprovedByMedics && application.ApprovedByOrganizer && application.ApprovedByTech;
        }
    }
}