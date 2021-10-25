using drift.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using drift.Models.Dto;

namespace drift.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Car> Cars { get; set; }
		public DbSet<CompetitionApplication> CompetitionApplications { get; set; }
		public DbSet<Competition> Competitions { get; set; }
	}
}
