using System;
using System.Linq;
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
            bracket.CompetitionId = competitionId;
            return View(bracket);
        }


        [HttpPost]
        public IActionResult GetResults(SetResultsRequest request)
        {
            _resultService.storeResult(request);
            return RedirectToAction("GetResults", new {competitionId = request.competitionId});
        }

        [HttpGet]
        public IActionResult GetAllStagesResults(int competitionId)
        {
            var results = _resultService.GetAllStagesResults(competitionId);
            return View(results);
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
        public IActionResult GetScores(int competitionId, string participantName = null)
        {
            var competition = _userService.GetCompetition(competitionId);
            var score = _resultService.GetScore(competitionId);
            if (!string.IsNullOrEmpty(participantName))
            {
	            score = score.Where(x => x.ParticipantName == participantName).ToList();
                if (score.Count == 0)
                {
                    score = _resultService.GetScore(competitionId).Where( x => x.ParticipantNumber.ToString() == participantName).ToList();
                }
            }
            return View(new CompetitionScoreResponse()
                {Scores = score, competitionId = competition.Id, createdById = competition.CreatedById});
        }

        [HttpGet]
        public IActionResult StartMainPhase(int competitionId, bool autoGenerate)
        {
            _resultService.GenerateResults(competitionId, HttpContext.User.Identity.Name, autoGenerate);
            return RedirectToAction("GetResults", new {competitionId = competitionId});
        }
    }
}