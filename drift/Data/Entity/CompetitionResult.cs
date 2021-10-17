using System.ComponentModel.DataAnnotations;
using drift.Models;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionResult
    {
        public int Id { get; set; }
        [Required] public string ParticipantId { get; set; }
        [Required] public int CompetitionId { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantCar { get; set; }
        public int Place { get; set; }
        public int CarNumber { get; set; }
        
        public int FirstPhaseScore { get; set; }
        
        public int SecondPhaseScore { get; set; }
        
        public int ThirdPhaseScore { get; set; }
        
        public int FourthPhaseScore { get; set; }
        public int TotalScore { get; set; }
    }
}