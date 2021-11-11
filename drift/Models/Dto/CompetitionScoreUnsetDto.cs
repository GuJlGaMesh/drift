using System.Collections.Generic;

namespace drift.Models.Dto
{
    public class CompetitionScoreUnsetDto
    {
        public List<CompetitionApplicationDto> Participants { get; set; }
        public int Attempts { get; set; }
        public int CompetitionId { get; set; }

    }
}