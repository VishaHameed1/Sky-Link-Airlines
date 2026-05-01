using MongoDB.Driver;
using System;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;
using VISHA_HAMEED.Services;

namespace VISHA_HAMEED.UI
{
    public class AdminPortal
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleService _scheduleService;
        private readonly PromotionService _promotionService;
        private readonly LoyaltyService _loyaltyService;
        private readonly AuthService _authService;
        private readonly AdminService _adminService;
        private readonly FeedbackService _feedbackService;
        private readonly AnalyticsService _analyticsService;
        private readonly ExportService _exportService;

        public AdminPortal(DatabaseContext context, ScheduleService scheduleService, PromotionService promotionService, LoyaltyService loyaltyService, AuthService authService)
        {
            _context = context;
            _scheduleService = scheduleService;
            _promotionService = promotionService;
            _loyaltyService = loyaltyService;
            _authService = authService;
            _adminService = new AdminService(context);
            _feedbackService = new FeedbackService(context);
            _analyticsService = new AnalyticsService(context);
            _exportService = new ExportService(context);
        }

        public void Show()
        {
            Console.Write("\n  Enter Admin Password: ");
            string pass = Console.ReadLine();
            if (pass != "admin123")
            {
                Console.WriteLine("\n  Unauthorized Access! Press any key...");
                Console.ReadKey();
                return;
            }

            bool logout = false;
            while (!logout)
            {
                Console.Clear();
                Console.WriteLine("================================================================================");
                Console.WriteLine("                             ADMIN DASHBOARD                                    ");
                Console.WriteLine("                              FULL ACCESS MODE                                  ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("\n  1.   Manage Live Schedules (CRUD)");
                Console.WriteLine("  2.   View All Passenger Bookings");
                Console.WriteLine("  3.   Update Passenger Record");
                Console.WriteLine("  4.   Delete/Cancel Booking");
                Console.WriteLine("  5.   View Cancelled Bookings History");
                Console.WriteLine("  6.   Generate Reports");
                Console.WriteLine("  7.   View Customer Feedback");
                Console.WriteLine("  8.   Manage Promotions");
                Console.WriteLine("  9.   Manage Users");
                Console.WriteLine("  10.  Manage Loyalty Program");
                Console.WriteLine("  11.  Real-time Flight Status");
                Console.WriteLine("  12.  Export Data");
                Console.WriteLine("  13.  System Statistics");
                Console.WriteLine("  14.  Logout");
                Console.Write("\n  Select Option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ManageSchedules(); break;
                    case "2": ViewAllBookings(); break;
                    case "3": UpdatePassengerRecord(); break;
                    case "4": DeleteBooking(); break;
                    case "5": ViewCancelledBookings(); break;
                    case "6": GenerateReports(); break;
                    case "7": ViewCustomerFeedback(); break;
                    case "8": ManagePromotions(); break;
                    case "9": ManageUsers(); break;
                    case "10": ManageLoyaltyProgram(); break;
                    case "11": ManageFlightStatus(); break;
                    case "12": ExportData(); break;
                    case "13": ShowSystemStatistics(); break;
                    case "14": logout = true; break;
                    default: Console.WriteLine("Invalid Option!"); Console.ReadKey(); break;
                }
            }
        }

        private void ManageSchedules()
        {
            bool back = false;
            while (!back)
            {
                DisplayFlightSchedules(false);
                Console.WriteLine("\n================================================================================");
                Console.WriteLine("                           SCHEDULE MANAGEMENT                                   ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("  1. Add New Flight");
                Console.WriteLine("  2. Update Existing Flight");
                Console.WriteLine("  3. Delete Flight");
                Console.WriteLine("  4. Update Flight Status");
                Console.WriteLine("  5. Back to Admin Menu");
                Console.Write("\n  Action: ");
                string act = Console.ReadLine();

                switch (act)
                {
                    case "1": AddSchedule(); break;
                    case "2": UpdateSchedule(); break;
                    case "3": DeleteSchedule(); break;
                    case "4": UpdateFlightStatus(); break;
                    case "5": back = true; break;
                    default: Console.WriteLine("  Invalid Option!"); Console.ReadKey(); break;
                }
            }
        }

        private void DisplayFlightSchedules(bool pause = true)
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

            if (pause)
            {
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void AddSchedule()
        {
            Console.Clear();
            var s = new FlightSchedule();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                      ADD NEW FLIGHT SCHEDULE                                   ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Flight ID (e.g. AI-202): "); s.FlightID = Console.ReadLine();
            Console.Write("  Type (Domestic/International): "); s.Type = Console.ReadLine();
            Console.Write("  Route (e.g. Karachi -> Lahore): "); s.Route = Console.ReadLine();
            Console.Write("  Boarding Time (e.g. 09:00 AM): "); s.Boarding = Console.ReadLine();
            Console.Write("  Landing Time (e.g. 10:45 AM): "); s.Landing = Console.ReadLine();
            Console.Write("  Available Seats: "); s.AvailableSeats = Console.ReadLine();
            Console.Write("  Gate Number: "); s.Gate = Console.ReadLine();
            Console.Write("  Ticket Price ($): "); s.Price = decimal.Parse(Console.ReadLine());
            Console.Write("  Terminal: "); s.Terminal = Console.ReadLine();

            if (_scheduleService.AddFlight(s))
                Console.WriteLine("\n  Flight Added Successfully!");
            else
                Console.WriteLine("\n  Failed to add flight!");
            Console.ReadKey();
        }

        private void UpdateSchedule()
        {
            Console.Clear();
            DisplayFlightSchedules(false);
            Console.Write("\n  Enter Flight ID to Update: ");
            string id = Console.ReadLine();

            var flight = _scheduleService.GetFlightById(id);
            if (flight == null)
            {
                Console.WriteLine("  Flight not found!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n  Updating Flight: {flight.FlightID}");
            Console.Write($"  New Boarding Time (current: {flight.Boarding}): ");
            string timeB = Console.ReadLine();
            if (!string.IsNullOrEmpty(timeB)) flight.Boarding = timeB;

            Console.Write($"  New Landing Time (current: {flight.Landing}): ");
            string timeL = Console.ReadLine();
            if (!string.IsNullOrEmpty(timeL)) flight.Landing = timeL;

            Console.Write($"  New Available Seats (current: {flight.AvailableSeats}): ");
            string seats = Console.ReadLine();
            if (!string.IsNullOrEmpty(seats)) flight.AvailableSeats = seats;

            Console.Write($"  New Gate (current: {flight.Gate}): ");
            string gate = Console.ReadLine();
            if (!string.IsNullOrEmpty(gate)) flight.Gate = gate;

            Console.Write($"  New Price (current: ${flight.Price}): ");
            string price = Console.ReadLine();
            if (!string.IsNullOrEmpty(price)) flight.Price = decimal.Parse(price);

            if (_scheduleService.UpdateFlight(id, flight))
                Console.WriteLine("\n  Schedule Updated Successfully!");
            else
                Console.WriteLine("\n  Failed to update schedule!");
            Console.ReadKey();
        }

        private void DeleteSchedule()
        {
            Console.Clear();
            DisplayFlightSchedules(false);
            Console.Write("\n  Enter Flight ID to Remove: ");
            string id = Console.ReadLine();

            if (_scheduleService.DeleteFlight(id))
                Console.WriteLine("\n  Flight Removed from Schedule.");
            else
                Console.WriteLine("\n  Flight not found!");
            Console.ReadKey();
        }

        private void UpdateFlightStatus()
        {
            Console.Clear();
            var flights = _scheduleService.GetAllFlights();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                          UPDATE FLIGHT STATUS                                   ");
            Console.WriteLine("================================================================================");

            Console.WriteLine("\n  Available Flights:");
            foreach (var f in flights)
            {
                Console.WriteLine($"  {f.FlightID} | {f.Route}");
            }

            Console.Write("\n  Enter Flight ID: ");
            string flightId = Console.ReadLine();

            Console.WriteLine("\n  Status Options:");
            Console.WriteLine("  1. On Time");
            Console.WriteLine("  2. Delayed");
            Console.WriteLine("  3. Boarding");
            Console.WriteLine("  4. Departed");
            Console.WriteLine("  5. Arrived");
            Console.WriteLine("  6. Cancelled");
            Console.Write("\n  Select Status: ");
            string statusChoice = Console.ReadLine();

            string status = statusChoice switch
            {
                "1" => "On Time",
                "2" => "Delayed",
                "3" => "Boarding",
                "4" => "Departed",
                "5" => "Arrived",
                "6" => "Cancelled",
                _ => "On Time"
            };

            if (statusChoice == "2")
            {
                Console.Write("  Delay Minutes: ");
                int delay = int.Parse(Console.ReadLine());
                Console.Write("  Delay Reason: ");
                string reason = Console.ReadLine();
            }

            Console.WriteLine($"\n  Flight {flightId} status updated to: {status}");
            Console.ReadKey();
        }

        private void ViewAllBookings()
        {
            Console.Clear();
            var domesticList = _adminService.GetAllDomesticBookings();
            var internationalList = _adminService.GetAllInternationalBookings();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                        ALL PASSENGER BOOKINGS                                  ");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"\n  Total Active Bookings: {domesticList.Count + internationalList.Count}");

            Console.WriteLine("\n  ------------------------------------------------------------------------------");
            Console.WriteLine("  | PNR    | Name                 | Flight     | Route         | Class    | Status   |");
            Console.WriteLine("  ------------------------------------------------------------------------------");

            foreach (var d in domesticList)
            {
                string route = $"{d.Source}->{d.Destination}";
                Console.WriteLine($"  | {d.PNR,-6} | {d.PassengerName,-20} | {d.FlightNumber,-10} | {route,-13} | {d.SeatClass,-8} | {d.Status,-8} |");
            }

            foreach (var i in internationalList)
            {
                string route = $"{i.Source}->{i.Destination}";
                Console.WriteLine($"  | {i.PnrInt,-6} | {i.PassengerName,-20} | {i.FlightNumber,-10} | {route,-13} | {i.SeatClass,-8} | {i.Status,-8} |");
            }

            Console.WriteLine("  ------------------------------------------------------------------------------");
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void UpdatePassengerRecord()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                        UPDATE PASSENGER RECORD                                 ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Enter PNR to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int pnr)) return;

            var domBooking = _adminService.GetAllDomesticBookings().FirstOrDefault(d => d.PNR == pnr);
            var intBooking = _adminService.GetAllInternationalBookings().FirstOrDefault(i => i.PnrInt == pnr);

            if (domBooking == null && intBooking == null)
            {
                Console.WriteLine("\n  Booking not found!");
                Console.ReadKey();
                return;
            }

            Console.Write("  Enter New Passenger Name: ");
            string newName = Console.ReadLine();
            Console.Write("  Enter New Contact Number: ");
            string newContact = Console.ReadLine();
            Console.Write("  Enter New Email: ");
            string newEmail = Console.ReadLine();

            bool updated = false;
            if (domBooking != null)
                updated = _adminService.UpdateDomesticBooking(pnr, newName, newContact, newEmail);
            else
                updated = _adminService.UpdateInternationalBooking(pnr, newName, newContact, newEmail);

            if (updated)
                Console.WriteLine("\n  Record Updated Successfully!");
            else
                Console.WriteLine("\n  Failed to update record!");
            Console.ReadKey();
        }

        private void DeleteBooking()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                     DELETE/CANCEL BOOKING (ADMIN)                               ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Enter PNR to Delete: ");
            if (!int.TryParse(Console.ReadLine(), out int pnr)) return;

            var domBooking = _adminService.GetAllDomesticBookings().FirstOrDefault(d => d.PNR == pnr);
            var intBooking = _adminService.GetAllInternationalBookings().FirstOrDefault(i => i.PnrInt == pnr);

            if (domBooking == null && intBooking == null)
            {
                Console.WriteLine("\n  Booking not found!");
                Console.ReadKey();
                return;
            }

            Console.Write("  Enter cancellation reason: ");
            string reason = Console.ReadLine();

            bool deleted = false;
            if (domBooking != null)
                deleted = _adminService.DeleteDomesticBooking(pnr, reason);
            else
                deleted = _adminService.DeleteInternationalBooking(pnr, reason);

            if (deleted)
                Console.WriteLine("\n  Booking Deleted Successfully!");
            else
                Console.WriteLine("\n  Failed to delete booking!");
            Console.ReadKey();
        }

        private void ViewCancelledBookings()
        {
            Console.Clear();
            var cancelled = _adminService.GetAllCancelledBookings();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                       CANCELLED BOOKINGS HISTORY                               ");
            Console.WriteLine("================================================================================");

            if (cancelled.Count == 0)
            {
                Console.WriteLine("\n  No cancelled bookings found.");
            }
            else
            {
                Console.WriteLine("\n  --------------------------------------------------------------------------------------------");
                Console.WriteLine("  | PNR    | Name                 | Type       | Cancelled Date     | Refund   | Reason          |");
                Console.WriteLine("  --------------------------------------------------------------------------------------------");

                foreach (var c in cancelled)
                {
                    Console.WriteLine($"  | {c.PNR,-6} | {c.PassengerName,-20} | {c.BookingType,-10} | {c.CancelledDate:dd/MM/yyyy HH:mm} | ${c.RefundAmount,-6} | {c.Reason,-15} |");
                }

                Console.WriteLine("  --------------------------------------------------------------------------------------------");
                Console.WriteLine($"\n  Total Cancellations: {cancelled.Count}");
                Console.WriteLine($"  Total Refunds Issued: ${_adminService.GetTotalRefunds():F2}");
            }
            Console.ReadKey();
        }

        private void GenerateReports()
        {
            Console.Clear();
            var dashboard = _analyticsService.GetDashboardMetrics();
            var revenueByRoute = _analyticsService.GetRevenueByRoute(DateTime.Now.AddMonths(-1), DateTime.Now);

            Console.WriteLine("================================================================================");
            Console.WriteLine("                         AIRLINE REPORTS DASHBOARD                              ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\n  BOOKING STATISTICS:");
            Console.WriteLine($"     * Active Domestic Bookings: {dashboard["DomesticBookings"]}");
            Console.WriteLine($"     * Active International Bookings: {dashboard["InternationalBookings"]}");
            Console.WriteLine($"     * Total Active Bookings: {dashboard["TotalBookings"]}");
            Console.WriteLine($"     * Cancelled Bookings: {dashboard["CancelledBookings"]}");
            Console.WriteLine($"     * Total Revenue: ${dashboard["TotalRevenue"]:F2}");
            Console.WriteLine($"     * Net Revenue: ${dashboard["NetRevenue"]:F2}");

            Console.WriteLine("\n  REVENUE BY ROUTE (Last Month):");
            Console.WriteLine("  ------------------------------------------------------------");
            foreach (var route in revenueByRoute.Take(10))
            {
                Console.WriteLine($"     * {route.Route}: {route.Bookings} bookings - ${route.Revenue:F2}");
            }

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ViewCustomerFeedback()
        {
            Console.Clear();
            var feedbacks = _feedbackService.GetAllFeedbacks();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                          CUSTOMER FEEDBACKS                                    ");
            Console.WriteLine("================================================================================");

            if (feedbacks.Count == 0)
            {
                Console.WriteLine("\n  No feedbacks received yet.");
            }
            else
            {
                foreach (var fb in feedbacks)
                {
                    Console.WriteLine($"\n  Email: {fb.Email}");
                    Console.WriteLine($"  Rating: {new string('*', fb.Rating)}{new string('.', 5 - fb.Rating)}");
                    Console.WriteLine($"  Comments: {fb.Comments}");
                    Console.WriteLine($"  Date: {fb.FeedbackDate:dd/MM/yyyy HH:mm}");
                    Console.WriteLine("  --------------------------------------------------------------------");
                }
                Console.WriteLine($"\n  Total Feedbacks: {_feedbackService.GetTotalFeedbacks()}");
                Console.WriteLine($"  Average Rating: {_feedbackService.GetAverageRating():F2} / 5.0");
            }
            Console.ReadKey();
        }

        private void ManagePromotions()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("================================================================================");
                Console.WriteLine("                          PROMOTION MANAGEMENT                                  ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("\n  1. Add New Promotion");
                Console.WriteLine("  2. View All Promotions");
                Console.WriteLine("  3. Update Promotion");
                Console.WriteLine("  4. Delete Promotion");
                Console.WriteLine("  5. Back");
                Console.Write("\n  Select: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddPromotion(); break;
                    case "2": ViewPromotions(); break;
                    case "3": UpdatePromotion(); break;
                    case "4": DeletePromotion(); break;
                    case "5": back = true; break;
                    default: Console.WriteLine("  Invalid Option!"); Console.ReadKey(); break;
                }
            }
        }

        private void AddPromotion()
        {
            Console.Clear();
            var promo = new Promotion();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                           ADD NEW PROMOTION                                     ");
            Console.WriteLine("================================================================================");
            Console.Write("\n  Promotion Title: "); promo.Title = Console.ReadLine();
            Console.Write("  Description: "); promo.Description = Console.ReadLine();
            Console.Write("  Promo Code: "); promo.PromoCode = Console.ReadLine();
            Console.Write("  Discount Percentage: "); promo.DiscountPercentage = int.Parse(Console.ReadLine());
            Console.Write("  Valid Until (yyyy-mm-dd): "); promo.ValidUntil = DateTime.Parse(Console.ReadLine());

            if (_promotionService.AddPromotion(promo))
                Console.WriteLine("\n  Promotion Added Successfully!");
            else
                Console.WriteLine("\n  Failed to add promotion!");
            Console.ReadKey();
        }

        private void ViewPromotions()
        {
            Console.Clear();
            var promotions = _promotionService.GetAllPromotions();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                           CURRENT PROMOTIONS                                    ");
            Console.WriteLine("================================================================================");

            if (promotions.Count == 0)
            {
                Console.WriteLine("\n  No active promotions at the moment.");
            }
            else
            {
                foreach (var promo in promotions)
                {
                    Console.WriteLine($"\n  Title: {promo.Title}");
                    Console.WriteLine($"  Description: {promo.Description}");
                    Console.WriteLine($"  Valid until: {promo.ValidUntil:dd/MM/yyyy}");
                    Console.WriteLine($"  Promo Code: {promo.PromoCode}");
                    Console.WriteLine($"  Discount: {promo.DiscountPercentage}% OFF");
                    Console.WriteLine("  --------------------------------------------------------------------");
                }
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void UpdatePromotion()
        {
            Console.Clear();
            ViewPromotions();
            Console.Write("\n  Enter Promo Code to Update: ");
            string code = Console.ReadLine();

            var promo = _promotionService.GetPromotionByCode(code);
            if (promo == null)
            {
                Console.WriteLine("  Promotion not found!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n  Updating Promotion: {promo.Title}");
            Console.Write($"  New Discount Percentage (current: {promo.DiscountPercentage}): ");
            int newDiscount = int.Parse(Console.ReadLine());
            Console.Write($"  New Valid Until (current: {promo.ValidUntil:yyyy-MM-dd}): ");
            DateTime newDate = DateTime.Parse(Console.ReadLine());

            if (_promotionService.UpdatePromotion(code, newDiscount, newDate))
                Console.WriteLine("\n  Promotion Updated!");
            else
                Console.WriteLine("\n  Failed to update promotion!");
            Console.ReadKey();
        }

        private void DeletePromotion()
        {
            Console.Clear();
            ViewPromotions();
            Console.Write("\n  Enter Promo Code to Delete: ");
            string code = Console.ReadLine();

            if (_promotionService.DeletePromotion(code))
                Console.WriteLine("\n  Promotion Deleted!");
            else
                Console.WriteLine("\n  Promotion not found!");
            Console.ReadKey();
        }

        private void ManageUsers()
        {
            Console.Clear();
            var users = _context.Users.Find(_ => true).ToList();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                             USER MANAGEMENT                                     ");
            Console.WriteLine("================================================================================");

            foreach (var user in users)
            {
                Console.WriteLine($"\n  Username: {user.Username}");
                Console.WriteLine($"  Name: {user.FullName}");
                Console.WriteLine($"  Email: {user.Email}");
                Console.WriteLine($"  Phone: {user.Phone}");
                Console.WriteLine($"  Role: {user.Role}");
                Console.WriteLine($"  Last Login: {user.LastLogin:dd/MM/yyyy HH:mm}");
                Console.WriteLine("  --------------------------------------------------------------------");
            }

            Console.WriteLine($"\n  Total Registered Users: {users.Count}");
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ManageLoyaltyProgram()
        {
            Console.Clear();
            var topMembers = _context.LoyaltyPrograms.Find(_ => true)
                .SortByDescending(l => l.TotalMiles)
                .Limit(10)
                .ToList();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                         TOP LOYALTY MEMBERS                                    ");
            Console.WriteLine("================================================================================");

            int rank = 1;
            foreach (var member in topMembers)
            {
                Console.WriteLine($"\n  #{rank++}. {member.Email}");
                Console.WriteLine($"     Tier: {member.Tier}");
                Console.WriteLine($"     Total Miles: {member.TotalMiles:N0}");
                Console.WriteLine($"     Available Miles: {member.AvailableMiles:N0}");
                Console.WriteLine($"     Total Flights: {member.TotalFlights}");
                Console.WriteLine($"     Total Spent: ${member.TotalSpent:F2}");
                Console.WriteLine("  --------------------------------------------------------------------");
            }

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ManageFlightStatus()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                          REAL-TIME FLIGHT STATUS                               ");
            Console.WriteLine("================================================================================");

            var flights = _scheduleService.GetAllFlights();

            foreach (var flight in flights)
            {
                var status = _context.FlightStatuses.Find(s => s.FlightId == flight.FlightID).FirstOrDefault();
                string statusText = status?.Status ?? "Scheduled";
                string color = statusText == "Delayed" ? "[DELAYED]" :
                              (statusText == "Cancelled" ? "[CANCELLED]" :
                              (statusText == "Departed" ? "[DEPARTED]" : "[ACTIVE]"));

                Console.WriteLine($"\n  {flight.FlightID} | {flight.Route} | {flight.Boarding} | {color} {statusText}");
                if (status?.DelayMinutes > 0)
                    Console.WriteLine($"     Delay: {status.DelayMinutes} minutes | Reason: {status.DelayReason}");
            }

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ExportData()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                              EXPORT DATA                                       ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\n  1. Export Bookings to CSV");
            Console.WriteLine("  2. Export Flights to CSV");
            Console.WriteLine("  3. Export Cancelled Bookings to CSV");
            Console.WriteLine("  4. Export Feedback to CSV");
            Console.WriteLine("  5. Export Loyalty Members to CSV");
            Console.WriteLine("  6. Back");
            Console.Write("\n  Select: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var bookings = _adminService.GetAllDomesticBookings();
                    var csv1 = _exportService.ExportToCSV(bookings);
                    _exportService.SaveToFile(csv1, $"Bookings_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                    Console.WriteLine("\n  Bookings exported successfully!");
                    break;

                case "2":
                    var flights = _scheduleService.GetAllFlights();
                    var csv2 = System.Text.Encoding.UTF8.GetString(_exportService.ExportToExcel(flights));
                    _exportService.SaveToFile(csv2, $"Flights_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                    Console.WriteLine("\n  Flights exported successfully!");
                    break;

                case "3":
                    var cancelled = _adminService.GetAllCancelledBookings();
                    var sb3 = new System.Text.StringBuilder();
                    sb3.AppendLine("PNR,Passenger Name,Booking Type,Cancelled Date,Refund Amount,Reason");
                    foreach (var c in cancelled)
                    {
                        sb3.AppendLine($"{c.PNR},{c.PassengerName},{c.BookingType},{c.CancelledDate:dd/MM/yyyy HH:mm},{c.RefundAmount},{c.Reason}");
                    }
                    _exportService.SaveToFile(sb3.ToString(), $"Cancelled_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                    Console.WriteLine("\n  Cancelled bookings exported successfully!");
                    break;

                case "4":
                    var feedbacks = _feedbackService.GetAllFeedbacks();
                    var sb4 = new System.Text.StringBuilder();
                    sb4.AppendLine("Email,Rating,Comments,Feedback Date");
                    foreach (var f in feedbacks)
                    {
                        sb4.AppendLine($"{f.Email},{f.Rating},{f.Comments},{f.FeedbackDate:dd/MM/yyyy HH:mm}");
                    }
                    _exportService.SaveToFile(sb4.ToString(), $"Feedback_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                    Console.WriteLine("\n  Feedback exported successfully!");
                    break;

                case "5":
                    var loyaltyMembers = _context.LoyaltyPrograms.Find(_ => true).ToList();
                    var sb5 = new System.Text.StringBuilder();
                    sb5.AppendLine("Email,Tier,Total Miles,Available Miles,Total Flights,Total Spent");
                    foreach (var l in loyaltyMembers)
                    {
                        sb5.AppendLine($"{l.Email},{l.Tier},{l.TotalMiles},{l.AvailableMiles},{l.TotalFlights},{l.TotalSpent}");
                    }
                    _exportService.SaveToFile(sb5.ToString(), $"LoyaltyMembers_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                    Console.WriteLine("\n  Loyalty members exported successfully!");
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("\n  Invalid option!");
                    break;
            }

            Console.WriteLine($"\n  Files saved to 'Exports' folder");
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ShowSystemStatistics()
        {
            Console.Clear();
            var domesticBookings = _adminService.GetAllDomesticBookings();
            var internationalBookings = _adminService.GetAllInternationalBookings();
            var cancelledBookings = _adminService.GetAllCancelledBookings();
            var feedbacks = _feedbackService.GetAllFeedbacks();
            var flightSchedules = _scheduleService.GetAllFlights();
            var users = _context.Users.Find(_ => true).ToList();
            var loyaltyMembers = _context.LoyaltyPrograms.Find(_ => true).ToList();

            Console.WriteLine("================================================================================");
            Console.WriteLine("                           SYSTEM STATISTICS                                    ");
            Console.WriteLine("================================================================================");

            Console.WriteLine("\n  DATABASE STATISTICS:");
            Console.WriteLine($"     * Domestic Bookings: {domesticBookings.Count}");
            Console.WriteLine($"     * International Bookings: {internationalBookings.Count}");
            Console.WriteLine($"     * Cancelled Bookings: {cancelledBookings.Count}");
            Console.WriteLine($"     * Customer Feedbacks: {feedbacks.Count}");
            Console.WriteLine($"     * Active Flights: {flightSchedules.Count}");
            Console.WriteLine($"     * Active Promotions: {_promotionService.GetAllPromotions().Count}");
            Console.WriteLine($"     * Registered Users: {users.Count}");
            Console.WriteLine($"     * Loyalty Members: {loyaltyMembers.Count}");

            Console.WriteLine("\n  REVENUE STATISTICS:");
            Console.WriteLine($"     * Gross Revenue: ${_adminService.GetTotalRevenue():F2}");
            Console.WriteLine($"     * Total Refunds: ${_adminService.GetTotalRefunds():F2}");
            Console.WriteLine($"     * Net Revenue: ${_adminService.GetTotalRevenue() - _adminService.GetTotalRefunds():F2}");
            Console.WriteLine($"     * Average Booking Value: ${(_adminService.GetTotalRevenue() / (domesticBookings.Count + internationalBookings.Count + 1)):F2}");

            Console.WriteLine("\n  CUSTOMER SATISFACTION:");
            if (feedbacks.Count > 0)
            {
                var avgRating = _feedbackService.GetAverageRating();
                Console.WriteLine($"     * Average Rating: {avgRating:F2} / 5.0");
                Console.WriteLine($"     * Satisfaction Rate: {(avgRating / 5 * 100):F1}%");
                Console.WriteLine($"     * Total Feedbacks: {feedbacks.Count}");

                var ratingDistribution = feedbacks.GroupBy(f => f.Rating)
                    .Select(g => new { Rating = g.Key, Count = g.Count() });
                Console.WriteLine("\n     Rating Distribution:");
                foreach (var r in ratingDistribution.OrderByDescending(r => r.Rating))
                {
                    Console.WriteLine($"         {r.Rating} stars: {new string('█', r.Count)}{r.Count}");
                }
            }
            else
            {
                Console.WriteLine($"     * No feedbacks received yet");
            }

            Console.WriteLine("\n  FLIGHT OCCUPANCY:");
            foreach (var flight in flightSchedules)
            {
                int totalSeats = 150;
                int availableSeats = int.Parse(flight.AvailableSeats);
                int occupiedSeats = totalSeats - availableSeats;
                double occupancyRate = (double)occupiedSeats / totalSeats * 100;

                string bar = new string('█', (int)(occupancyRate / 5));
                string emptyBar = new string('░', 20 - (int)(occupancyRate / 5));
                Console.WriteLine($"     * {flight.FlightID} [{bar}{emptyBar}] {occupancyRate:F1}% ({occupiedSeats}/{totalSeats})");
            }

            Console.WriteLine("\n  LOYALTY PROGRAM STATISTICS:");
            if (loyaltyMembers.Count > 0)
            {
                var tierDistribution = loyaltyMembers.GroupBy(l => l.Tier)
                    .Select(g => new { Tier = g.Key, Count = g.Count() });
                Console.WriteLine("     Tier Distribution:");
                foreach (var tier in tierDistribution)
                {
                    Console.WriteLine($"         {tier.Tier}: {tier.Count} members");
                }

                Console.WriteLine($"\n     Total Miles Issued: {loyaltyMembers.Sum(l => l.TotalMiles):N0}");
                Console.WriteLine($"     Total Miles Redeemed: {loyaltyMembers.Sum(l => l.LifetimeMiles - l.AvailableMiles):N0}");
                Console.WriteLine($"     Average Miles per Member: {loyaltyMembers.Average(l => l.TotalMiles):N0}");
            }

            Console.WriteLine("\n  SYSTEM HEALTH:");
            Console.WriteLine($"     * Database Connection: Active");
            Console.WriteLine($"     * Last Backup: {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"     * Total Records: {domesticBookings.Count + internationalBookings.Count + cancelledBookings.Count + users.Count + loyaltyMembers.Count}");

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }
    }
}