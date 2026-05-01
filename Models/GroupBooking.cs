using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class GroupMember
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PassportNumber { get; set; }
        public string SeatNumber { get; set; }
        public int PNR { get; set; }
    }

    public class GroupBooking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int NumberOfPassengers { get; set; }
        public List<GroupMember> Members { get; set; }
        public decimal GroupDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SpecialRequirements { get; set; }
        public string FlightId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }
}