using System;
using System.Collections.Generic;
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

        public string ParticipantName { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ParticipantId)}: {ParticipantId}, {nameof(AngleScore)}: {AngleScore}, {nameof(TrackScore)}: {TrackScore}, {nameof(StyleScore)}: {StyleScore}, {nameof(CompetitionId)}: {CompetitionId}, {nameof(Attempt)}: {Attempt}";
        }

        private sealed class CompetitionScoreEqualityComparer : IEqualityComparer<CompetitionScore>
        {
            public bool Equals(CompetitionScore x, CompetitionScore y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id && x.ParticipantId == y.ParticipantId && x.AngleScore == y.AngleScore && x.TrackScore == y.TrackScore && x.StyleScore == y.StyleScore && x.CompetitionId == y.CompetitionId && x.Attempt == y.Attempt && x.ParticipantName == y.ParticipantName;
            }

            public int GetHashCode(CompetitionScore obj)
            {
                return HashCode.Combine(obj.Id, obj.ParticipantId, obj.AngleScore, obj.TrackScore, obj.StyleScore, obj.CompetitionId, obj.Attempt, obj.ParticipantName);
            }
        }

        public static IEqualityComparer<CompetitionScore> CompetitionScoreComparer { get; } = new CompetitionScoreEqualityComparer();
    }
}