using System.ComponentModel.DataAnnotations;
using drift.Models;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionScore
    {
        public int Id { get; set; }
        [Required] public string ParticipantId { get; set; }
        public int AngleScore { get; set; }
        public int TrackScore { get; set; }
        public int StyleScore { get; set; }
        [Required] public int CompetitionId { get; set; }
        public int Attempt { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ParticipantId)}: {ParticipantId}, {nameof(AngleScore)}: {AngleScore}, {nameof(TrackScore)}: {TrackScore}, {nameof(StyleScore)}: {StyleScore}, {nameof(CompetitionId)}: {CompetitionId}, {nameof(Attempt)}: {Attempt}";
        }
    }
}