using System;
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
        private ApplicationDbContext _db;
        private IMapper _mapper;
        private UserService _userService;

        public CompetitionService(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            _mapper = mapper;
        }

        public CompetitionDto CreateCompetition(CompetitionDto dto, string creatorId)
        {
            var entity = _mapper.Map<Competition>(dto);
            var creator = _db.Users.FirstOrDefault(u => u.Id == creatorId);
            entity.CreatedBy = creator;
            entity.RegistrationOpen = true;
            _db.Competitions.Add(entity);
            _db.SaveChanges();
            return _mapper.Map<Competition, CompetitionDto>(entity);
        }

        public CompetitionDto GetById(int id)
        {
            return _mapper.Map<Competition, CompetitionDto>(_db.Competitions.FirstOrDefault(c => c.Id == id));
        }

        public List<CompetitionDto> FindCompetitions()
        {
            var competitions = _db.Competitions.Select(c => c).ToList();
            return _mapper.Map<IEnumerable<Competition>, List<CompetitionDto>>(competitions);
        }

        public List<CompetitionDto> FindCreatedCompetitions(string userId)
        {
            var competitions = _db.Competitions.Where(c => c.CreatedById == userId).Select(c => c).ToList();
            return _mapper.Map<IEnumerable<Competition>, List<CompetitionDto>>(competitions);
        }
    }
}