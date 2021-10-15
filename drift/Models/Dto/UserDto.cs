namespace drift.Models.Dto
{
    public class UserDto
    {
        private string Email { get; set; }
        private string UserName { get; set; }
        public string Id { get; set; }
        private string Password { get; set; }
        public UserRoleEnum RoleEnum { get; set; }
    }
}