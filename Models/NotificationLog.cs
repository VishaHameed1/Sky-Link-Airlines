using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class NotificationLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Recipient { get; set; }
        public string Type { get; set; } // Email, SMS, Push
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDelivered { get; set; }
        public string ErrorMessage { get; set; }
        public string ReferenceId { get; set; } // PNR, TransactionId, etc.
    }
}