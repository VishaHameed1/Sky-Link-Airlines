using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class CheckIn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int PNR { get; set; }
        public DateTime CheckInTime { get; set; }
        public string CheckInMethod { get; set; } // Web, Mobile, Kiosk, Counter
        public string BoardingPassNumber { get; set; }
        public string QRCode { get; set; }
        public string SeatNumber { get; set; }
        public string BoardingGroup { get; set; }
        public int BoardingSequence { get; set; }
        public bool BaggageDropped { get; set; }
        public int BaggageCount { get; set; }
        public decimal ExcessBaggageFee { get; set; }
        public string CounterNumber { get; set; }
    }
}