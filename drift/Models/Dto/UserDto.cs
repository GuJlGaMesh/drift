namespace drift.Models.Dto
{
    public class UserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }
        public UserRoleEnum RoleEnum { get; set; }
    }
}