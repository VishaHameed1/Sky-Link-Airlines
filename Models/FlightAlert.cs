using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class FlightAlert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AlertId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Route { get; set; }
        public decimal TargetPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastNotified { get; set; }
        public string PreferredClass { get; set; }
        public string Frequency { get; set; } // Daily, Weekly, Real-time
    }
}