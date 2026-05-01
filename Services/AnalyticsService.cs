using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;

namespace VISHA_HAMEED.Services
{
    public class AnalyticsService
    {
        private readonly DatabaseContext _context;

        public AnalyticsService(DatabaseContext context)
        {
            _context = context;
        }

        public Dictionary<string, object> GetDashboardMetrics()
        {
            var domesticCount = _context.DomesticBookings.CountDocuments(_ => true);
            var internationalCount = _context.InternationalBookings.CountDocuments(_ => true);
            var cancelledCount = _context.CancelledBookings.CountDocuments(_ => true);

            var domesticRevenue = _context.DomesticBookings.Find(_ => true).ToList().Sum(d => d.TotalFare);
            var internationalRevenue = _context.InternationalBookings.Find(_ => true).ToList().Sum(i => i.TotalFare);
            var refunds = _context.CancelledBookings.Find(_ => true).ToList().Sum(c => c.RefundAmount);

            return new Dictionary<string, object>
            {
                { "TotalBookings", domesticCount + internationalCount },
                { "DomesticBookings", domesticCount },
                { "InternationalBookings", internationalCount },
                { "CancelledBookings", cancelledCount },
                { "TotalRevenue", domesticRevenue + internationalRevenue },
                { "NetRevenue", domesticRevenue + internationalRevenue - refunds },
                { "TotalRefunds", refunds }
            };
        }

        public List<RevenueReport> GetRevenueByRoute(DateTime startDate, DateTime endDate)
        {
            var report = new List<RevenueReport>();

            var domesticBookings = _context.DomesticBookings.Find(_ => true).ToList()
                .Where(d => DateTime.Parse(d.TravelDate) >= startDate && DateTime.Parse(d.TravelDate) <= endDate);

            var grouped = domesticBookings.GroupBy(d => $"{d.Source}->{d.Destination}");

            foreach (var group in grouped)
            {
                report.Add(new RevenueReport
                {
                    Route = group.Key,
                    Bookings = group.Count(),
                    Revenue = group.Sum(b => b.TotalFare)
                });
            }

            return report.OrderByDescending(r => r.Revenue).ToList();
        }

        public List<OccupancyForecast> GetOccupancyForecast(DateTime startDate, int days)
        {
            var forecast = new List<OccupancyForecast>();
            var flights = _context.FlightSchedules.Find(_ => true).ToList();

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                foreach (var flight in flights)
                {
                    int totalSeats = 150;
                    int availableSeats = int.Parse(flight.AvailableSeats);
                    int bookedSeats = totalSeats - availableSeats;
                    double occupancyRate = (double)bookedSeats / totalSeats * 100;

                    // Simple forecasting based on historical patterns
                    double forecastRate = occupancyRate;
                    if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Sunday)
                        forecastRate += 15;
                    if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Wednesday)
                        forecastRate -= 10;

                    forecast.Add(new OccupancyForecast
                    {
                        Date = date,
                        FlightId = flight.FlightID,
                        CurrentOccupancy = occupancyRate,
                        ForecastOccupancy = Math.Min(forecastRate, 100)
                    });
                }
            }

            return forecast;
        }

        public List<CustomerInsight> GetCustomerBehavior()
        {
            var insights = new List<CustomerInsight>();

            var domesticCustomers = _context.DomesticBookings.Find(_ => true).ToList()
                .GroupBy(d => d.Email);

            foreach (var customer in domesticCustomers)
            {
                insights.Add(new CustomerInsight
                {
                    Email = customer.Key,
                    TotalBookings = customer.Count(),
                    TotalSpent = customer.Sum(c => c.TotalFare),
                    PreferredRoute = customer.GroupBy(c => $"{c.Source}->{c.Destination}")
                        .OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "N/A",
                    LastBooking = customer.Max(c => DateTime.Parse(c.TravelDate))
                });
            }

            return insights.OrderByDescending(i => i.TotalSpent).Take(100).ToList();
        }
    }

    public class RevenueReport
    {
        public string Route { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OccupancyForecast
    {
        public DateTime Date { get; set; }
        public string FlightId { get; set; }
        public double CurrentOccupancy { get; set; }
        public double ForecastOccupancy { get; set; }
    }

    public class CustomerInsight
    {
        public string Email { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public string PreferredRoute { get; set; }
        public DateTime LastBooking { get; set; }
    }
}