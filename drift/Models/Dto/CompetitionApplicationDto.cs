using System.ComponentModel.DataAnnotations;
using drift.Data.Entity;
using Microsoft.AspNetCore.Identity;

namespace drift.Models.Dto
{
	public class CompetitionApplicationDto
	{
		public int Id { get; set; }

		public string ApplicantId { get; set; }
		public int CarId { get; set; }
		public Car Car { get; set; }
		[Required] public int ParticipantNumber { get; set; }

		public double Model { get; set; }

		//owner id
		public IdentityUser IdentityUser { get; set; }

		public bool ApprovedByMedics { get; set; }
		public bool ApprovedByTech { get; set; }
		public bool ApprovedByOrganizer { get; set; }
	}
}