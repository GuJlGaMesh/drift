using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using drift.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace drift.Controllers
{
	public class UserController : Controller
	{
		private readonly UserService _userService;
		private readonly AuthService _authService;
		private readonly IMapper _mapper;

		public UserController(IMapper mapper, UserService userService, AuthService authService)
		{
			_mapper = mapper;
			_userService = userService;
			_authService = authService;
		}

		// GET: User
		public async Task<IActionResult> Index()
		{
			dynamic model = new ExpandoObject();
			var userId = HttpContext.User.Identity.Name;
			model.Competitions = _userService.GetAllAvailableCompetitions();
			model.Car = _userService.GetCar(await _authService.GetCurrentUserAsync());
			model.IsApproved = _userService.IsApplicantApproved();
			return View(model);
		}

		public IActionResult CreateCar()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateCar([Bind("Name,Model")] CarDto car)
		{
			car.Owner = await _authService.GetCurrentUserAsync();
			_userService.SetCar(car);
			return RedirectToAction(nameof(Index));
		}

		public IActionResult EditCar(int? id)
		{
			return View(_userService.GetCarById(id));
		}

		[HttpPost]
		public async Task<IActionResult> EditCar([FromForm] CarDto car)
		{
			car.Owner = await _authService.GetCurrentUserAsync();
			_userService.SetCar(car);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Participate(int? id)
		{
			if (id == null) return NotFound();

			var competition = _userService.GetCompetition(id);
			var user = await _authService.GetCurrentUserAsync();
			var car = _userService.GetCar(user);
			if (car == null) return RedirectToAction("CreateCar");
			if (competition == null) return NotFound();
			var competitionApplication = new CompetitionApplicationDto()
			{
				CarModelAndName = car.Name + car.Model,
				CompetitionId = id.Value
			};
			//ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", competition.CreatedById);
			return View(competitionApplication);
		}

		[HttpPost]
		public async Task<IActionResult> Participate(CompetitionApplicationDto dto)
		{
			if (dto == null) return NotFound();
			var numAvailable = _userService.CheckAvailabilityOfParticipantNumber(dto);
			if (!numAvailable)
				dto.ParticipantNumberError = true;
			if (!dto.Paid)
				dto.PaidError = true;
			if (dto.PaidError || dto.ParticipantNumberError)
				return View(dto);

			var user = await _authService.GetCurrentUserAsync();

			var car = _userService.GetCar(user);

			if (car is null) return RedirectToAction("CreateCar");

			dto.ApplicantId = user.Id;

			_userService.SaveNewCompetitionApplication(dto);

			return Redirect(nameof(Index));
		}
	}
}