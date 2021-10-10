﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
	public class CompetitionApplication
	{
		public int Id { get; set; }

		[Required]
		public string ApplicantId { get; set; }
		[Required]
		public int CarId { get; set; }
		public Car Car { get; set; }
		[Required]
		public int ParticipantNumber { get; set; }
		public double Model { get; set; }
		//owner id
		public IdentityUser IdentityUser { get; set; }

		public bool ApprovedByMedics { get; set; }
		public bool ApprovedByTech{ get; set; }
		public bool ApprovedByOrganizer{ get; set; }
	}
}