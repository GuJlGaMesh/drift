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
            var competitionResults = _resultService.getResults(competitionId);
            return View(competitionResults);
        }
    }
}