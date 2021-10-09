namespace drift.Models.Template
{
    public class UserCredentialsTemplate
    {
        public string Email { get; set; }
        public string Role { get; set; }

        public UserCredentialsTemplate(string email, string role)
        {
            Email = email;
            Role = role;
        }
    }
}