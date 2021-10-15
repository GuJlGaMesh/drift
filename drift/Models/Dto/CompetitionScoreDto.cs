using System.ComponentModel.DataAnnotations;
using drift.Models;
using drift.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionScoreDto
    {
        public int Id { get; set; }
        public UserDto Participant { get; set; }
        public int AngleScore { get; set; }
        public int TrackScore { get; set; }
        public int StyleScore { get; set; }
        public CompetitionDto Competition { get; set; }
        public int Attempt { get; set; }
        public CompetitionType Type;
    }
}