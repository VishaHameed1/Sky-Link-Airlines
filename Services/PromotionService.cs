using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class PromotionService
    {
        private readonly DatabaseContext _context;

        public PromotionService(DatabaseContext context)
        {
            _context = context;
        }

        public List<Promotion> GetAllPromotions()
        {
            return _context.Promotions.Find(_ => true).ToList();
        }

        public Promotion GetPromotionByCode(string code)
        {
            return _context.Promotions.Find(p => p.PromoCode == code && p.ValidUntil >= DateTime.Now).FirstOrDefault();
        }

        public bool AddPromotion(Promotion promotion)
        {
            try
            {
                _context.Promotions.InsertOne(promotion);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePromotion(string code, int newDiscount, DateTime newValidUntil)
        {
            var update = Builders<Promotion>.Update
                .Set(p => p.DiscountPercentage, newDiscount)
                .Set(p => p.ValidUntil, newValidUntil);

            var result = _context.Promotions.UpdateOne(p => p.PromoCode == code, update);
            return result.ModifiedCount > 0;
        }

        public bool DeletePromotion(string code)
        {
            var result = _context.Promotions.DeleteOne(p => p.PromoCode == code);
            return result.DeletedCount > 0;
        }

        public decimal ApplyDiscount(decimal price, string promoCode)
        {
            var promotion = GetPromotionByCode(promoCode);
            if (promotion != null)
            {
                decimal discount = price * promotion.DiscountPercentage / 100;
                return price - discount;
            }
            return price;
        }

        public void SeedInitialPromotions()
        {
            if (_context.Promotions.CountDocuments(_ => true) == 0)
            {
                var promotions = new List<Promotion>
                {
                    new Promotion { Title = "Summer Sale", Description = "Get flat 20% off on all domestic flights", PromoCode = "SUMMER20", DiscountPercentage = 20, ValidUntil = DateTime.Now.AddMonths(1) },
                    new Promotion { Title = "International Offer", Description = "15% off on international bookings", PromoCode = "INTL15", DiscountPercentage = 15, ValidUntil = DateTime.Now.AddMonths(2) },
                    new Promotion { Title = "First Class Upgrade", Description = "50% off on business class upgrade", PromoCode = "FIRST50", DiscountPercentage = 50, ValidUntil = DateTime.Now.AddMonths(1) }
                };
                _context.Promotions.InsertMany(promotions);
            }
        }
    }
}