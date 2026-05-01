using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // Customer, Admin, Staff
        public string SecurityQuestion { get; set; }
        public string SecurityAnswerHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public string TwoFactorSecret { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}