using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;

namespace drift.Service
{
    public class CompetitionResultService
    {
        private ApplicationDbContext db;
        private IMapper _mapper;
        private UserService _userService;

        public CompetitionResultService(UserService userService, IMapper mapper, ApplicationDbContext db)
        {
            _userService = userService;
            _mapper = mapper;
            this.db = db;
        }

        public List<CompetitionResultDto> getResults(int competitionId)
        {
            using (db)
            {
                var result = from cr in db.CompetitionResults
                    join c in db.Competitions on cr.CompetitionId equals c.Id
                    where cr.CompetitionId == competitionId
                    select new CompetitionResultDto()
                    {
                        Id = cr.Id,
                        Competition = _mapper.Map<CompetitionDto>(c),
                        Place = cr.Place,
                        CarNumber = cr.CarNumber,
                        QualificationScore = cr.QualificationScore,
                        MainPhaseScore = cr.MainPhaseScore,
                        TotalScore = cr.TotalScore,
                        User = _userService.findById(cr.ParticipantId)
                    };
                return result.Select(dto => dto).OrderBy(dto => dto.Place).ToList();
            }
        }


        public CompetitionResultDto storeResult(CompetitionResultDto dto)
        {
            using (db)
            {
                var result = db.CompetitionResults.Add(new CompetitionResult()
                {
                    CarNumber = dto.CarNumber,
                    CompetitionId = dto.Competition.Id,
                    MainPhaseScore = dto.MainPhaseScore,
                    ParticipantId = dto.User.Id,
                    Place = dto.Place,
                    QualificationScore = dto.QualificationScore,
                    TotalScore = dto.TotalScore,
                    ParticipantCar = dto.CarName
                });
                db.SaveChanges();
                dto.Id = result.Entity.Id;
            }

            return dto;
        }

        public CompetitionResultDto updateResult(CompetitionResultDto dto)
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
        }
    }
}