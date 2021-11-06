using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using drift.Data;
using drift.Data.Entity;
using drift.Service;

namespace drift.Controllers
{
    public class MedicalController : Controller
    {
        private readonly ApprovingService _approvingService;
        private readonly ApplicationDbContext _context;

        public MedicalController(ApprovingService approvingService, ApplicationDbContext context)
        {
	        _approvingService = approvingService;
	        _context = context;
        }

        // GET: Medical
        public async Task<IActionResult> Index()
        {
            return View(_approvingService.GetPendingApplications());
        }

        // GET: Medical/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var competitionApplication = await _context.CompetitionApplications
        //         .Include(c => c.Car)
        //         .Include(c => c.Competition)
        //         .Include(c => c.IdentityUser)
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (competitionApplication == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(competitionApplication);
        // }
        //
        // // GET: Medical/Create
        // public IActionResult Create()
        // {
        //     ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Model");
        //     ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "CreatedById");
        //     ViewData["ApplicantId"] = new SelectList(_context.Users, "Id", "Id");
        //     return View();
        // }
        //
        // // POST: Medical/Create
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,ApplicantId,CarId,ParticipantNumber,Model,CompetitionId,ApprovedByMedics,ApprovedByTech,ApprovedByOrganizer")] CompetitionApplication competitionApplication)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(competitionApplication);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Model", competitionApplication.CarId);
        //     ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "CreatedById", competitionApplication.CompetitionId);
        //     ViewData["ApplicantId"] = new SelectList(_context.Users, "Id", "Id", competitionApplication.ApplicantId);
        //     return View(competitionApplication);
        // }
        //
        // GET: Medical/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
        
            var competitionApplication = _approvingService.GetApplicationById(id.Value);
            if (competitionApplication == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Model", competitionApplication.CarId);
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "CreatedById", competitionApplication.CompetitionId);
            ViewData["ApplicantId"] = new SelectList(_context.Users, "Id", "Id", competitionApplication.ApplicantId);
            return View(competitionApplication);
        }
        
        // POST: Medical/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicantId,CarId,ParticipantNumber,Model,CompetitionId,ApprovedByMedics,ApprovedByTech,ApprovedByOrganizer")] CompetitionApplication competitionApplication)
        {
            if (id != competitionApplication.Id)
            {
                return NotFound();
            }
        
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competitionApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
	                throw;
	                // if (!CompetitionApplicationExists(competitionApplication.Id))
	                // {
	                //     return NotFound();
	                // }
	                // else
	                // {
	                //     throw;
	                // }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Model", competitionApplication.CarId);
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "CreatedById", competitionApplication.CompetitionId);
            ViewData["ApplicantId"] = new SelectList(_context.Users, "Id", "Id", competitionApplication.ApplicantId);
            return View(competitionApplication);
        }
        //
        // // GET: Medical/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var competitionApplication = await _context.CompetitionApplications
        //         .Include(c => c.Car)
        //         .Include(c => c.Competition)
        //         .Include(c => c.IdentityUser)
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (competitionApplication == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(competitionApplication);
        // }
        //
        // // POST: Medical/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var competitionApplication = await _context.CompetitionApplications.FindAsync(id);
        //     _context.CompetitionApplications.Remove(competitionApplication);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
        //
        // private bool CompetitionApplicationExists(int id)
        // {
        //     return _context.CompetitionApplications.Any(e => e.Id == id);
        // }
    }
}
