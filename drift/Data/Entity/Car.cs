using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace drift.Data.Entity
{
	public class Car
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Model { get; set; }
		//owner id
		[Required]
		public IdentityUser Owner { get; set; }
	}
}
