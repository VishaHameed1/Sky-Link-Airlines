using System;
using System.Linq;
using MongoDB.Driver;
using AirlineReservationConsole;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class BookingService
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleService _scheduleService;
        private readonly Random _random;

        public BookingService(DatabaseContext context, ScheduleService scheduleService)
        {
            _context = context;
            _scheduleService = scheduleService;
            _random = new Random();
        }

        public bool BookDomesticFlight(dom_booking booking, string flightId)
        {
            try
            {
                _context.DomesticBookings.InsertOne(booking);

                var flight = _scheduleService.GetFlightById(flightId);
                if (flight != null)
                {
                    int currentSeats = int.Parse(flight.AvailableSeats);
                    string newSeats = (currentSeats - 1).ToString();
                    _scheduleService.UpdateSeats(flightId, newSeats);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool BookInternationalFlight(int_booking booking, string flightId)
        {
            try
            {
                _context.InternationalBookings.InsertOne(booking);

                var flight = _scheduleService.GetFlightById(flightId);
                if (flight != null)
                {
                    int currentSeats = int.Parse(flight.AvailableSeats);
                    string newSeats = (currentSeats - 1).ToString();
                    _scheduleService.UpdateSeats(flightId, newSeats);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public dom_booking SearchDomesticBooking(int pnr)
        {
            return _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
        }

        public int_booking SearchInternationalBooking(int pnr)
        {
            return _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
        }

        public decimal CancelDomesticBooking(int pnr)
        {
            var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (booking == null) return 0;

            decimal cancellationFee = booking.CalculateCancellationFee();
            decimal refundAmount = booking.TotalFare - cancellationFee;

            _context.DomesticBookings.DeleteOne(d => d.PNR == pnr);
            return refundAmount;
        }

        public decimal CancelInternationalBooking(int pnr)
        {
            var booking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
            if (booking == null) return 0;

            decimal cancellationFee = booking.CalculateCancellationFee();
            decimal refundAmount = booking.TotalFare - cancellationFee;

            _context.InternationalBookings.DeleteOne(i => i.PnrInt == pnr);
            return refundAmount;
        }

        public int GeneratePNR()
        {
            return _random.Next(100000, 999999);
        }
    }
}