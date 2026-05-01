using AirlineReservationConsole;
using MongoDB.Driver;
using System;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;
using VISHA_HAMEED.Services;

namespace VISHA_HAMEED.UI
{
    public class CustomerPortal
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleService _scheduleService;
        private readonly PromotionService _promotionService;
        private readonly BookingService _bookingService;
        private readonly FeedbackService _feedbackService;
        private readonly AuthService _authService;
        private readonly LoyaltyService _loyaltyService;
        private readonly NotificationService _notificationService;
        private User _currentUser;

        public CustomerPortal(DatabaseContext context, ScheduleService scheduleService, PromotionService promotionService, AuthService authService, LoyaltyService loyaltyService)
        {
            _context = context;
            _scheduleService = scheduleService;
            _promotionService = promotionService;
            _authService = authService;
            _loyaltyService = loyaltyService;
            _bookingService = new BookingService(context, scheduleService);
            _feedbackService = new FeedbackService(context);
            _notificationService = new NotificationService(context);
        }

        public void Show()
        {
            if (!AuthenticateUser())
            {
                Console.WriteLine("\n  Press any key to return to main menu...");
                Console.ReadKey();
                return;
            }

            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("================================================================================");
                Console.WriteLine("                          CUSTOMER PORTAL                                      ");
                Console.WriteLine($"  Welcome, {_currentUser?.FullName ?? "Guest"}!                                  ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("\n  1.  View Flight Schedule");
                Console.WriteLine("  2.  Book Domestic Flight");
                Console.WriteLine("  3.  Book International Flight");
                Console.WriteLine("  4.  Search Booking (Check PNR Status)");
                Console.WriteLine("  5.  Cancel Booking");
                Console.WriteLine("  6.  Submit Feedback");
                Console.WriteLine("  7.  View My Bookings");
                Console.WriteLine("  8.  View My Loyalty Status");
                Console.WriteLine("  9.  Web Check-in");
                Console.WriteLine("  10. Back to Main Menu");
                Console.Write("\n  Select Option: ");

                string c = Console.ReadLine();
                switch (c)
                {
                    case "1": DisplayFlightSchedules(); break;
                    case "2": HandleDomesticBooking(); break;
                    case "3": HandleInternationalBooking(); break;
                    case "4": SearchByPNR(); break;
                    case "5": CancelBooking(); break;
                    case "6": SubmitFeedback(); break;
                    case "7": ViewMyBookings(); break;
                    case "8": ViewLoyaltyStatus(); break;
                    case "9": WebCheckIn(); break;
                    case "10": back = true; break;
                    default: Console.WriteLine("  Invalid Option!"); Console.ReadKey(); break;
                }
            }
        }

        private bool AuthenticateUser()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                          CUSTOMER LOGIN                                        ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\n  1. Login");
            Console.WriteLine("  2. Register New Account");
            Console.WriteLine("  3. Continue as Guest");
            Console.Write("\n  Select: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("\n  Username/Email: ");
                string username = Console.ReadLine();
                Console.Write("  Password: ");
                string password = Console.ReadLine();

                _currentUser = _authService.Login(username, password);
                if (_currentUser != null)
                {
                    Console.WriteLine($"\n  Welcome back, {_currentUser.FullName}!");
                    Console.ReadKey();
                    return true;
                }
                Console.WriteLine("\n  Invalid credentials! Press any key...");
                Console.ReadKey();
                return false;
            }
            else if (choice == "2")
            {
                Console.Write("\n  Full Name: ");
                string fullName = Console.ReadLine();
                Console.Write("  Username: ");
                string username = Console.ReadLine();
                Console.Write("  Email: ");
                string email = Console.ReadLine();
                Console.Write("  Phone: ");
                string phone = Console.ReadLine();
                Console.Write("  Password: ");
                string password = Console.ReadLine();

                if (_authService.Register(username, password, email, phone, fullName))
                {
                    Console.WriteLine("\n  Registration successful! Please login.");
                    Console.ReadKey();
                    return AuthenticateUser();
                }
                Console.WriteLine("\n  Registration failed! Username or email may already exist.");
                Console.ReadKey();
                return false;
            }

