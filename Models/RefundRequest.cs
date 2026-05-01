using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class RefundRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RequestId { get; set; }
        public int PNR { get; set; }
        public string PassengerName { get; set; }
        public string Email { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected, Processed
        public decimal RequestedAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }
        public string RejectionReason { get; set; }
        public string TransactionId { get; set; }
    }
}