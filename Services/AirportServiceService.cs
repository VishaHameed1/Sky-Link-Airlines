using MongoDB.Driver;
using System;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class AirportServiceService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;
        private readonly PaymentService _paymentService;

        public AirportServiceService(DatabaseContext context, NotificationService notificationService, PaymentService paymentService)
        {
            _context = context;
            _notificationService = notificationService;
            _paymentService = paymentService;
        }

        public AirportService BookService(int pnr, string serviceType, DateTime serviceDateTime, string specialInstructions = null)
        {
            decimal price = GetServicePrice(serviceType);

            var service = new AirportService
            {
                ServiceType = serviceType,
                BookingId = GenerateBookingId(),
                PNR = pnr,
                ServiceDateTime = serviceDateTime,
                BookingTime = DateTime.Now,
                Price = price,
                Status = "Pending",
                SpecialInstructions = specialInstructions
            };

            _context.AirportServices.InsertOne(service);

            var passenger = GetPassengerByPNR(pnr);
            if (passenger != null)
            {
                _notificationService.SendEmail(passenger.Email, "Airport Service Booked",
                    $"Dear {passenger.PassengerName},\n\nYour {serviceType} service has been booked successfully.\nBooking ID: {service.BookingId}\nService Date: {serviceDateTime:dd/MM/yyyy HH:mm}\nPrice: ${price}\n\nThank you!");
            }

            return service;
        }

        private decimal GetServicePrice(string serviceType)
        {
            switch (serviceType)
            {
                case "Lounge": return 50;
                case "Parking": return 20;
                case "MeetGreet": return 30;
                case "FastTrack": return 40;
                case "Wheelchair": return 0; // Free
                case "UnaccompaniedMinor": return 100;
                default: return 0;
            }
        }

        private string GenerateBookingId()
        {
            return "SVC" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }

        private dynamic GetPassengerByPNR(int pnr)
        {
            var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (booking != null) return new { booking.PassengerName, booking.Email };

            var intBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
            if (intBooking != null) return new { intBooking.PassengerName, intBooking.Email };

            return null;
        }

        public AirportService GetService(string bookingId)
        {
            return _context.AirportServices.Find(s => s.BookingId == bookingId).FirstOrDefault();
        }

        public bool CancelService(string bookingId)
        {
            var service = _context.AirportServices.Find(s => s.BookingId == bookingId).FirstOrDefault();
            if (service == null || service.Status == "Confirmed") return false;

            service.Status = "Cancelled";
            _context.AirportServices.ReplaceOne(s => s.BookingId == bookingId, service);
            return true;
        }
    }
}