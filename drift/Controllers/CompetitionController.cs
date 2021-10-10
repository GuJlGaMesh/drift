using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using drift.Models.Dto;
using drift.Models.Request;
using drift.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace drift.Controllers
{
    public class CompetitionController : Controller
    {
        private CompetitionService _competitionService;

        public CompetitionController(CompetitionService competitionService)
        {
            _competitionService = competitionService;
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
            return View(competition);
        }

        [HttpGet]
        public IActionResult GetCompetitions()
        {
            var competitions = _competitionService.FindCompetitions();
            return View(competitions);
        }
    }
}