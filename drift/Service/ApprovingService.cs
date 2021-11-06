using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace drift.Service
{
    public class ApprovingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ApprovingService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<CompetitionApplicationDto> GetPendingApplications()
        {
            var applications = _context.CompetitionApplications.Include(c => c.Competition).Include(c => c.IdentityUser).ToList();
            return _mapper.Map<IEnumerable<CompetitionApplicationDto>>(applications.AsEnumerable());
        }


        public CompetitionApplicationDto GetApplicationById(int id)
        {
	        var x = _context.CompetitionApplications.Find(id);
	        return _mapper.Map<CompetitionApplicationDto>(x);
        }
    }
}