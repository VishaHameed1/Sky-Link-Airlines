using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using AirlineReservationConsole;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class LoyaltyService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;

        public LoyaltyService(DatabaseContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public LoyaltyProgram GetOrCreateLoyaltyAccount(string email, string customerId)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var account = _context.LoyaltyPrograms.Find(l => l.Email == email).FirstOrDefault();

            if (account == null)
            {
                account = new LoyaltyProgram
                {
                    CustomerId = customerId ?? email,
                    Email = email,
                    Tier = "Bronze",
                    TotalMiles = 0,
                    AvailableMiles = 0,
                    MilesToNextTier = 5000,
                    Transactions = new List<MileTransaction>(),
                    TierValidUntil = DateTime.Now.AddYears(1),
                    LastActivity = DateTime.Now,
                    LifetimeMiles = 0,
                    TotalFlights = 0,
                    TotalSpent = 0
                };
                _context.LoyaltyPrograms.InsertOne(account);
            }

            return account;
        }

        public void AddMiles(int pnr, int miles)
        {
            // Find the booking
            var domesticBooking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            var internationalBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();

            if (domesticBooking == null && internationalBooking == null) return;

            string email = domesticBooking?.Email ?? internationalBooking?.Email;
            string passengerName = domesticBooking?.PassengerName ?? internationalBooking?.PassengerName;
            double price = domesticBooking?.Price ?? internationalBooking?.Price ?? 0;

            if (string.IsNullOrEmpty(email)) return;

            var loyalty = GetOrCreateLoyaltyAccount(email, passengerName);
            if (loyalty == null) return;

            loyalty.TotalMiles += miles;
            loyalty.AvailableMiles += miles;
            loyalty.LifetimeMiles += miles;
            loyalty.TotalFlights++;
            loyalty.TotalSpent += (decimal)price;
            loyalty.LastActivity = DateTime.Now;

            if (loyalty.Transactions == null)
                loyalty.Transactions = new List<MileTransaction>();

            loyalty.Transactions.Add(new MileTransaction
            {
                Date = DateTime.Now,
                Miles = miles,
                Type = "Earned",
                Description = $"Flight booking PNR: {pnr}",
                ReferenceId = pnr.ToString()
            });

            // Check for tier upgrade
            var oldTier = loyalty.Tier;
            string newTier = GetTierByMiles(loyalty.LifetimeMiles);

            if (newTier != oldTier)
            {
                loyalty.Tier = newTier;
                _notificationService?.SendEmail(email, "Loyalty Tier Upgrade!",
                    $"Dear {passengerName},\n\nCongratulations! You've been upgraded to {newTier} tier!\n\n" +
                    $"Benefits:\n{GetTierBenefits(newTier)}\n\n" +
                    $"Thank you for flying with Sky-Link Airlines!");
            }

            loyalty.MilesToNextTier = GetMilesToNextTier(loyalty.Tier, loyalty.LifetimeMiles);

            _context.LoyaltyPrograms.ReplaceOne(l => l.Email == email, loyalty);
        }

        private string GetTierByMiles(int lifetimeMiles)
        {
            if (lifetimeMiles >= 50000) return "Platinum";
            if (lifetimeMiles >= 25000) return "Gold";
            if (lifetimeMiles >= 10000) return "Silver";
            return "Bronze";
        }

        public bool RedeemMiles(string email, int miles, string description)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var loyalty = _context.LoyaltyPrograms.Find(l => l.Email == email).FirstOrDefault();
            if (loyalty == null || loyalty.AvailableMiles < miles || miles < 5000)
                return false;

            loyalty.AvailableMiles -= miles;

            if (loyalty.Transactions == null)
                loyalty.Transactions = new List<MileTransaction>();

            loyalty.Transactions.Add(new MileTransaction
            {
                Date = DateTime.Now,
                Miles = miles,
                Type = "Redeemed",
                Description = description,
                ReferenceId = DateTime.Now.Ticks.ToString()
            });

            loyalty.LastActivity = DateTime.Now;
            _context.LoyaltyPrograms.ReplaceOne(l => l.Email == email, loyalty);

            // Send notification
            _notificationService?.SendEmail(email, "Miles Redeemed",
                $"You have successfully redeemed {miles} miles.\n{description}");

            return true;
        }

        public int GetAvailableMiles(string email)
        {
            if (string.IsNullOrEmpty(email)) return 0;

            var loyalty = _context.LoyaltyPrograms.Find(l => l.Email == email).FirstOrDefault();
            return loyalty?.AvailableMiles ?? 0;
        }

        private int GetMilesToNextTier(string currentTier, int lifetimeMiles)
        {
            switch (currentTier)
            {
                case "Bronze": return Math.Max(0, 10000 - lifetimeMiles);
                case "Silver": return Math.Max(0, 25000 - lifetimeMiles);
                case "Gold": return Math.Max(0, 50000 - lifetimeMiles);
                default: return 0;
            }
        }

        public string GetTierBenefits(string tier)
        {
            switch (tier)
            {
                case "Silver":
                    return "• Priority check-in counter\n• 5kg extra baggage allowance\n• 10% discount on lounge access\n• Dedicated customer support";
                case "Gold":
                    return "• Priority check-in and security\n• 10kg extra baggage allowance\n• Free lounge access worldwide\n• Priority boarding\n• 15% bonus miles on flights\n• Free seat selection";
                case "Platinum":
                    return "• All Gold benefits\n• Companion ticket annually\n• Free upgrades when available\n• 24/7 dedicated concierge\n• 25% bonus miles on flights\n• Free extra legroom seats\n• Priority baggage handling";
                default:
                    return "• Standard check-in\n• Standard baggage allowance (15kg)\n• Earn 5 miles per $1 spent\n• Web check-in available";
            }
        }

        public LoyaltyProgram GetLoyaltyDetails(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return _context.LoyaltyPrograms.Find(l => l.Email == email).FirstOrDefault();
        }

        public List<LoyaltyProgram> GetTopMembers(int count = 10)
        {
            return _context.LoyaltyPrograms.Find(_ => true)
                .SortByDescending(l => l.TotalMiles)
                .Limit(count)
                .ToList();
        }

        public decimal CalculateMileDiscount(int miles, decimal bookingAmount)
        {
            // 100 miles = $1 discount (100 miles = Rs. 100 discount)
            decimal discount = (miles / 100) * 1m;
            return Math.Min(discount, bookingAmount * 0.3m); // Max 30% discount
        }
    }
}