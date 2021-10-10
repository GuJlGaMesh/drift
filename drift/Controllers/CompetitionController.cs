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
        public IActionResult Competition(CompetitionDto dto)
        {
            var creatorId = HttpContext.User.Identity?.Name;
           var createdDto = _competitionService.CreateCompetition(dto, creatorId);
            return View(createdDto);
        }
    }
}