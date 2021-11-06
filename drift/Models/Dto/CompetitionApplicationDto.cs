using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using drift.Data.Entity;
using Microsoft.AspNetCore.Identity;

namespace drift.Models.Dto
{
	public class CompetitionApplicationDto
	{
		public int ApplicationId { get; set; }

		public string ApplicantId { get; set; }
		public int CarId { get; set; }
		public Car Car { get; set; }
		[DisplayName("Номер для участия")]
		[Required] public int ParticipantNumber { get; set; }
		[DisplayName("Оплата")]
		public bool Paid { get; set; }
		public int CompetitionId { get; set; }
		[DisplayName("Соревнование")]
		public Competition Competition { get; set; }
		public string CarModelAndName { get; set; }
		//owner id
		[DisplayName("Данные участника")]
		public IdentityUser IdentityUser { get; set; }

		public string ParticipantName => IdentityUser?.UserName;
		public bool PaidError { get; set; }
		public bool ParticipantNumberError { get; set; }
		[DisplayName("Подтверждение")]
		public bool ApprovedByMedics { get; set; }
		public bool ApprovedByTech { get; set; }
		public bool ApprovedByOrganizer { get; set; }

		public override string ToString()
		{
			return $"{nameof(ApplicationId)}: {ApplicationId}, {nameof(ApplicantId)}: {ApplicantId}, {nameof(CarId)}: {CarId}, {nameof(Car)}: {Car}, {nameof(ParticipantNumber)}: {ParticipantNumber}, {nameof(Paid)}: {Paid}, {nameof(CompetitionId)}: {CompetitionId}, {nameof(Competition)}: {Competition}, {nameof(CarModelAndName)}: {CarModelAndName}, {nameof(IdentityUser)}: {IdentityUser}, {nameof(ParticipantName)}: {ParticipantName}, {nameof(PaidError)}: {PaidError}, {nameof(ParticipantNumberError)}: {ParticipantNumberError}, {nameof(ApprovedByMedics)}: {ApprovedByMedics}, {nameof(ApprovedByTech)}: {ApprovedByTech}, {nameof(ApprovedByOrganizer)}: {ApprovedByOrganizer}";
		}
	}
}