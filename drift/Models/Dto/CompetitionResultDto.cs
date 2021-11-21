using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using drift.Models;
using drift.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionResultDto
    {
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id},{nameof(ParticipantName)}: {ParticipantName}";
        }

        public int Id { get; set; }
        public UserDto User { get; set; }
        public CompetitionDto Competition { get; set; }
        public int Place { get; set; }
        public int CarNumber { get; set; }
        public int QualificationScore { get; set; }
        public int? FirstPhaseScore { get; set; }

        public int? SecondPhaseScore { get; set; }

        public int? ThirdPhaseScore { get; set; }

        public int? FourthPhaseScore { get; set; }
        public int TotalScore { get; set; }
        public string ParticipantName { get; set; }

        public Dictionary<int, int> competitionScores { get; set; } = new Dictionary<int, int>();
    }
}