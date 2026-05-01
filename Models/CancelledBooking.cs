using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class CancelledBooking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int PNR { get; set; }
        public string PassengerName { get; set; }
        public string BookingType { get; set; }
        public DateTime CancelledDate { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }
    }
}