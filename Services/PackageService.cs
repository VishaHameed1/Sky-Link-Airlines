using System;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;
using MongoDB.Driver;

namespace VISHA_HAMEED.Services
{
    public class PackageService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;

        public PackageService(DatabaseContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public TravelPackage AddPackage(string destination, string hotelName, int nights, decimal price, string flightId, List<string> inclusions)
        {
            var package = new TravelPackage
            {
                PackageId = GeneratePackageId(),
                Destination = destination,
                HotelName = hotelName,
                Nights = nights,
                PackagePrice = price,
                FlightId = flightId,
                Inclusions = inclusions,
                Exclusions = new List<string>(),
                MealPlan = "Breakfast Included",
                RoomType = "Standard",
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddMonths(6),
                IsActive = true
            };

            _context.TravelPackages.InsertOne(package);
            return package;
        }

        public List<TravelPackage> SearchPackages(string destination, int nights = 0, decimal maxPrice = 0)
        {
            var query = _context.TravelPackages.Find(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(destination))
                query = _context.TravelPackages.Find(p => p.Destination.Contains(destination) && p.IsActive == true);

            var packages = query.ToList();

            if (nights > 0)
                packages = packages.Where(p => p.Nights == nights).ToList();

            if (maxPrice > 0)
                packages = packages.Where(p => p.PackagePrice <= maxPrice).ToList();

            return packages;
        }

        public bool BookPackage(string packageId, int pnr, string customerEmail, string customerName)
        {
            var package = _context.TravelPackages.Find(p => p.PackageId == packageId).FirstOrDefault();
            if (package == null) return false;

            _notificationService.SendEmail(customerEmail, "Travel Package Booked!",
                $"Dear {customerName},\n\nYou have successfully booked the {package.Destination} package!\n\nPackage Details:\nHotel: {package.HotelName}\nNights: {package.Nights}\nMeal Plan: {package.MealPlan}\nTotal Price: ${package.PackagePrice}\n\nWe will send you the hotel voucher shortly.\n\nHappy travels!\nSky-Link Airlines");

            return true;
        }

        private string GeneratePackageId()
        {
            return "PKG" + DateTime.Now.ToString("yyyyMMdd") + new Random().Next(100, 999);
        }
    }
}