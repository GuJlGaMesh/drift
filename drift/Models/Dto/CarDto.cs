using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace drift.Models.Dto
{
    public class CarDto
    {
        public int Id { get; set; }
        [DisplayName("Имя")]
        public string Name { get; set; }
        [DisplayName("Модель")]
        public string Model { get; set; }
        //owner id

        public IdentityUser Owner { get; set; }
    }
}