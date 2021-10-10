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
        public IActionResult CreateCompetition()
        {
            return View();
        }

        [HttpPost]
        public void CreateCompetition(CompetitionDto dto)
        {
            var creatorId = HttpContext.User.Identity?.Name;
            Console.WriteLine(HttpContext.User.Identity.Name);
            Console.WriteLine(dto);
            _competitionService.CreateCompetition(dto, creatorId);
        }
        
        [HttpGet]
        public IActionResult GetCompetition(int id)
        {
            var competition = _competitionService.GetById(id);
            return View(competition);
        }
    }
}