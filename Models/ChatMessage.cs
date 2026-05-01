using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsFromCustomer { get; set; }
        public string SessionId { get; set; }
        public List<string> Attachments { get; set; }
    }

    public class ChatSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string AssignedAgent { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Status { get; set; }
        public int MessageCount { get; set; }
    }
}