            return true; // Guest mode
        }

        private void DisplayFlightSchedules()
        {
            Console.Clear();
            var list = _scheduleService.GetAllFlights();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                    LIVE FLIGHT DEPARTURE & ARRIVAL                              ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("{0,-10} | {1,-13} | {2,-20} | {3,-9} | {4,-10} | {5,-6} | {6,-7} | {7,-6}",
                "Flight ID", "Type", "Route", "Boarding", "Landing", "Seats", "Gate", "Price");
            Console.WriteLine(new string('-', 105));

            foreach (var s in list)
            {
                Console.WriteLine("{0,-10} | {1,-13} | {2,-20} | {3,-9} | {4,-10} | {5,-6} | {6,-7} | ${7,-6}",
                    s.FlightID, s.Type, s.Route, s.Boarding, s.Landing, s.AvailableSeats, s.Gate ?? "TBA", s.Price);
            }
            Console.WriteLine(new string('=', 105));
            Console.WriteLine("\n  Note: Please reach airport 2 hours before boarding.");
            Console.WriteLine("\n  Press any key to continue...");
            Console.ReadKey();
        }

        private void HandleDomesticBooking()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                         DOMESTIC FLIGHT BOOKING                                 ");
            Console.WriteLine("================================================================================");

            var domesticFlights = _scheduleService.GetDomesticFlights();
            if (domesticFlights.Count == 0)
            {
                Console.WriteLine("\n  No domestic flights available!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n  Available Domestic Flights:");
            Console.WriteLine("  ------------------------------------------------------------");
            foreach (var flight in domesticFlights)
            {
                Console.WriteLine($"  {flight.FlightID} | {flight.Route} | {flight.Boarding}-{flight.Landing} | ${flight.Price} | Seats: {flight.AvailableSeats}");
            }

            Console.Write("\n  Enter Flight ID: ");
            string selectedFlightId = Console.ReadLine();
            var selectedFlight = _scheduleService.GetFlightById(selectedFlightId);

            if (selectedFlight == null)
            {
                Console.WriteLine("  Invalid Flight ID!");
                Console.ReadKey();
                return;
            }

            // Show seat map
            Console.Write("\n  Would you like to select a seat? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                var seatService = new SeatService(_context);
                seatService.DisplaySeatMap(selectedFlightId);
                Console.Write("  Enter Seat Number: ");
                string seatNumber = Console.ReadLine();
            }

            // Apply promo code
            Console.Write("\n  Do you have a promo code? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("  Enter Promo Code: ");
                string promoCode = Console.ReadLine();
                var discountedPrice = _promotionService.ApplyDiscount(selectedFlight.Price, promoCode);
                if (discountedPrice < selectedFlight.Price)
                {
                    Console.WriteLine($"  Promo code applied! You saved ${selectedFlight.Price - discountedPrice:F2}");
                    selectedFlight.Price = discountedPrice;
                }
                else
                {
                    Console.WriteLine("  Invalid or expired promo code!");
                }
            }

            // Check loyalty miles for discount
            if (_currentUser != null)
            {
                var loyalty = _loyaltyService.GetOrCreateLoyaltyAccount(_currentUser.Email, _currentUser.Id);
                if (loyalty != null && loyalty.AvailableMiles >= 5000)
                {
                    Console.Write($"\n  You have {loyalty.AvailableMiles} miles. Apply miles discount? (Y/N): ");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        Console.Write("  Enter miles to redeem (5000 minimum): ");
                        if (int.TryParse(Console.ReadLine(), out int miles) && miles >= 5000)
                        {
                            decimal discount = (miles / 100) * 5;
                            selectedFlight.Price -= discount;
                            _loyaltyService.RedeemMiles(_currentUser.Email, miles, $"Discount on booking");
                            Console.WriteLine($"  Applied {miles} miles discount: ${discount:F2}");
                        }
                    }
                }
            }

            using (var booking = new dom_booking())
            {
                Console.WriteLine("\n  PASSENGER DETAILS:");
                if (_currentUser != null)
                {
                    booking.PassengerName = _currentUser.FullName;
                    booking.Email = _currentUser.Email;
                    booking.ContactNo = _currentUser.Phone;
                    Console.WriteLine($"  Name: {booking.PassengerName}");
                    Console.WriteLine($"  Email: {booking.Email}");
                    Console.WriteLine($"  Contact: {booking.ContactNo}");
                }
                else
                {
                    Console.Write("  Passenger Name: "); booking.PassengerName = Console.ReadLine();
                    Console.Write("  Email: "); booking.Email = Console.ReadLine();
                    Console.Write("  Gender (Male/Female/Other): "); booking.Gender = Console.ReadLine();
                    Console.Write("  Contact No: "); booking.ContactNo = Console.ReadLine();
                }

                Console.Write("  Age: "); booking.Age = int.Parse(Console.ReadLine());
                Console.Write("  ID Proof Type (Aadhar/PAN/Driving License): "); booking.IDProofType = Console.ReadLine();
                Console.Write("  ID Proof Number: "); booking.IDProofNumber = Console.ReadLine();

                string[] routeParts = selectedFlight.Route.Split(new[] { " -> " }, StringSplitOptions.None);
                booking.SetTravelDetails(DateTime.Now, selectedFlight.FlightID, routeParts[0], routeParts[1]);

                Console.Write("\n  Meal Preference (Veg/Non-Veg): ");
                booking.SetMeal(Console.ReadLine());

                Console.Write("  Select Seat Class (Economy/Business/First): ");
                booking.SeatClass = Console.ReadLine();

                if (booking.Age >= 60)
                {
                    Console.Write("  Are you a senior citizen? (Y/N): ");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        booking.IsSeniorCitizen = true;
                        booking.ApplySeniorCitizenDiscount();
                    }
                }

                Console.Write("\n  Mode of Payment (Debit/Credit/Cash/PayPal): ");
                booking.ModeOfPayment = Console.ReadLine();

                Console.Write("  Enter Card/Ref Number: ");
                booking.CardNumber = Console.ReadLine();

                booking.PNR = _bookingService.GeneratePNR();
                booking.BookingDate = DateTime.Now;
                booking.TotalFare = selectedFlight.Price;
                booking.Status = "Confirmed";

                if (_bookingService.BookDomesticFlight(booking, selectedFlightId))
                {
                    // Add loyalty miles
                    if (_currentUser != null)
                    {
                        _loyaltyService.AddMiles(booking.PNR, (int)(booking.TotalFare * 5));
                    }

                    // ✅ SEND EMAIL CONFIRMATION
                    try
                    {
                        _notificationService.SendBookingConfirmation(
                            booking.Email,
                            booking.PassengerName,
                            booking.PNR,
                            booking.FlightNumber,
                            $"{booking.Source} → {booking.Destination}",
                            DateTime.Parse(booking.TravelDate),
                            booking.TotalFare
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n  ⚠ Could not send email: {ex.Message}");
                    }

                    Console.Clear();
                    Console.WriteLine("================================================================================");
                    Console.WriteLine("                           BOOKING CONFIRMED!                                    ");
                    Console.WriteLine("================================================================================");
                    booking.DisplayBookingInfo();
                    Console.WriteLine($"\n  Total Fare: ${booking.TotalFare}");
                    Console.WriteLine($"  PNR: {booking.PNR}");
                    Console.WriteLine($"\n  A confirmation email has been sent to {booking.Email}");
                }
                else
                {
                    Console.WriteLine("\n  Booking failed!");
                }
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void HandleInternationalBooking()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                       INTERNATIONAL FLIGHT BOOKING                              ");
            Console.WriteLine("================================================================================");

            var intlFlights = _scheduleService.GetInternationalFlights();
            if (intlFlights.Count == 0)
            {
                Console.WriteLine("\n  No international flights available!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n  Available International Flights:");
            Console.WriteLine("  ------------------------------------------------------------");
            foreach (var flight in intlFlights)
            {
                Console.WriteLine($"  {flight.FlightID} | {flight.Route} | {flight.Boarding}-{flight.Landing} | ${flight.Price} | Seats: {flight.AvailableSeats}");
            }

            Console.Write("\n  Enter Flight ID: ");
            string selectedFlightId = Console.ReadLine();
            var selectedFlight = _scheduleService.GetFlightById(selectedFlightId);

            if (selectedFlight == null)
            {
                Console.WriteLine("  Invalid Flight ID!");
                Console.ReadKey();
                return;
            }

            using (var booking = new int_booking())
            {
                Console.WriteLine("\n  PASSENGER DETAILS:");
                if (_currentUser != null)
                {
                    booking.PassengerName = _currentUser.FullName;
                    booking.Email = _currentUser.Email;
                    booking.ContactNo = _currentUser.Phone;
                    Console.WriteLine($"  Name: {booking.PassengerName}");
                    Console.WriteLine($"  Email: {booking.Email}");
                    Console.WriteLine($"  Contact: {booking.ContactNo}");
                }
                else
                {
                    Console.Write("  Passenger Name: "); booking.PassengerName = Console.ReadLine();
                    Console.Write("  Email: "); booking.Email = Console.ReadLine();
                    Console.Write("  Contact No: "); booking.ContactNo = Console.ReadLine();
                }

                Console.Write("  Passport Number: "); booking.PassportNumber = Console.ReadLine();
                Console.Write("  Passport Expiry Date (yyyy-mm-dd): "); booking.PassportExpiryDate = Console.ReadLine();
                Console.Write("  Nationality: "); booking.Nationality = Console.ReadLine();
                Console.Write("  Age: "); booking.Age = int.Parse(Console.ReadLine());
                Console.Write("  Purpose of Travel (Tourism/Business/Education): "); booking.PurposeOfTravel = Console.ReadLine();

                if (!booking.ValidatePassport())
                {
                    Console.WriteLine("\n  Passport must be valid for at least 6 months!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("\n  Do you have a valid visa? (Y/N): ");
                booking.HasVisa = Console.ReadLine().ToUpper() == "Y";
                if (booking.HasVisa)
                {
                    Console.Write("  Visa Type (Tourist/Business/Work/Student): "); booking.VisaType = Console.ReadLine();
                    Console.Write("  Visa Number: "); booking.VisaNumber = Console.ReadLine();
                    Console.Write("  Visa Expiry Date (yyyy-mm-dd): "); booking.VisaExpiryDate = DateTime.Parse(Console.ReadLine());
                }

                string[] routeParts = selectedFlight.Route.Split(new[] { " -> " }, StringSplitOptions.None);
                booking.SetIntTravelDetails(DateTime.Now, selectedFlight.FlightID, routeParts[0], routeParts[1]);

                Console.Write("\n  Meal Selection (Veg/Non-Veg/Halal): ");
                booking.SetMeal(Console.ReadLine());

                Console.Write("  Select Seat Class (Economy/Business/First): ");
                booking.SeatClass = Console.ReadLine();

                Console.Write("\n  International Baggage (Number of checked bags): ");
                int checkedBags = int.Parse(Console.ReadLine());
                Console.Write("  Number of carry-on bags: ");
                int carryOnBags = int.Parse(Console.ReadLine());
                booking.SetInternationalBaggage(checkedBags, carryOnBags);

                Console.Write("\n  Add lounge access? (Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    booking.AddLoungeAccess("Sky-Link Premium Lounge");
                }

                Console.Write("\n  Mode of Payment (Debit/Credit/Cash/PayPal): ");
                booking.ModeOfPayment = Console.ReadLine();

                Console.Write("  Enter Card Number: ");
                booking.CardNumber = Console.ReadLine();

                booking.PnrInt = _bookingService.GeneratePNR();
                booking.BookingDate = DateTime.Now;
                booking.TotalFare = selectedFlight.Price;
                booking.Status = "Confirmed";

                if (_bookingService.BookInternationalFlight(booking, selectedFlightId))
                {
                    // Add loyalty miles
                    if (_currentUser != null)
                    {
                        _loyaltyService.AddMiles(booking.PnrInt, (int)(booking.TotalFare * 5));
                    }

                    // ✅ SEND EMAIL CONFIRMATION
                    try
                    {
                        _notificationService.SendBookingConfirmation(
                            booking.Email,
                            booking.PassengerName,
                            booking.PnrInt,
                            booking.FlightNumber,
                            $"{booking.Source} → {booking.Destination}",
                            DateTime.Parse(booking.DateInt),
                            booking.TotalFare
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n  ⚠ Could not send email: {ex.Message}");
                    }

                    Console.Clear();
                    Console.WriteLine("================================================================================");
                    Console.WriteLine("                     INTERNATIONAL BOOKING CONFIRMED!                            ");
                    Console.WriteLine("================================================================================");
                    booking.DisplayBookingInfo();
                    Console.WriteLine($"\n  Total Fare: ${booking.TotalFare}");
                    Console.WriteLine($"  PNR: {booking.PnrInt}");
                    Console.WriteLine($"\n  A confirmation email has been sent to {booking.Email}");
                }
                else
                {
                    Console.WriteLine("\n  Booking failed!");
                }
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void SearchByPNR()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                         SEARCH BOOKING BY PNR                                  ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Enter PNR to Search: ");
            if (!int.TryParse(Console.ReadLine(), out int pnr))
            {
                Console.WriteLine("\n  Invalid PNR format!");
                Console.ReadKey();
                return;
            }

            var domResult = _bookingService.SearchDomesticBooking(pnr);
            var intResult = _bookingService.SearchInternationalBooking(pnr);

            if (domResult != null)
            {
                Console.WriteLine("\n  DOMESTIC BOOKING FOUND");
                domResult.DisplayBookingInfo();
            }
            else if (intResult != null)
            {
                Console.WriteLine("\n  INTERNATIONAL BOOKING FOUND");
                intResult.DisplayBookingInfo();
            }
            else
            {
                Console.WriteLine("\n  No Record Found with this PNR.");
            }
            Console.ReadKey();
        }

        private void CancelBooking()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                              CANCEL BOOKING                                    ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Enter PNR to Cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int pnr))
            {
                Console.WriteLine("\n  Invalid PNR format!");
                Console.ReadKey();
                return;
            }

            var domBooking = _bookingService.SearchDomesticBooking(pnr);
            var intBooking = _bookingService.SearchInternationalBooking(pnr);

            if (domBooking == null && intBooking == null)
            {
                Console.WriteLine("\n  No booking found with this PNR!");
                Console.ReadKey();
                return;
            }

            if (domBooking != null)
            {
                domBooking.DisplayBookingInfo();
                Console.Write("\n  Are you sure you want to cancel this booking? (Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    var refund = _bookingService.CancelDomesticBooking(pnr);
                    Console.WriteLine($"\n  Booking cancelled! Refund: ${refund:F2}");
                }
            }
            else
            {
                intBooking.DisplayBookingInfo();
                Console.Write("\n  Are you sure you want to cancel this booking? (Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    var refund = _bookingService.CancelInternationalBooking(pnr);
                    Console.WriteLine($"\n  Booking cancelled! Refund: ${refund:F2}");
                }
            }
            Console.ReadKey();
        }

        private void SubmitFeedback()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                           SUBMIT YOUR FEEDBACK                                 ");
            Console.WriteLine("================================================================================");

            Console.Write("\n  Enter your Email: ");
            string email = Console.ReadLine();
            Console.Write("  Rate your experience (1-5 Stars): ");
            int rating = int.Parse(Console.ReadLine());
            Console.Write("  Your Feedback/Comments: ");
            string comments = Console.ReadLine();

            var feedback = new Feedback
            {
                Email = email,
                Rating = rating,
                Comments = comments,
                FeedbackDate = DateTime.Now
            };

            if (_feedbackService.SubmitFeedback(feedback))
            {
                Console.WriteLine("\n  Thank you for your feedback!");
                if (rating >= 4)
                    Console.WriteLine("  We're thrilled you enjoyed your experience!");
                else if (rating == 3)
                    Console.WriteLine("  Thank you! We'll work to improve our services.");
                else
                    Console.WriteLine("  We apologize for the inconvenience. We'll contact you soon!");
            }
            Console.ReadKey();
        }

        private void ViewMyBookings()
        {
            Console.Clear();
            string email = _currentUser?.Email ?? "";

            if (string.IsNullOrEmpty(email))
            {
                Console.Write("  Enter your Email to view bookings: ");
                email = Console.ReadLine();
            }

            var domesticBookings = _context.DomesticBookings.Find(d => d.Email == email).ToList();
            var internationalBookings = _context.InternationalBookings.Find(i => i.Email == email).ToList();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                              YOUR BOOKINGS                                    ");
            Console.WriteLine("================================================================================");

            if (domesticBookings.Count == 0 && internationalBookings.Count == 0)
            {
                Console.WriteLine("\n  No bookings found for this email.");
            }
            else
            {
                Console.WriteLine($"\n  Email: {email}");
                Console.WriteLine($"  Total Bookings: {domesticBookings.Count + internationalBookings.Count}\n");
                Console.WriteLine("  ------------------------------------------------------------");

                foreach (var d in domesticBookings)
                {
                    Console.WriteLine($"  Domestic Flight - PNR: {d.PNR}");
                    Console.WriteLine($"  Flight: {d.FlightNumber} | {d.Source} -> {d.Destination}");
                    Console.WriteLine($"  Date: {d.TravelDate} | Status: {d.Status}");
                    Console.WriteLine($"  Total Fare: ${d.TotalFare}\n");
                }

                foreach (var i in internationalBookings)
                {
                    Console.WriteLine($"  International Flight - PNR: {i.PnrInt}");
                    Console.WriteLine($"  Flight: {i.FlightNumber} | {i.Source} -> {i.Destination}");
                    Console.WriteLine($"  Date: {i.DateInt} | Status: {i.Status}");
                    Console.WriteLine($"  Total Fare: ${i.TotalFare}\n");
                }
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ViewLoyaltyStatus()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("\n  Please login to view loyalty status!");
                Console.ReadKey();
                return;
            }

            var loyalty = _loyaltyService.GetOrCreateLoyaltyAccount(_currentUser.Email, _currentUser.Id);

            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                           YOUR LOYALTY STATUS                                  ");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"\n  Member: {_currentUser.FullName}");
            Console.WriteLine($"  Email: {_currentUser.Email}");
            Console.WriteLine($"\n  Current Tier: {loyalty?.Tier ?? "Bronze"}");
            Console.WriteLine($"  Available Miles: {loyalty?.AvailableMiles:N0 ?? 0}");
            Console.WriteLine($"  Lifetime Miles: {loyalty?.LifetimeMiles:N0 ?? 0}");
            Console.WriteLine($"  Total Flights: {loyalty?.TotalFlights ?? 0}");
            Console.WriteLine($"  Total Spent: ${loyalty?.TotalSpent:F2 ?? 0}");
            Console.WriteLine($"\n  Miles to Next Tier: {loyalty?.MilesToNextTier:N0 ?? 5000}");
            Console.WriteLine($"  Tier Valid Until: {loyalty?.TierValidUntil:dd/MM/yyyy ?? DateTime.Now.AddYears(1):dd/MM/yyyy}");

            Console.WriteLine("\n  TIER BENEFITS:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine(_loyaltyService.GetTierBenefits(loyalty?.Tier ?? "Bronze"));

            if (loyalty?.Transactions != null && loyalty.Transactions.Count > 0)
            {
                Console.WriteLine("\n  RECENT TRANSACTIONS:");
                Console.WriteLine("  ------------------------------------------------------------");
                var recentTransactions = loyalty.Transactions.Take(5);
                foreach (var trans in recentTransactions)
                {
                    Console.WriteLine($"  {trans.Date:dd/MM/yyyy} | {trans.Type} | {trans.Miles} miles | {trans.Description}");
                }
            }

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void WebCheckIn()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                              WEB CHECK-IN                                      ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Enter PNR to Check-in: ");
            if (!int.TryParse(Console.ReadLine(), out int pnr))
            {
                Console.WriteLine("\n  Invalid PNR format!");
                Console.ReadKey();
                return;
            }

            var checkInService = new CheckInService(_context, new SeatService(_context), _notificationService);

            if (!checkInService.CanCheckIn(pnr))
            {
                Console.WriteLine("\n  Check-in is not available for this booking.");
                Console.WriteLine("  Check-in opens 48 hours before departure and closes 1 hour before.");
                Console.ReadKey();
                return;
            }

            // Get booking details
            var domBooking = _bookingService.SearchDomesticBooking(pnr);
            var intBooking = _bookingService.SearchInternationalBooking(pnr);

            if (domBooking == null && intBooking == null)
            {
                Console.WriteLine("\n  Booking not found!");
                Console.ReadKey();
                return;
            }

            string flightNumber = domBooking?.FlightNumber ?? intBooking?.FlightNumber;
            string source = domBooking?.Source ?? intBooking?.Source;
            string destination = domBooking?.Destination ?? intBooking?.Destination;
            string travelDate = domBooking?.TravelDate ?? intBooking?.DateInt;

            Console.WriteLine($"\n  Flight: {flightNumber}");
            Console.WriteLine($"  Route: {source} -> {destination}");
            Console.WriteLine($"  Date: {travelDate}");

            // Show available seats
            var seatService = new SeatService(_context);
            seatService.DisplaySeatMap(flightNumber);

            Console.Write("\n  Select Seat Number: ");
            string seatNumber = Console.ReadLine();

            var checkIn = checkInService.PerformCheckIn(pnr, "Web", seatNumber);
            if (checkIn != null)
            {
                Console.WriteLine($"\n  Check-in successful!");
                Console.WriteLine($"  Boarding Pass: {checkIn.BoardingPassNumber}");
                Console.WriteLine($"  Seat: {checkIn.SeatNumber}");
                Console.WriteLine($"  Boarding Group: {checkIn.BoardingGroup}");
                Console.WriteLine($"\n  A boarding pass has been sent to your email.");
            }
            else
            {
                Console.WriteLine("\n  Check-in failed. Please try again or contact support.");
            }
            Console.ReadKey();
        }
    }
}