using AirlineReservationConsole;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class ExportService
    {
        private readonly DatabaseContext _context;

        public ExportService(DatabaseContext context)
        {
            _context = context;
        }

        public string ExportToCSV(List<dom_booking> bookings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PNR,Passenger Name,Email,Flight Number,Source,Destination,Travel Date,Total Fare,Status");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"{booking.PNR},{booking.PassengerName},{booking.Email},{booking.FlightNumber},{booking.Source},{booking.Destination},{booking.TravelDate},{booking.TotalFare},{booking.Status}");
            }

            return sb.ToString();
        }

        public string ExportInternationalToCSV(List<int_booking> bookings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PNR,Passenger Name,Email,Flight Number,Source,Destination,Travel Date,Total Fare,Status,Passport Number,Nationality");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"{booking.PnrInt},{booking.PassengerName},{booking.Email},{booking.FlightNumber},{booking.Source},{booking.Destination},{booking.DateInt},{booking.TotalFare},{booking.Status},{booking.PassportNumber},{booking.Nationality}");
            }

            return sb.ToString();
        }

        public byte[] GenerateInvoice(int pnr)
        {
            // Check domestic booking first
            var domesticBooking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (domesticBooking != null)
            {
                return GenerateDomesticInvoice(domesticBooking);
            }

            // Check international booking
            var internationalBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
            if (internationalBooking != null)
            {
                return GenerateInternationalInvoice(internationalBooking);
            }

            return null;
        }

        private byte[] GenerateDomesticInvoice(dom_booking booking)
        {
            var invoice = $@"
            ================================================
                    SKY-LINK AIRLINES INVOICE
                         DOMESTIC FLIGHT
            ================================================
            
            Invoice Number: INV-{booking.PNR}-{DateTime.Now:yyyyMMdd}
            Date: {DateTime.Now:dd/MM/yyyy HH:mm}
            
            Passenger Details:
            -----------------
            Name: {booking.PassengerName}
            Email: {booking.Email}
            Contact: {booking.ContactNo}
            ID Proof: {booking.IDProofType} - {booking.IDProofNumber}
            
            Flight Details:
            ---------------
            Flight Number: {booking.FlightNumber}
            Route: {booking.Source} -> {booking.Destination}
            Date: {booking.TravelDate}
            Departure: {booking.DepartureTime}
            Arrival: {booking.ArrivalTime}
            Seat Class: {booking.SeatClass}
            
            Payment Details:
            ----------------
            Total Fare: ${booking.TotalFare:F2}
            Payment Mode: {booking.ModeOfPayment}
            
            PNR: {booking.PNR}
            
            ================================================
            Thank you for choosing Sky-Link Airlines!
            ================================================
            ";

            return Encoding.UTF8.GetBytes(invoice);
        }

        private byte[] GenerateInternationalInvoice(int_booking booking)
        {
            var invoice = $@"
            ================================================
                    SKY-LINK AIRLINES INVOICE
                     INTERNATIONAL FLIGHT
            ================================================
            
            Invoice Number: INV-{booking.PnrInt}-{DateTime.Now:yyyyMMdd}
            Date: {DateTime.Now:dd/MM/yyyy HH:mm}
            
            Passenger Details:
            -----------------
            Name: {booking.PassengerName}
            Email: {booking.Email}
            Contact: {booking.ContactNo}
            Passport: {booking.PassportNumber}
            Nationality: {booking.Nationality}
            Visa Type: {booking.VisaType ?? "N/A"}
            
            Flight Details:
            ---------------
            Flight Number: {booking.FlightNumber}
            Route: {booking.Source} -> {booking.Destination}
            Date: {booking.DateInt}
            Departure: {booking.IntDep}
            Arrival: {booking.IntArr}
            Seat Class: {booking.SeatClass}
            Airline Alliance: {booking.AirlineAlliance ?? "N/A"}
            
            Payment Details:
            ----------------
            Total Fare: ${booking.TotalFare:F2}
            Payment Mode: {booking.ModeOfPayment}
            
            PNR: {booking.PnrInt}
            
            ================================================
            Thank you for choosing Sky-Link Airlines!
            ================================================
            ";

            return Encoding.UTF8.GetBytes(invoice);
        }

        public void SaveToFile(string content, string fileName, string directory = "Exports")
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, content);
        }

        public void SaveBytesToFile(byte[] content, string fileName, string directory = "Exports")
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            File.WriteAllBytes(filePath, content);
        }

        public byte[] ExportToExcel(List<FlightSchedule> flights)
        {
            // In production, use EPPlus or ClosedXML library
            // For now, return CSV bytes
            var sb = new StringBuilder();
            sb.AppendLine("Flight ID,Type,Route,Departure,Arrival,Available Seats,Price,Gate,Terminal");

            foreach (var flight in flights)
            {
                sb.AppendLine($"{flight.FlightID},{flight.Type},{flight.Route},{flight.Boarding},{flight.Landing},{flight.AvailableSeats},{flight.Price},{flight.Gate},{flight.Terminal}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public string ExportCancelledBookingsToCSV(List<CancelledBooking> cancelled)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PNR,Passenger Name,Booking Type,Cancelled Date,Refund Amount,Reason");

            foreach (var c in cancelled)
            {
                sb.AppendLine($"{c.PNR},{c.PassengerName},{c.BookingType},{c.CancelledDate:dd/MM/yyyy HH:mm},{c.RefundAmount:F2},{c.Reason}");
            }

            return sb.ToString();
        }

        public string ExportFeedbackToCSV(List<Feedback> feedbacks)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Email,Rating,Comments,Feedback Date");

            foreach (var f in feedbacks)
            {
                sb.AppendLine($"{f.Email},{f.Rating},{f.Comments},{f.FeedbackDate:dd/MM/yyyy HH:mm}");
            }

            return sb.ToString();
        }

        public string ExportLoyaltyToCSV(List<LoyaltyProgram> members)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Email,Tier,Total Miles,Available Miles,Total Flights,Total Spent");

            foreach (var m in members)
            {
                sb.AppendLine($"{m.Email},{m.Tier},{m.TotalMiles},{m.AvailableMiles},{m.TotalFlights},{m.TotalSpent:F2}");
            }

            return sb.ToString();
        }

        public byte[] GenerateCombinedReport(DateTime startDate, DateTime endDate)
        {
            var domesticBookings = _context.DomesticBookings.Find(_ => true).ToList()
                .Where(b => DateTime.TryParse(b.TravelDate, out DateTime d) && d >= startDate && d <= endDate)
                .ToList();

            var internationalBookings = _context.InternationalBookings.Find(_ => true).ToList()
                .Where(b => DateTime.TryParse(b.DateInt, out DateTime d) && d >= startDate && d <= endDate)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("SKY-LINK AIRLINES - PERIODIC REPORT");
            sb.AppendLine($"Report Period: {startDate:dd/MM/yyyy} to {endDate:dd/MM/yyyy}");
            sb.AppendLine($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine(new string('=', 80));

            sb.AppendLine("\nDOMESTIC BOOKINGS:");
            sb.AppendLine(ExportToCSV(domesticBookings));

            sb.AppendLine("\nINTERNATIONAL BOOKINGS:");
            sb.AppendLine(ExportInternationalToCSV(internationalBookings));

            sb.AppendLine($"\nSUMMARY:");
            sb.AppendLine($"Total Domestic Bookings: {domesticBookings.Count}");
            sb.AppendLine($"Total International Bookings: {internationalBookings.Count}");
            sb.AppendLine($"Total Revenue: ${domesticBookings.Sum(b => b.TotalFare) + internationalBookings.Sum(b => b.TotalFare):F2}");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}