using System;
using drift.Models.Dto;
using drift.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace drift.Controllers
{
    public class CompetitionResultController : Controller
    {
        private CompetitionResultService _resultService;
        private CompetitionScoreService _scoreService;
        private UserService _userService;

        public CompetitionResultController(CompetitionScoreService scoreService, CompetitionResultService resultService,
            UserService userService)
        {
            _scoreService = scoreService;
            _resultService = resultService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetResults(int competitionId)
        {
            var bracket = _resultService.getResultsBracket(competitionId);
            return View(bracket);
        }

        [HttpPost]
        public IActionResult SetScores(CompetitionScoreSetRequest request)
        {
            _scoreService.SetCompetitionScore(request.Scores, request.competitionId);
            return RedirectToAction("GetScores", new
            {
                competitionId = request.competitionId
            });
        }


        [HttpGet]
        public IActionResult GetScores(int competitionId)
        {
            var competition = _userService.GetCompetition(competitionId);
            var score = _resultService.GetScore(competitionId);
            return View(new CompetitionScoreResponse()
                {Scores = score, competitionId = competition.Id, createdById = competition.CreatedById});
        }

        [HttpGet]
        public IActionResult StartMainPhase(int competitionId)
        {
            _resultService.GenerateResults(competitionId, HttpContext.User.Identity.Name);
            return RedirectToAction("GetResults", new {competitionId = competitionId});
        }
    }
}