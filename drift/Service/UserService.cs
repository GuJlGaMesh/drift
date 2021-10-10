using System;
using System.Linq;
using drift.Data;
using drift.Models.Request;
using drift.Models.Template;
using Microsoft.AspNetCore.Identity;

namespace drift.Service
{
    public class UserService
    {
        private ApplicationDbContext db;

        private const string USER_ROLE = "User";

        public UserService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IdentityUser Register(RegisterRequest request)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user != null)
            {
                throw new Exception("Aboba");
            }

            user = db.Users.Add(new IdentityUser
            {
                Email = request.Email, UserName = request.UserName,
                PasswordHash = PasswordEncoder.Encrypt(request.Password)
            }).Entity;
            db.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = user.Id, RoleId = Enum.GetName(request.Role) ?? USER_ROLE
            });
            db.SaveChanges();

            return user;
        }

        public UserCredentialsTemplate Login(LoginRequest loginRequest)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
            if (user == null || !PasswordEncoder.Decrypt(user.PasswordHash).Equals(loginRequest.Password))
            {
                return null;
            }

            var userRole = db.UserRoles.FirstOrDefault(r =>
                r.UserId == user.Id);
            var role = db.Roles.FirstOrDefault(r =>
                r.Id == userRole.RoleId);
            return new UserCredentialsTemplate(user.Email, role?.Name ?? USER_ROLE,user.Id);
        }
    }
}