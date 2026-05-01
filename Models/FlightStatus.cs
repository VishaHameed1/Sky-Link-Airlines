using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class FlightStatus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FlightId { get; set; }
        public string Status { get; set; } // Scheduled, On Time, Delayed, Boarding, Departed, In Air, Arrived, Cancelled
        public int DelayMinutes { get; set; }
        public string DelayReason { get; set; }
        public string CurrentLocation { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public DateTime LastUpdate { get; set; }
        public string WeatherCondition { get; set; }
        public int GateChangeCount { get; set; }
        public string PreviousGate { get; set; }
        public string CurrentGate { get; set; }
    }
}