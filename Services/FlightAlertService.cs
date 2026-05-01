using MongoDB.Driver;
using System;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class FlightAlertService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;
        private readonly PromotionService _promotionService;

        public FlightAlertService(DatabaseContext context, NotificationService notificationService, PromotionService promotionService)
        {
            _context = context;
            _notificationService = notificationService;
            _promotionService = promotionService;
        }

        public FlightAlert CreateAlert(string email, string phone, string route, decimal targetPrice, string preferredClass = "Economy")
        {
            var alert = new FlightAlert
            {
                AlertId = GenerateAlertId(),
                Email = email,
                Phone = phone,
                Route = route,
                TargetPrice = targetPrice,
                CreatedAt = DateTime.Now,
                IsActive = true,
                PreferredClass = preferredClass,
                Frequency = "Real-time"
            };

            _context.FlightAlerts.InsertOne(alert);
            return alert;
        }

        public void CheckAndNotifyAlerts()
        {
            var activeAlerts = _context.FlightAlerts.Find(a => a.IsActive == true).ToList();
            var currentFlights = _context.FlightSchedules.Find(_ => true).ToList();

            foreach (var alert in activeAlerts)
            {
                var matchingFlight = currentFlights.FirstOrDefault(f => f.Route.Contains(alert.Route));

                if (matchingFlight != null && matchingFlight.Price <= alert.TargetPrice)
                {
                    // Send notification
                    if (!string.IsNullOrEmpty(alert.Email))
                    {
                        _notificationService.SendEmail(alert.Email, "Price Drop Alert!",
                            $"Good news! The price for {alert.Route} flight has dropped to ${matchingFlight.Price}!");
                    }

                    if (!string.IsNullOrEmpty(alert.Phone))
                    {
                        _notificationService.SendSMS(alert.Phone, $"Price alert: {alert.Route} flight now ${matchingFlight.Price}!");
                    }

                    alert.LastNotified = DateTime.Now;
                    _context.FlightAlerts.ReplaceOne(a => a.AlertId == alert.AlertId, alert);
                }
            }
        }

        public bool DeactivateAlert(string alertId)
        {
            var alert = _context.FlightAlerts.Find(a => a.AlertId == alertId).FirstOrDefault();
            if (alert == null) return false;

            alert.IsActive = false;
            _context.FlightAlerts.ReplaceOne(a => a.AlertId == alertId, alert);
            return true;
        }

        private string GenerateAlertId()
        {
            return "ALT" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }
    }
}