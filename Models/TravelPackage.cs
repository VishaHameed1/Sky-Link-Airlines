using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class TravelPackage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PackageId { get; set; }
        public string Destination { get; set; }
        public string HotelName { get; set; }
        public int Nights { get; set; }
        public decimal PackagePrice { get; set; }
        public List<string> Inclusions { get; set; }
        public List<string> Exclusions { get; set; }
        public string MealPlan { get; set; }
        public string RoomType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string FlightId { get; set; }
        public bool IsActive { get; set; }
    }
}