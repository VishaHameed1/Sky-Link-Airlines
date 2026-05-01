using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VISHA_HAMEED.Models
{
    public class PaymentTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TransactionId { get; set; }
        public int PNR { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; } // CreditCard, PayPal, EasyPaisa, JazzCash, BankTransfer
        public string CardLast4 { get; set; }
        public string CardType { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed, Refunded
        public DateTime TransactionDate { get; set; }
        public string AuthorizationCode { get; set; }
        public string InstallmentPlan { get; set; } // None, 3Months, 6Months
        public decimal InstallmentAmount { get; set; }
        public int InstallmentsLeft { get; set; }
        public string SavedCardToken { get; set; }
        public bool IsRecurring { get; set; }
    }
}