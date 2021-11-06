using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;
using drift.Models;
using drift.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
    public class CompetitionScoreDto
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Participant)}: {Participant}, {nameof(AngleScore)}: {AngleScore}, {nameof(TrackScore)}: {TrackScore}, {nameof(StyleScore)}: {StyleScore}, {nameof(Competition)}: {Competition}, {nameof(Attempt)}: {Attempt}, {nameof(BestAngle)}: {BestAngle}, {nameof(BestTrack)}: {BestTrack}, {nameof(BestStyle)}: {BestStyle}, {nameof(Total)}: {Total}";
        }

        public UserDto Participant { get; set; }
        public int AngleScore { get; set; }
        public int TrackScore { get; set; }
        public int StyleScore { get; set; }
        public CompetitionDto Competition { get; set; }
        public int Attempt { get; set; }
        public int BestAngle { get; set; }

        public int BestTrack { get; set; }

        public int BestStyle { get; set; }

        public int Total { get; set; }

        public string ParticipantName { get; set; }
    }
}