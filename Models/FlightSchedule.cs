using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class FlightSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FlightID { get; set; }
        public string Type { get; set; }
        public string Route { get; set; }
        public string Boarding { get; set; }
        public string Landing { get; set; }
        public string AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public string Terminal { get; set; }
        public string Gate { get; set; }
    }
}