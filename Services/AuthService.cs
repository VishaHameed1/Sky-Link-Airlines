using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class AuthService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;

        public AuthService(DatabaseContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool Register(string username, string password, string email, string phone, string fullName)
        {
            try
            {
                var existingUser = _context.Users.Find(u => u.Username == username || u.Email == email).FirstOrDefault();
                if (existingUser != null)
                    return false;

                var user = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    Email = email,
                    Phone = phone,
                    FullName = fullName,
                    Role = "Customer",
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    FailedLoginAttempts = 0
                };

                _context.Users.InsertOne(user);
                _notificationService.SendEmail(email, "Welcome to Sky-Link Airlines",
                    $"Dear {fullName},\n\nThank you for registering with Sky-Link Airlines!\n\nYour account has been created successfully.\n\nUsername: {username}\n\nHappy travels!\n\nSky-Link Airlines Team");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public User Login(string username, string password)
        {
            var user = _context.Users.Find(u => u.Username == username || u.Email == username).FirstOrDefault();

            if (user == null)
                return null;

            if (user.LockoutUntil > DateTime.Now)
                return null;

            if (user.PasswordHash != HashPassword(password))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                    user.LockoutUntil = DateTime.Now.AddMinutes(30);

                _context.Users.ReplaceOne(u => u.Id == user.Id, user);
                return null;
            }

            user.FailedLoginAttempts = 0;
            user.LastLogin = DateTime.Now;
            user.LockoutUntil = null;
            _context.Users.ReplaceOne(u => u.Id == user.Id, user);

            return user;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _context.Users.Find(u => u.Username == username).FirstOrDefault();
            if (user == null || user.PasswordHash != HashPassword(oldPassword))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            _context.Users.ReplaceOne(u => u.Id == user.Id, user);

            _notificationService.SendEmail(user.Email, "Password Changed",
                $"Dear {user.FullName},\n\nYour password has been changed successfully.\n\nIf this wasn't you, please contact support immediately.\n\nSky-Link Airlines Team");

            return true;
        }

        public bool ForgotPassword(string email, string newPassword, string securityAnswer)
        {
            var user = _context.Users.Find(u => u.Email == email).FirstOrDefault();
            if (user == null || user.SecurityAnswerHash != HashPassword(securityAnswer.ToLower()))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            _context.Users.ReplaceOne(u => u.Id == user.Id, user);

            _notificationService.SendEmail(email, "Password Reset",
                $"Dear {user.FullName},\n\nYour password has been reset successfully.\n\nPlease login with your new password.\n\nSky-Link Airlines Team");

            return true;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Find(u => u.Email == email).FirstOrDefault();
        }
    }
}