using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
	public class CompetitionApplication
	{
		public int Id { get; set; }

		[Required]
		[ForeignKey(nameof(IdentityUser))]
		public string ApplicantId { get; set; }

		[Required] public int CarId { get; set; }

		public Car Car { get; set; }

		[Required] public int ParticipantNumber { get; set; }

		public double Model { get; set; }

		//owner id
		public IdentityUser IdentityUser { get; set; }

		[Required] public int CompetitionId { get; set; }

		public Competition Competition { get; set; }
		public bool ApprovedByMedics { get; set; }
		public bool ApprovedByTech { get; set; }
		public bool ApprovedByOrganizer { get; set; }
	}
}