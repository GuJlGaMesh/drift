using Microsoft.AspNetCore.Identity;

namespace drift.Models.Dto
{
    public class CarDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }
        //owner id

        public IdentityUser Owner { get; set; }
    }
}