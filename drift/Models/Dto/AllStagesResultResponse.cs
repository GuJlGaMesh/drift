using System.Collections.Generic;
using drift.Data.Entity;

namespace drift.Models.Dto
{
    public class AllStagesResultResponse
    {
        public List<CompetitionResultDto> Results { get; set; }
        public List<CompetitionDto> CompetitionDtos { get; set; }
    }
}