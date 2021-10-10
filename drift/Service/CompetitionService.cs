using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;

namespace drift.Service
{
    public class CompetitionService
    {
        private ApplicationDbContext db;
        private IMapper _mapper;
        private UserService _userService;

        public CompetitionService(ApplicationDbContext db, IMapper mapper)
        {
            this.db = db;
            _mapper = mapper;
        }

        public CompetitionDto CreateCompetition(CompetitionDto dto, string creatorId)
        {
            var entity = _mapper.Map<Competition>(dto);
            entity.CreatedBy = db.Users.FirstOrDefault(u => u.Id == creatorId);
            db.Competitions.Add(entity);
            db.SaveChanges();
            return _mapper.Map<Competition, CompetitionDto>(entity);
        }

        public CompetitionDto GetById(int id)
        {
            return _mapper.Map<Competition, CompetitionDto>(db.Competitions.FirstOrDefault(c => c.Id == id));
        }

        public List<CompetitionDto> FindCompetitions()
        {
            return _mapper.Map<IEnumerable<Competition>, List<CompetitionDto>>(db.Competitions.Select(c => c).ToList());
        }
    }
}