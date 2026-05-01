using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class AirportService
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ServiceType { get; set; } // Lounge, Parking, MeetGreet, FastTrack, Wheelchair, UnaccompaniedMinor
        public string BookingId { get; set; }
        public int PNR { get; set; }
        public string PassengerName { get; set; }
        public DateTime BookingTime { get; set; }
        public DateTime ServiceDateTime { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string SpecialInstructions { get; set; }
        public string Location { get; set; } // Terminal, Gate, etc.
    }
}