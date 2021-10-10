using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
	public class Competition
	{
		public int Id { get; set; }

		[Required]
		public DateTime StartDate { get; set; }
		[ForeignKey("CreatedBy")]
		public string CreatedById { get; set; }
		[Required]
		public IdentityUser CreatedBy { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public bool RegistrationOpen { get; set; }
		public bool Finished { get; set; }

		public string Description { get; set; }
		[Required]
		public double Fee { get; set; }

	}
}
