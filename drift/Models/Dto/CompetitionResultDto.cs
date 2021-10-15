using System.ComponentModel.DataAnnotations;
using drift.Models;
using drift.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionResultDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public CompetitionDto Competition { get; set; }
        public int Place { get; set; }
        public int CarNumber { get; set; }
        public int QualificationScore { get; set; }
        public int MainPhaseScore { get; set; }
        public int TotalScore { get; set; }

        public string CarName { get; set; }
    }
}