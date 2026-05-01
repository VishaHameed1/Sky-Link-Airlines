using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class ScheduleService
    {
        private readonly DatabaseContext _context;

        public ScheduleService(DatabaseContext context)
        {
            _context = context;
        }

        public List<FlightSchedule> GetAllFlights()
        {
            return _context.FlightSchedules.Find(_ => true).ToList();
        }

        public List<FlightSchedule> GetDomesticFlights()
        {
            return _context.FlightSchedules.Find(s => s.Type == "Domestic").ToList();
        }

        public List<FlightSchedule> GetInternationalFlights()
        {
            return _context.FlightSchedules.Find(s => s.Type == "International").ToList();
        }

        public FlightSchedule GetFlightById(string flightId)
        {
            return _context.FlightSchedules.Find(s => s.FlightID == flightId).FirstOrDefault();
        }

        public bool AddFlight(FlightSchedule flight)
        {
            try
            {
                _context.FlightSchedules.InsertOne(flight);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateFlight(string flightId, FlightSchedule updatedFlight)
        {
            var update = Builders<FlightSchedule>.Update
                .Set(s => s.Boarding, updatedFlight.Boarding)
                .Set(s => s.Landing, updatedFlight.Landing)
                .Set(s => s.AvailableSeats, updatedFlight.AvailableSeats)
                .Set(s => s.Gate, updatedFlight.Gate)
                .Set(s => s.Price, updatedFlight.Price);

            var result = _context.FlightSchedules.UpdateOne(s => s.FlightID == flightId, update);
            return result.ModifiedCount > 0;
        }

        public bool DeleteFlight(string flightId)
        {
            var result = _context.FlightSchedules.DeleteOne(s => s.FlightID == flightId);
            return result.DeletedCount > 0;
        }

        public bool UpdateSeats(string flightId, string newSeats)
        {
            var update = Builders<FlightSchedule>.Update.Set(s => s.AvailableSeats, newSeats);
            var result = _context.FlightSchedules.UpdateOne(s => s.FlightID == flightId, update);
            return result.ModifiedCount > 0;
        }

        public void SeedInitialFlights()
        {
            if (_context.FlightSchedules.CountDocuments(_ => true) == 0)
            {
                var initial = new List<FlightSchedule>
                {
                    new FlightSchedule { FlightID="AI-202", Type="Domestic", Route="Karachi -> Lahore", Boarding="09:00 AM", Landing="10:45 AM", AvailableSeats="50", Price=120.00m, Gate="A1", Terminal="Domestic Terminal 1" },
                    new FlightSchedule { FlightID="IG-405", Type="Domestic", Route="Islamabad -> Karachi", Boarding="02:30 PM", Landing="04:15 PM", AvailableSeats="45", Price=110.00m, Gate="A2", Terminal="Domestic Terminal 1" },
                    new FlightSchedule { FlightID="EK-601", Type="International", Route="Karachi -> Dubai", Boarding="03:00 AM", Landing="05:30 AM", AvailableSeats="100", Price=450.00m, Gate="B1", Terminal="International Terminal" },
                    new FlightSchedule { FlightID="QA-992", Type="International", Route="Islamabad -> Doha", Boarding="11:15 PM", Landing="02:45 AM", AvailableSeats="85", Price=500.00m, Gate="B2", Terminal="International Terminal" },
                    new FlightSchedule { FlightID="SJ-118", Type="Domestic", Route="Quetta -> Karachi", Boarding="08:00 AM", Landing="09:30 AM", AvailableSeats="30", Price=95.00m, Gate="A3", Terminal="Domestic Terminal 2" },
                    new FlightSchedule { FlightID="BA-007", Type="International", Route="London -> Islamabad", Boarding="10:00 PM", Landing="08:00 AM", AvailableSeats="120", Price=800.00m, Gate="B3", Terminal="International Terminal" }
                };
                _context.FlightSchedules.InsertMany(initial);
            }
        }
    }
}