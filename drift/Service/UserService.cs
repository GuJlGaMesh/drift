using System;
using System.Linq;
using drift.Data;
using drift.Models.Request;
using drift.Models.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            var encrypted = PasswordEncoder.Encrypt(request.Password);
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
            db.SaveChanges();

            return user;
        }

        public UserCredentialsTemplate Login(LoginRequest loginRequest)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
            if (!PasswordEncoder.Decrypt(user.PasswordHash).Equals((loginRequest.Password)))
            {
                return null;
            }

            if (user != null)
            {
                var userRole = db.UserRoles.FirstOrDefault(r =>
                    r.UserId == user.Id);
                var role = db.Roles.FirstOrDefault(r =>
                    r.Id == userRole.RoleId);
                return new UserCredentialsTemplate(user.Email, role?.Name ?? USER_ROLE);
            }

            return null;
        }
    }
}