using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class Seat
    {
        public string SeatNumber { get; set; }
        public string Row { get; set; }
        public string Column { get; set; }
        public string Class { get; set; } // Economy, Business, First
        public string Type { get; set; } // Window, Aisle, Middle
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public string PassengerName { get; set; }
        public string PassengerPNR { get; set; }
        public bool HasExtraLegroom { get; set; }
        public bool NearExit { get; set; }
        public bool HasPowerOutlet { get; set; }
        public bool IsPremium { get; set; }
    }

    public class SeatMap
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FlightId { get; set; }
        public string AircraftType { get; set; }
        public int TotalRows { get; set; }
        public List<Seat> Seats { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}