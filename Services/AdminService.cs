using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using AirlineReservationConsole;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class AdminService
    {
        private readonly DatabaseContext _context;

        public AdminService(DatabaseContext context)
        {
            _context = context;
        }

        public List<dom_booking> GetAllDomesticBookings()
        {
            return _context.DomesticBookings.Find(_ => true).ToList();
        }

        public List<int_booking> GetAllInternationalBookings()
        {
            return _context.InternationalBookings.Find(_ => true).ToList();
        }

        public List<CancelledBooking> GetAllCancelledBookings()
        {
            return _context.CancelledBookings.Find(_ => true).ToList();
        }

        public bool UpdateDomesticBooking(int pnr, string newName, string newContact, string newEmail)
        {
            var update = Builders<dom_booking>.Update
                .Set(d => d.PassengerName, newName)
                .Set(d => d.ContactNo, newContact)
                .Set(d => d.Email, newEmail);

            var result = _context.DomesticBookings.UpdateOne(d => d.PNR == pnr, update);
            return result.ModifiedCount > 0;
        }

        public bool UpdateInternationalBooking(int pnr, string newName, string newContact, string newEmail)
        {
            var update = Builders<int_booking>.Update
                .Set(i => i.PassengerName, newName)
                .Set(i => i.ContactNo, newContact)
                .Set(i => i.Email, newEmail);

            var result = _context.InternationalBookings.UpdateOne(i => i.PnrInt == pnr, update);
            return result.ModifiedCount > 0;
        }

        public bool DeleteDomesticBooking(int pnr, string reason)
        {
            var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (booking == null) return false;

            var cancelled = new CancelledBooking
            {
                PNR = booking.PNR,
                PassengerName = booking.PassengerName,
                BookingType = "Domestic",
                CancelledDate = DateTime.Now,
                RefundAmount = 0,
                Reason = reason
            };

            _context.CancelledBookings.InsertOne(cancelled);
            _context.DomesticBookings.DeleteOne(d => d.PNR == pnr);
            return true;
        }

        public bool DeleteInternationalBooking(int pnr, string reason)
        {
            var booking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
            if (booking == null) return false;

            var cancelled = new CancelledBooking
            {
                PNR = booking.PnrInt,
                PassengerName = booking.PassengerName,
                BookingType = "International",
                CancelledDate = DateTime.Now,
                RefundAmount = 0,
                Reason = reason
            };

            _context.CancelledBookings.InsertOne(cancelled);
            _context.InternationalBookings.DeleteOne(i => i.PnrInt == pnr);
            return true;
        }

        public decimal GetTotalRevenue()
        {
            var domesticTotal = _context.DomesticBookings.Find(_ => true).ToList().Sum(d => d.TotalFare);
            var internationalTotal = _context.InternationalBookings.Find(_ => true).ToList().Sum(i => i.TotalFare);
            return domesticTotal + internationalTotal;
        }

        public decimal GetTotalRefunds()
        {
            return _context.CancelledBookings.Find(_ => true).ToList().Sum(c => c.RefundAmount);
        }
    }
}