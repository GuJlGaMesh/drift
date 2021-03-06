namespace drift.Models.Request
{
    public class RegisterRequest
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public UserRoleEnum RoleEnum { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return $"{nameof(Email)}: {Email}, " +
                   $"{nameof(UserName)}: {UserName}, {nameof(RoleEnum)}: {RoleEnum}, {nameof(Password)}: {Password}";
        }
    }
}