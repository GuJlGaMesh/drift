using System;
using drift.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace drift.Controllers
{
    public class CompetitionResultController : Controller
    {
        private CompetitionResultService _resultService;
        private CompetitionScoreService _scoreService;

        public CompetitionResultController(CompetitionScoreService scoreService, CompetitionResultService resultService)
        {
            _scoreService = scoreService;
            _resultService = resultService;
        }

        [HttpGet]
        public IActionResult GetResults(int competitionId)
        {
            var bracket = _resultService.getResultsBracket(competitionId);
            return View(bracket);
        }

        [HttpGet]
        public IActionResult GetScores(int competitionId)
        {
            var score = _resultService.GetScore(competitionId);
            return View(score);
        }

        [HttpGet]
        public IActionResult GenerateResults(int competitionId)
        {
            _resultService.GenerateResults(competitionId, HttpContext.User.Identity.Name);
            return RedirectToAction("GetResults", new {competitionId = competitionId});
        }
    }
}