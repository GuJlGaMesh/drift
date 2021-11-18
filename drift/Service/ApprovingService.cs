using AutoMapper;
using drift.Data;
using drift.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<CompetitionApplicationDto> GetPendingMedicsApplications()
		{
			var applications = _context.CompetitionApplications.Include(c => c.Competition).Include(c => c.IdentityUser)
				.ToList().Where(x => !x.ApprovedByMedics && !x.Ignore);
			return _mapper.Map<IEnumerable<CompetitionApplicationDto>>(applications.AsEnumerable());
		}

		public IEnumerable<CompetitionApplicationDto> GetPendingTechApplications()
		{
			var applications = _context.CompetitionApplications.Include(c => c.Competition).Include(c => c.IdentityUser).ToList()
				.Where(x => !x.ApprovedByTech && !x.Ignore);
			return _mapper.Map<IEnumerable<CompetitionApplicationDto>>(applications.AsEnumerable());
		}

		public IEnumerable<CompetitionApplicationDto> GetPendingOrganizerApplications()
		{
			var applications = _context.CompetitionApplications.Include(c => c.Competition).Include(c => c.IdentityUser).ToList()
				.Where(x => !x.ApprovedByOrganizer && !x.Ignore);
			return _mapper.Map<IEnumerable<CompetitionApplicationDto>>(applications.AsEnumerable());
		}


		public CompetitionApplicationDto GetApplicationById(int id)
		{
			var x = _context.CompetitionApplications.Find(id);
			return _mapper.Map<CompetitionApplicationDto>(x);
		}

		public void UpdateMedicalApplicationStatus(CompetitionApplicationDto dto)
		{
			var compApl =
				_context.CompetitionApplications.First(x => x.Id == dto.ApplicationId);

			compApl.ApprovedByMedics = dto.ApprovedByMedics;
			compApl.Ignore = dto.Ignore;
			_context.SaveChanges();

		}

		public void UpdateTechApplicationStatus(CompetitionApplicationDto dto)
		{
			var compApl =
				_context.CompetitionApplications.First(x => x.Id == dto.ApplicationId);

			compApl.ApprovedByTech = dto.ApprovedByTech;
			compApl.Ignore = dto.Ignore;
			_context.SaveChanges();

		}

		public void UpdateOrganizerApplicationStatus(CompetitionApplicationDto dto)
		{
			var compApl =
				_context.CompetitionApplications.First(x => x.Id == dto.ApplicationId);

			compApl.ApprovedByOrganizer = dto.ApprovedByOrganizer;
			compApl.Ignore = dto.Ignore;
			_context.SaveChanges();

		}
	}
}