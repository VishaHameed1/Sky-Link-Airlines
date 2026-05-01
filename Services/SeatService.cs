using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class SeatService
    {
        private readonly DatabaseContext _context;

        public SeatService(DatabaseContext context)
        {
            _context = context;
        }

        public SeatMap GetSeatMap(string flightId)
        {
            var seatMap = _context.SeatMaps.Find(s => s.FlightId == flightId).FirstOrDefault();
            if (seatMap == null)
            {
                seatMap = GenerateDefaultSeatMap(flightId);
                _context.SeatMaps.InsertOne(seatMap);
            }
            return seatMap;
        }

        private SeatMap GenerateDefaultSeatMap(string flightId)
        {
            var seats = new List<Seat>();
            string[] classes = { "First", "Business", "Economy" };
            int[] classRows = { 4, 10, 30 };
            string[] columns = { "A", "B", "C", "D", "E", "F" };

            int rowCounter = 1;
            for (int c = 0; c < classes.Length; c++)
            {
                for (int r = 0; r < classRows[c]; r++)
                {
                    foreach (var col in columns)
                    {
                        var isWindow = col == "A" || col == "F";
                        var isAisle = col == "C" || col == "D";

                        seats.Add(new Seat
                        {
                            SeatNumber = $"{rowCounter}{col}",
                            Row = rowCounter.ToString(),
                            Column = col,
                            Class = classes[c],
                            Type = isWindow ? "Window" : (isAisle ? "Aisle" : "Middle"),
                            IsAvailable = true,
                            Price = classes[c] == "First" ? 500 : (classes[c] == "Business" ? 300 : 100),
                            HasExtraLegroom = rowCounter == 1,
                            NearExit = rowCounter <= 2,
                            HasPowerOutlet = classes[c] != "Economy",
                            IsPremium = rowCounter <= 5 && classes[c] == "Economy"
                        });
                    }
                    rowCounter++;
                }
            }

            return new SeatMap
            {
                FlightId = flightId,
                AircraftType = "Boeing 777",
                TotalRows = rowCounter - 1,
                Seats = seats,
                LastUpdated = DateTime.Now
            };
        }

        public List<Seat> GetAvailableSeats(string flightId, string seatClass = null)
        {
            var seatMap = GetSeatMap(flightId);
            var availableSeats = seatMap.Seats.Where(s => s.IsAvailable).ToList();

            if (!string.IsNullOrEmpty(seatClass))
                availableSeats = availableSeats.Where(s => s.Class == seatClass).ToList();

            return availableSeats;
        }

        public bool AssignSeat(int pnr, string seatNumber)
        {
            // Find which flight this PNR belongs to
            var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            string flightId = booking?.FlightNumber;

            if (flightId == null)
            {
                var intBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
                flightId = intBooking?.FlightNumber;
            }

            if (flightId == null) return false;

            var seatMap = GetSeatMap(flightId);
            var seat = seatMap.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);

            if (seat == null || !seat.IsAvailable) return false;

            seat.IsAvailable = false;
            seat.PassengerName = booking?.PassengerName ?? "Passenger";
            seat.PassengerPNR = pnr.ToString();
            seatMap.LastUpdated = DateTime.Now;

            _context.SeatMaps.ReplaceOne(s => s.FlightId == flightId, seatMap);
            return true;
        }

        public decimal GetSeatPrice(string flightId, string seatNumber)
        {
            var seatMap = GetSeatMap(flightId);
            var seat = seatMap.Seats.FirstOrDefault(s => s.SeatNumber == seatNumber);
            return seat?.Price ?? 0;
        }

        public void DisplaySeatMap(string flightId)
        {
            var seatMap = GetSeatMap(flightId);

            Console.WriteLine($"\n  Seat Map for Flight {flightId}");
            Console.WriteLine("  " + new string('=', 60));

            var seatsByRow = seatMap.Seats.GroupBy(s => s.Row);

            foreach (var row in seatsByRow)
            {
                Console.Write($"  Row {row.Key,-3} ");
                foreach (var seat in row)
                {
                    char display;
                    if (!seat.IsAvailable)
                        display = 'X';
                    else if (seat.Class == "First")
                        display = 'F';
                    else if (seat.Class == "Business")
                        display = 'B';
                    else if (seat.IsPremium)
                        display = 'P';
                    else
                        display = 'E';

                    Console.Write($"[{seat.SeatNumber} {display}] ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("  Legend: F=First, B=Business, P=Premium Economy, E=Economy, X=Booked\n");
        }
    }
}