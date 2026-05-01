using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class NotificationService
    {
        private readonly DatabaseContext _context;

        // ==== YOUR ACTUAL CREDENTIALS ====
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string senderEmail = "vishahameed666@gmail.com";     // Your Gmail
        private readonly string senderPassword = "gnnh rayx hwhk ctne";        // Your App Password
        private readonly string senderName = "Sky-Link Airlines";

        public NotificationService(DatabaseContext context)
        {
            _context = context;
        }

        // Main email sending method
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // HTML body support
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                // Log successful email
                LogNotification(toEmail, subject, body, true);
                Console.WriteLine($"✓ Email sent successfully to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                // Log failed email
                LogNotification(toEmail, subject, body, false, ex.Message);
                Console.WriteLine($"✗ Failed to send email to {toEmail}: {ex.Message}");
                return false;
            }
        }

        // Synchronous wrapper (for compatibility with existing code)
        public void SendEmail(string to, string subject, string body)
        {
            SendEmailAsync(to, subject, body).GetAwaiter().GetResult();
        }

        // Booking confirmation email
        public void SendBookingConfirmation(string email, string passengerName, int pnr,
            string flightNumber, string route, DateTime date, decimal totalFare)
        {
            var subject = "Booking Confirmation - Sky-Link Airlines";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #003366; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .footer {{ background-color: #333; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    th, td {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
                    .total {{ font-size: 18px; font-weight: bold; color: #003366; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Sky-Link Airlines</h1>
                        <h2>Booking Confirmation</h2>
                    </div>
                    <div class='content'>
                        <p>Dear <strong>{passengerName}</strong>,</p>
                        <p>Thank you for choosing Sky-Link Airlines! Your booking has been confirmed successfully.</p>
                        
                        <h3>Booking Details:</h3>
                        <table border='1' cellpadding='5' style='border-collapse: collapse; width: 100%;'>
                            <tr style='background-color: #f2f2f2;'><th>PNR Number:</th><td><strong>{pnr}</strong></td></tr>
                            <tr><th>Flight Number:</th><td>{flightNumber}</td></tr>
                            <tr style='background-color: #f2f2f2;'><th>Route:</th><td>{route}</td></tr>
                            <tr><th>Travel Date:</th><td>{date:dddd, MMMM dd, yyyy}</td></tr>
                            <tr style='background-color: #f2f2f2;'><th>Total Fare:</th><td class='total'>${totalFare:F2}</td></tr>
                        </table>
                        
                        <p><strong>Important Information:</strong></p>
                        <ul>
                            <li>Please arrive at the airport 2 hours before departure</li>
                            <li>Carry a valid ID proof for verification</li>
                            <li>Web check-in is available 48 hours before departure</li>
                        </ul>
                        
                        <p>You can check your booking status anytime using your PNR number.</p>
                        <p><strong>Safe journey!</strong></p>
                    </div>
                    <div class='footer'>
                        <p>Sky-Link Airlines | 24/7 Customer Support: +92-21-111-000-786</p>
                        <p>Email: support@skylinkairlines.com</p>
                        <p>&copy; 2024 Sky-Link Airlines. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            SendEmail(email, subject, body);
        }

        // Boarding pass email
        public void SendBoardingPass(string email, string passengerName, int pnr,
            string flightNumber, string seatNumber, string gate, string departureTime, string qrCodeUrl = null)
        {
            var subject = "Your Boarding Pass - Sky-Link Airlines";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #003366; color: white; padding: 20px; text-align: center; }}
                    .boarding-pass {{ border: 2px dashed #003366; padding: 20px; margin: 20px 0; border-radius: 10px; }}
                    .footer {{ background-color: #333; color: white; padding: 10px; text-align: center; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Sky-Link Airlines</h1>
                        <h2>Your Boarding Pass</h2>
                    </div>
                    <div class='boarding-pass'>
                        <p><strong>Passenger:</strong> {passengerName}</p>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Flight:</strong> {flightNumber}</p>
                        <p><strong>Seat:</strong> {seatNumber}</p>
                        <p><strong>Gate:</strong> {gate}</p>
                        <p><strong>Boarding Time:</strong> {departureTime}</p>
                    </div>
                    <p>Please have this boarding pass ready at the gate.</p>
                    <p><strong>Happy journey!</strong></p>
                    <div class='footer'>
                        <p>Sky-Link Airlines | Safe Travels</p>
                    </div>
                </div>
            </body>
            </html>";

            SendEmail(email, subject, body);
        }

        // Flight delay notification
        public void SendFlightDelayAlert(string email, string phone, string flightNumber,
            int delayMinutes, string reason)
        {
            var subject = "Flight Delay Alert - Sky-Link Airlines";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .alert {{ background-color: #ff9800; color: white; padding: 20px; text-align: center; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='alert'>
                        <h2>Flight Delay Notification</h2>
                    </div>
                    <p>Dear Passenger,</p>
                    <p>Flight <strong>{flightNumber}</strong> has been delayed by <strong>{delayMinutes} minutes</strong>.</p>
                    <p><strong>Reason:</strong> {reason}</p>
                    <p>We apologize for the inconvenience caused.</p>
                    <p>Please check the display boards for updated departure time.</p>
                    <p>For assistance, contact our support team.</p>
                </div>
            </body>
            </html>";

            SendEmail(email, subject, body);

            // Send SMS as well
            SendSMS(phone, $"Flight {flightNumber} delayed by {delayMinutes} minutes. Reason: {reason}");
        }

        // Password reset email
        public void SendPasswordResetOTP(string email, string otp)
        {
            var subject = "Password Reset OTP - Sky-Link Airlines";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .otp {{ font-size: 24px; font-weight: bold; color: #003366; padding: 10px; background-color: #f0f0f0; display: inline-block; }}
                </style>
            </head>
            <body>
                <h2>Password Reset Request</h2>
                <p>We received a request to reset your password.</p>
                <p>Your OTP for password reset is: <span class='otp'>{otp}</span></p>
                <p>This OTP is valid for <strong>10 minutes</strong>.</p>
                <p>If you didn't request this, please ignore this email.</p>
                <hr>
                <p>Sky-Link Airlines Team</p>
            </body>
            </html>";

            SendEmail(email, subject, body);
        }

        // Welcome email for new registration
        public void SendWelcomeEmail(string email, string fullName, string username)
        {
            var subject = "Welcome to Sky-Link Airlines!";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    .header {{ background-color: #003366; color: white; padding: 20px; text-align: center; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Welcome to Sky-Link Airlines!</h1>
                </div>
                <p>Dear <strong>{fullName}</strong>,</p>
                <p>Thank you for registering with Sky-Link Airlines!</p>
                <p>Your username: <strong>{username}</strong></p>
                <p>You can now:</p>
                <ul>
                    <li>Book domestic and international flights</li>
                    <li>Earn loyalty miles on every booking</li>
                    <li>Enjoy exclusive member benefits</li>
                    <li>Access web check-in 48 hours before departure</li>
                </ul>
                <p>Start your journey with us today!</p>
                <p><strong>Happy travels!</strong></p>
                <hr>
                <p>Sky-Link Airlines Team</p>
            </body>
            </html>";

            SendEmail(email, subject, body);
        }

        // Cancel booking confirmation email
        public void SendCancellationConfirmation(string email, string passengerName, int pnr, decimal refundAmount)
        {
            var subject = "Booking Cancellation Confirmation - Sky-Link Airlines";
            var body = $@"
            <!DOCTYPE html>
            <html>
            <body>
                <h2>Booking Cancellation Confirmation</h2>
                <p>Dear {passengerName},</p>
                <p>Your booking with PNR <strong>{pnr}</strong> has been cancelled successfully.</p>
                <p><strong>Refund Amount:</strong> ${refundAmount:F2}</p>
                <p>The refund will be processed within 7-10 business days.</p>
                <p>We hope to serve you again soon!</p>
                <p>Sky-Link Airlines Team</p>
            </body>
            </html>";

            SendEmail(email, subject, body);
        }

        // SMS sending (for phone notifications)
        public void SendSMS(string phoneNumber, string message)
        {
            try
            {
                // For SMS, you can use services like Twilio, Fast2SMS, etc.
                // This is a placeholder - implement based on your SMS provider
                Console.WriteLine($"📱 SMS to {phoneNumber}: {message}");
                LogNotification(phoneNumber, "SMS", message, true);
            }
            catch (Exception ex)
            {
                LogNotification(phoneNumber, "SMS", message, false, ex.Message);
            }
        }

        // Log all notifications to database
        private void LogNotification(string recipient, string subject, string message, bool isDelivered, string error = null)
        {
            try
            {
                var log = new NotificationLog
                {
                    Recipient = recipient,
                    Type = subject.Contains("SMS") ? "SMS" : "Email",
                    Subject = subject.Length > 100 ? subject.Substring(0, 100) : subject,
                    Message = message.Length > 500 ? message.Substring(0, 500) : message,
                    SentAt = DateTime.Now,
                    IsDelivered = isDelivered,
                    ErrorMessage = error
                };
                _context.NotificationLogs.InsertOne(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log notification: {ex.Message}");
            }
        }

        // Check email configuration status
        public bool IsEmailConfigured()
        {
            return !string.IsNullOrEmpty(senderEmail) &&
                   !string.IsNullOrEmpty(senderPassword) &&
                   senderEmail != "your-email@gmail.com";
        }

        // Test method to verify email configuration
        public async Task<bool> TestEmailConfiguration(string testEmail)
        {
            var subject = "Test Email - Sky-Link Airlines Configuration";
            var body = @"
            <!DOCTYPE html>
            <html>
            <body>
                <h1>✅ Email Configuration Test Successful!</h1>
                <p>Your Sky-Link Airlines notification system is working correctly.</p>
                <p>You will now receive:</p>
                <ul>
                    <li>Booking confirmations</li>
                    <li>Boarding passes</li>
                    <li>Flight delay alerts</li>
                    <li>Password reset OTPs</li>
                </ul>
                <p><strong>Test Details:</strong></p>
                <ul>
                    <li>Time: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</li>
                    <li>System: Sky-Link Airlines v3.0</li>
                </ul>
                <p>Happy flying! ✈️</p>
                <hr>
                <p><small>This is an automated test message.</small></p>
            </body>
            </html>";

            return await SendEmailAsync(testEmail, subject, body);
        }
    }
}