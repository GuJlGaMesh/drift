using drift.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using drift.Data.Entity;
using drift.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace drift.Controllers
{
    public class CompetitionController : Controller
    {
        private CompetitionService _competitionService;
        private UserService _userService;
        private readonly ApprovingService _approvingService;

        public CompetitionController(CompetitionService competitionService, UserService userService,
            ApprovingService approvingService)
        {
            _competitionService = competitionService;
            _userService = userService;
            _approvingService = approvingService;
        }

        [HttpGet]
        [Authorize(Roles = "ORGANIZER")]
        public IActionResult CreateCompetition()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCompetition(CompetitionDto dto)
        {
            var creatorId = HttpContext.User.Identity?.Name;
            var created = _competitionService.CreateCompetition(dto, creatorId);
            return RedirectToAction("GetCompetition", "Competition", new {id = created.Id});
        }

        [HttpGet]
        public IActionResult GetCompetition(int id)
        {
            var competition = _competitionService.GetById(id);
            var applications = _userService.GetApprovedApplicationsByCompetition(competition.Id);
            var ignoredApplications = _userService.GetIgnoredApplicationsByCompetition(competition.Id);
            competition.Applications = applications.Union(ignoredApplications).ToList();

            return View(competition);
        }

        [HttpGet]
        public IActionResult GetCompetitions()
        {
            var competitions = _competitionService.FindCompetitions();
            return View(competitions);
        }

        [HttpGet]
        public IActionResult Approve(int? id)
        {
            var application = _approvingService.GetApplicationById(id.Value);
            application.ApprovedByOrganizer = true;
            _approvingService.UpdateOrganizerApplicationStatus(application);
            return RedirectToAction(nameof(GetCompetition), new {Id = application.CompetitionId});
        }

        [HttpGet]
        public IActionResult SetCompetitionScores(int competitionId, int attempts)
        {
            var competitiion = _userService.GetCompetition(competitionId);
            var participants = _userService.GetApprovedApplicationsByCompetition(competitiion.Id);
            return View(
                new CompetitionScoreUnsetDto()
                {
                    Participants = participants,
                    Attempts = attempts,
                    CompetitionId = competitiion.Id
                });
        }


        [HttpGet]
        public IActionResult GetCreatedCompetitions()
        {
            var userId = HttpContext.User.Identity.Name;
            var competitions = _competitionService.FindCreatedCompetitions(userId);
            return View(competitions);
        }

        [HttpGet]
        public IActionResult Decline(int? id)
        {
	        var application = _approvingService.GetApplicationById(id.Value);
		        application.Ignore = true;
		        _approvingService.UpdateMedicalApplicationStatus(application);
		        return RedirectToAction(nameof(GetCompetition), new { Id = application.CompetitionId });
        }
    }
}