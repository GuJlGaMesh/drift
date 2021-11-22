using System.Collections.Generic;
using drift.Data.Entity;

namespace drift.Models.Dto
{
    public class CompetitionScoreResponse
    {
        public List<CompetitionScoreDto> Scores { get; set; }
        public int competitionId;
        public string createdById;
        public string participantName { get; set; }
    }
}