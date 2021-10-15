using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models;
using drift.Models.Dto;

namespace drift.Service
{
    public class CompetitionScoreService
    {
        private ApplicationDbContext db;
        private IMapper _mapper;
        private UserService _userService;

        public CompetitionScoreService(UserService userService, IMapper mapper, ApplicationDbContext db)
        {
            _userService = userService;
            _mapper = mapper;
            this.db = db;
        }

        public List<CompetitionScoreDto> getCompetitionScore(int competitionId)
        {
            using (db)
            {
                var score = from cs in db.CompetitionScores
                    join c in db.Competitions on cs.CompetitionId equals c.Id
                    where cs.CompetitionId == competitionId
                    select new CompetitionScoreDto()
                    {
                        Id = cs.Id,
                        Competition = _mapper.Map<CompetitionDto>(c),
                        AngleScore = cs.AngleScore,
                        TrackScore = cs.TrackScore,
                        StyleScore = cs.StyleScore,
                        Attempt = cs.Attempt,
                        Type = cs.Type,
                        Participant = _userService.findById(cs.ParticipantId)
                    };
                return score.Select(dto => dto).ToList();
            }
        }


        public CompetitionScoreDto storeScore(CompetitionScoreDto dto)
        {
            using (db)
            {
                var result = db.CompetitionScores.Add(new CompetitionScore()
                {
                    ParticipantId = dto.Participant.Id,
                    CompetitionId = dto.Competition.Id,
                    AngleScore = dto.AngleScore,
                    Attempt = dto.Attempt,
                    Type = dto.Type,
                });
                db.SaveChanges();
                dto.Id = result.Entity.Id;
            }

            return dto;
        }

        /*public CompetitionResultDto updateResult(CompetitionResultDto dto)
        {
            using (db)
            {
                var competitionResult = db.CompetitionResults.FirstOrDefault(cr => cr.Id == dto.Id);
                if (competitionResult != null)
                {
                    competitionResult.QualificationScore = dto.QualificationScore;
                    competitionResult.MainPhaseScore = dto.MainPhaseScore;
                    competitionResult.TotalScore = dto.TotalScore;
                    db.SaveChanges();
                }
            }

            return dto;
        }*/
    }
}