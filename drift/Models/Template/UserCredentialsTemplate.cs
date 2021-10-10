using System.Collections.Generic;

namespace drift.Models.Template
{
    public class UserCredentialsTemplate
    {
        public string Email { get; set; }
        public string Role { get; set; }

        public string UserId { get; set; }

        public UserCredentialsTemplate(string email, string? role, string userId)
        {
            Email = email;
            Role = role;
            UserId = userId;
        }
    }
}