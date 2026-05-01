using MongoDB.Driver;
using System;
using System.Linq;
using AirlineReservationConsole;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class CheckInService
    {
        private readonly DatabaseContext _context;
        private readonly SeatService _seatService;
        private readonly NotificationService _notificationService;

        public CheckInService(DatabaseContext context, SeatService seatService, NotificationService notificationService)
        {
            _context = context;
            _seatService = seatService;
            _notificationService = notificationService;
        }

        public bool CanCheckIn(int pnr)
        {
            var domesticBooking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            var internationalBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();

            // Check if booking exists and is confirmed
            if (domesticBooking != null)
            {
                if (domesticBooking.Status != "Confirmed") return false;

                string travelDateStr = domesticBooking.TravelDate;
                if (string.IsNullOrEmpty(travelDateStr)) return false;

                try
                {
                    DateTime departureDate = DateTime.Parse(travelDateStr);
                    var hoursUntilDeparture = (departureDate - DateTime.Now).TotalHours;
                    return hoursUntilDeparture <= 48 && hoursUntilDeparture >= 1;
                }
                catch
                {
                    return false;
                }
            }
            else if (internationalBooking != null)
            {
                if (internationalBooking.Status != "Confirmed") return false;

                string travelDateStr = internationalBooking.DateInt;
                if (string.IsNullOrEmpty(travelDateStr)) return false;

                try
                {
                    DateTime departureDate = DateTime.Parse(travelDateStr);
                    var hoursUntilDeparture = (departureDate - DateTime.Now).TotalHours;
                    return hoursUntilDeparture <= 48 && hoursUntilDeparture >= 1;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public CheckIn PerformCheckIn(int pnr, string method, string seatNumber = null)
        {
            if (!CanCheckIn(pnr))
                return null;

            var checkIn = new CheckIn
            {
                PNR = pnr,
                CheckInTime = DateTime.Now,
                CheckInMethod = method,
                BoardingPassNumber = GenerateBoardingPassNumber(),
                QRCode = GenerateQRCode(pnr),
                BoardingGroup = GetBoardingGroup(),
                BoardingSequence = GetNextSequence(),
                BaggageDropped = false,
                BaggageCount = 0,
                ExcessBaggageFee = 0,
                SeatNumber = seatNumber ?? "",
                CounterNumber = ""
            };

            // Assign seat if requested
            if (!string.IsNullOrEmpty(seatNumber))
            {
                checkIn.SeatNumber = seatNumber;
                _seatService.AssignSeat(pnr, seatNumber);
            }

            _context.CheckIns.InsertOne(checkIn);

            // Update booking status
            string email = "";
            var domesticBooking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
            if (domesticBooking != null)
            {
                domesticBooking.Status = "Checked-in";
                _context.DomesticBookings.ReplaceOne(d => d.PNR == pnr, domesticBooking);
                email = domesticBooking.Email ?? "";
            }
            else
            {
                var internationalBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
                if (internationalBooking != null)
                {
                    internationalBooking.Status = "Checked-in";
                    _context.InternationalBookings.ReplaceOne(i => i.PnrInt == pnr, internationalBooking);
                    email = internationalBooking.Email ?? "";
                }
            }

            // Send notification
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    _notificationService?.SendEmail(email, "Check-in Successful",
                        $"Your check-in is complete!\n\n" +
                        $"Boarding Pass: {checkIn.BoardingPassNumber}\n" +
                        $"Seat: {checkIn.SeatNumber}\n" +
                        $"Boarding Group: {checkIn.BoardingGroup}\n\n" +
                        $"Please arrive at the gate 30 minutes before departure.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }

            return checkIn;
        }

        public CheckIn GetCheckIn(int pnr)
        {
            return _context.CheckIns.Find(c => c.PNR == pnr).FirstOrDefault();
        }

        public void DropBaggage(int pnr, int bagCount, decimal excessFee = 0)
        {
            var checkIn = _context.CheckIns.Find(c => c.PNR == pnr).FirstOrDefault();
            if (checkIn != null)
            {
                checkIn.BaggageDropped = true;
                checkIn.BaggageCount = bagCount;
                checkIn.ExcessBaggageFee = excessFee;
                _context.CheckIns.ReplaceOne(c => c.PNR == pnr, checkIn);

                // Send notification
                try
                {
                    string email = "";
                    var domesticBooking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();
                    if (domesticBooking != null)
                    {
                        email = domesticBooking.Email ?? "";
                    }
                    else
                    {
                        var internationalBooking = _context.InternationalBookings.Find(i => i.PnrInt == pnr).FirstOrDefault();
                        if (internationalBooking != null)
                        {
                            email = internationalBooking.Email ?? "";
                        }
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        _notificationService?.SendEmail(email, "Baggage Drop Confirmation",
                            $"Your baggage has been checked in successfully.\n\n" +
                            $"Number of bags: {bagCount}\n" +
                            $"Excess baggage fee: ${excessFee:F2}\n\n" +
                            $"Your bags will be loaded onto the flight.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send notification: {ex.Message}");
                }
            }
        }

        public bool UpdateSeatAssignment(int pnr, string newSeatNumber)
        {
            var checkIn = _context.CheckIns.Find(c => c.PNR == pnr).FirstOrDefault();
            if (checkIn == null) return false;

            // Release old seat if assigned
            if (!string.IsNullOrEmpty(checkIn.SeatNumber))
            {
                // Logic to release old seat would go here
            }

            checkIn.SeatNumber = newSeatNumber;
            _context.CheckIns.ReplaceOne(c => c.PNR == pnr, checkIn);

            return _seatService.AssignSeat(pnr, newSeatNumber);
        }

        private string GenerateBoardingPassNumber()
        {
            return "BP" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }

        private string GenerateQRCode(int pnr)
        {
            // In production, generate actual QR code image
            // For now, return a URL or encoded string
            return $"https://skylinkairlines.com/boardingpass/{pnr}";
        }

        private string GetBoardingGroup()
        {
            var random = new Random();
            int group = random.Next(1, 5);
            return $"Group {group}";
        }

        private int GetNextSequence()
        {
            var count = _context.CheckIns.CountDocuments(_ => true);
            return (int)count + 1;
        }

        public int GetCheckedInCount()
        {
            return (int)_context.CheckIns.CountDocuments(_ => true);
        }

        public int GetBaggageDroppedCount()
        {
            return (int)_context.CheckIns.CountDocuments(c => c.BaggageDropped == true);
        }
    }
}