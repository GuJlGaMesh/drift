using System.ComponentModel.DataAnnotations;

namespace drift.Models.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }

        [DataType(DataType.Password)] 
        public string Password { get; set; }
    }
}