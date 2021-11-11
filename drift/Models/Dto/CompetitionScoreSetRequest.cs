using System.Collections.Generic;
using drift.Data.Entity;

namespace drift.Models.Dto
{
    public class CompetitionScoreSetRequest
    {
        public List<CompetitionScoreDto> Scores { get; set; }
        public int competitionId { get; set; }
    }
}