using System;
using System.Linq;
using MongoDB.Driver;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class PaymentService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;
        private readonly LoyaltyService _loyaltyService;
        private static Random _random = new Random();

        public PaymentService(DatabaseContext context, NotificationService notificationService, LoyaltyService loyaltyService)
        {
            _context = context;
            _notificationService = notificationService;
            _loyaltyService = loyaltyService;
        }

        public PaymentTransaction ProcessPayment(int pnr, decimal amount, string paymentMethod, string cardNumber = null, string expiryDate = null, string cvv = null)
        {
            var transaction = new PaymentTransaction
            {
                TransactionId = GenerateTransactionId(),
                PNR = pnr,
                Amount = amount,
                Currency = "USD",
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                TransactionDate = DateTime.Now,
                CardLast4 = "",
                CardType = "",
                AuthorizationCode = "",
                InstallmentPlan = "None",
                SavedCardToken = ""
            };

            // Simulate payment processing
            bool paymentSuccess = SimulatePaymentProcessing();

            if (paymentSuccess)
            {
                transaction.PaymentStatus = "Completed";
                if (!string.IsNullOrEmpty(cardNumber) && cardNumber.Length >= 4)
                {
                    transaction.CardLast4 = cardNumber.Substring(cardNumber.Length - 4);
                }
                transaction.AuthorizationCode = GenerateAuthCode();

                // Add loyalty miles
                _loyaltyService.AddMiles(pnr, (int)(amount * 5));
            }
            else
            {
                transaction.PaymentStatus = "Failed";
            }

            _context.PaymentTransactions.InsertOne(transaction);
            return transaction;
        }

        public PaymentTransaction ProcessInstallmentPayment(int pnr, decimal totalAmount, string installmentPlan)
        {
            int months = installmentPlan == "3Months" ? 3 : 6;
            decimal installmentAmount = totalAmount / months;

            var transaction = new PaymentTransaction
            {
                TransactionId = GenerateTransactionId(),
                PNR = pnr,
                Amount = installmentAmount,
                Currency = "USD",
                PaymentMethod = "CreditCard",
                PaymentStatus = "Completed",
                TransactionDate = DateTime.Now,
                InstallmentPlan = installmentPlan,
                InstallmentAmount = installmentAmount,
                InstallmentsLeft = months - 1,
                IsRecurring = true,
                CardLast4 = "",
                CardType = "",
                AuthorizationCode = GenerateAuthCode(),
                SavedCardToken = ""
            };

            _context.PaymentTransactions.InsertOne(transaction);
            return transaction;
        }

        public bool RefundPayment(string transactionId, decimal amount, string reason)
        {
            var transaction = _context.PaymentTransactions.Find(t => t.TransactionId == transactionId).FirstOrDefault();
            if (transaction == null || transaction.PaymentStatus != "Completed")
                return false;

            transaction.PaymentStatus = "Refunded";
            _context.PaymentTransactions.ReplaceOne(t => t.TransactionId == transactionId, transaction);

            return true;
        }

        public string GenerateTransactionId()
        {
            return "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss") + _random.Next(1000, 9999);
        }

        private string GenerateAuthCode()
        {
            return _random.Next(100000, 999999).ToString();
        }

        private bool SimulatePaymentProcessing()
        {
            // Simulate 95% success rate
            return _random.Next(1, 100) <= 95;
        }

        public PaymentTransaction GetTransactionByPNR(int pnr)
        {
            return _context.PaymentTransactions.Find(t => t.PNR == pnr).FirstOrDefault();
        }
    }
}