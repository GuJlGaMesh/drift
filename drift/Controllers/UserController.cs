using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using drift.Service;

namespace drift.Controllers
{
	public class UserController : Controller
	{
		private readonly UserService _userService;
		private readonly AuthService _authService;
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public UserController(IMapper mapper,UserService userService, AuthService authService)
		{
			_mapper = mapper;
			_userService = userService;
			_authService = authService;
		}

		// GET: User
		public async Task<IActionResult> Index()
		{
			dynamic model = new ExpandoObject();
			model.Competitions = _userService.GetAllAvailableCompetitions();
			model.Car = _userService.GetCar(await _authService.GetCurrentUserAsync());
			return View(model);
		}

		// GET: User/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var competition = await _context.Competitions
				.Include(c => c.CreatedBy)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (competition == null) return NotFound();

			return View(competition);
		}

		// GET: User/Create
		public IActionResult Create()
		{
			ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
			return View();
		}
		
		public IActionResult CreateCar()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateCar([Bind("Name,Model")]CarDto car)
		{
			car.Owner = await _authService.GetCurrentUserAsync();
			_userService.SetCar(car);
			return RedirectToAction(nameof(Index));
		}
		

		// POST: User/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(
			[Bind("Id,StartDate,CreatedById,Name,RegistrationOpen,Finished,Description,Fee")]
			Competition competition)
		{
			if (ModelState.IsValid)
			{
				_context.Add(competition);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", competition.CreatedById);
			return View(competition);
		}

		// GET: User/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var competition = await _context.Competitions.FindAsync(id);
			if (competition == null) return NotFound();
			ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", competition.CreatedById);
			return View(competition);
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

			var competition = await _context.Competitions.FindAsync(id);
			if (competition == null) return NotFound();
			ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", competition.CreatedById);
			return View();
		}

		// POST: User/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id,
			[Bind("Id,StartDate,CreatedById,Name,RegistrationOpen,Finished,Description,Fee")]
			Competition competition)
		{
			if (id != competition.Id) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(competition);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CompetitionExists(competition.Id))
						return NotFound();
					else
						throw;
				}

				return RedirectToAction(nameof(Index));
			}

			ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", competition.CreatedById);
			return View(competition);
		}

		// GET: User/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var competition = await _context.Competitions
				.Include(c => c.CreatedBy)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (competition == null) return NotFound();

			return View(competition);
		}

		// POST: User/Delete/5
		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var competition = await _context.Competitions.FindAsync(id);
			_context.Competitions.Remove(competition);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool CompetitionExists(int id)
		{
			return _context.Competitions.Any(e => e.Id == id);
		}
	}
}