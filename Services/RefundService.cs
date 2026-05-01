using MongoDB.Driver;
using System;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class RefundService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;
        private readonly PaymentService _paymentService;

        public RefundService(DatabaseContext context, NotificationService notificationService, PaymentService paymentService)
        {
            _context = context;
            _notificationService = notificationService;
            _paymentService = paymentService;
        }

        public RefundRequest SubmitRefundRequest(int pnr, string reason, decimal requestedAmount)
        {
            var request = new RefundRequest
            {
                RequestId = GenerateRequestId(),
                PNR = pnr,
                RequestDate = DateTime.Now,
                Reason = reason,
                Status = "Pending",
                RequestedAmount = requestedAmount
            };

            _context.RefundRequests.InsertOne(request);

            var passenger = GetPassengerByPNR(pnr);
            if (passenger != null)
            {
                _notificationService.SendEmail(passenger.Email, "Refund Request Submitted",
                    $"Dear {passenger.PassengerName},\n\nYour refund request has been submitted.\nRequest ID: {request.RequestId}\nAmount: ${requestedAmount}\n\nWe will process your request within 7-10 business days.");
            }

            return request;
        }

        public bool ProcessRefund(string requestId, bool approve, string processedBy, string rejectionReason = null)
        {
            var request = _context.RefundRequests.Find(r => r.RequestId == requestId).FirstOrDefault();
            if (request == null) return false;

            if (approve)
            {
                request.Status = "Approved";
                request.ApprovedAmount = request.RequestedAmount;

                // Process refund payment
                var transaction = _paymentService.GetTransactionByPNR(request.PNR);
                if (transaction != null)
                {
                    _paymentService.RefundPayment(transaction.TransactionId, request.ApprovedAmount, request.Reason);
                }
            }
            else
            {
                request.Status = "Rejected";
                request.RejectionReason = rejectionReason;
            }

            request.ProcessedDate = DateTime.Now;
            request.ProcessedBy = processedBy;

            _context.RefundRequests.ReplaceOne(r => r.RequestId == requestId, request);

            var passenger = GetPassengerByPNR(request.PNR);
            if (passenger != null)
            {
                var status = approve ? "approved" : "rejected";
                _notificationService.SendEmail(passenger.Email, $"Refund Request {status.ToUpper()}",
                    approve ?
                    $"Dear {passenger.PassengerName},\n\nYour refund request has been approved.\nRefund Amount: ${request.ApprovedAmount}\nPlease allow 5-7 business days for the refund to reflect." :
                    $"Dear {passenger.PassengerName},\n\nYour refund request has been rejected.\nReason: {rejectionReason}");
            }

            return true;
        }

        private string GenerateRequestId()
        {
            return "REF" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }

        private dynamic GetPassengerByPNR(int pnr)
        {
            var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (booking != null) return new { booking.PassengerName, booking.Email };

            var intBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
            if (intBooking != null) return new { intBooking.PassengerName, intBooking.Email };

            return null;
        }

        public RefundRequest GetRefundRequest(string requestId)
        {
            return _context.RefundRequests.Find(r => r.RequestId == requestId).FirstOrDefault();
        }
    }
}