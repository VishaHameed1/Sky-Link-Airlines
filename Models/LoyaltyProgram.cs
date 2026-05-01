using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class MileTransaction
    {
        public DateTime Date { get; set; }
        public int Miles { get; set; }
        public string Type { get; set; } // Earned, Redeemed, Expired
        public string Description { get; set; }
        public string ReferenceId { get; set; } // PNR or TransactionId
    }

    public class LoyaltyProgram
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string Tier { get; set; } // Bronze, Silver, Gold, Platinum
        public int TotalMiles { get; set; }
        public int AvailableMiles { get; set; }
        public int MilesToNextTier { get; set; }
        public List<MileTransaction> Transactions { get; set; }
        public DateTime TierValidUntil { get; set; }
        public DateTime LastActivity { get; set; }
        public int LifetimeMiles { get; set; }
        public int TotalFlights { get; set; }
        public decimal TotalSpent { get; set; }
    }
}