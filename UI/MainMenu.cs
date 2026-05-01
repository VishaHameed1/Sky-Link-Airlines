using System;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Services;

namespace VISHA_HAMEED.UI
{
    public class MainMenu
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleService _scheduleService;
        private readonly PromotionService _promotionService;
        private readonly LoyaltyService _loyaltyService;
        private readonly AuthService _authService;
        private readonly NotificationService _notificationService;
        private readonly CustomerPortal _customerPortal;
        private readonly AdminPortal _adminPortal;

        public MainMenu()
        {
            _context = new DatabaseContext();
            _scheduleService = new ScheduleService(_context);
            _promotionService = new PromotionService(_context);
            _notificationService = new NotificationService(_context);
            _loyaltyService = new LoyaltyService(_context, _notificationService);
            _authService = new AuthService(_context, _notificationService);

            // Seed initial data
            _scheduleService.SeedInitialFlights();
            _promotionService.SeedInitialPromotions();

            _customerPortal = new CustomerPortal(_context, _scheduleService, _promotionService, _authService, _loyaltyService);
            _adminPortal = new AdminPortal(_context, _scheduleService, _promotionService, _loyaltyService, _authService);
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("================================================================================");
                Console.WriteLine("                SKY-LINK AIRLINE RESERVATION SYSTEM v3.0                       ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("\n   WELCOME TO SKY-LINK AIRLINES");
                Console.WriteLine("\n--------------------------------------------------------------------------------");
                Console.WriteLine("  1.  CUSTOMER PORTAL");
                Console.WriteLine("  2.  ADMIN PORTAL");
                Console.WriteLine("  3.  VIEW PROMOTIONS");
                Console.WriteLine("  4.  LOYALTY PROGRAM INFO");
                Console.WriteLine("  5.  CONTACT US");
                Console.WriteLine("  6.  TEST EMAIL CONFIGURATION");
                Console.WriteLine("  7.  EXIT");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.Write("\n  Select: ");
                string role = Console.ReadLine();

                if (role == "1") _customerPortal.Show();
                else if (role == "2") _adminPortal.Show();
                else if (role == "3") ViewPromotions();
                else if (role == "4") ShowLoyaltyInfo();
                else if (role == "5") ContactUs();
                else if (role == "6") TestEmailConfiguration();
                else if (role == "7") break;
                else Console.WriteLine("\n  Invalid option! Press any key..."); Console.ReadKey();
            }
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
                    Console.WriteLine($"\n  {promo.Title}");
                    Console.WriteLine($"  {promo.Description}");
                    Console.WriteLine($"  Valid until: {promo.ValidUntil:dd/MM/yyyy}");
                    Console.WriteLine($"  Promo Code: {promo.PromoCode} | {promo.DiscountPercentage}% OFF");
                    Console.WriteLine("  --------------------------------------------------------------------");
                }
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ShowLoyaltyInfo()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                         LOYALTY PROGRAM INFORMATION                            ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\n  LOYALTY TIERS:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  Bronze  : 0 - 4,999 miles | Standard benefits");
            Console.WriteLine("  Silver  : 5,000 - 9,999 miles | Priority check-in, 5kg extra baggage");
            Console.WriteLine("  Gold    : 10,000 - 24,999 miles | Free lounge access, Priority boarding");
            Console.WriteLine("  Platinum: 25,000+ miles | All benefits + Companion ticket annually");
            Console.WriteLine("\n  HOW TO EARN MILES:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  * 5 miles per $1 spent on flights");
            Console.WriteLine("  * 10 miles per $1 spent on premium cabin");
            Console.WriteLine("  * 500 bonus miles on first flight");
            Console.WriteLine("  * 1000 bonus miles on birthday month");
            Console.WriteLine("\n  HOW TO REDEEM MILES:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  * 5000 miles = $50 off on next booking");
            Console.WriteLine("  * 10000 miles = Free upgrade to Business class");
            Console.WriteLine("  * 25000 miles = Free domestic flight ticket");
            Console.WriteLine("  * 50000 miles = Free international flight ticket");
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void ContactUs()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                              CONTACT US                                        ");
            Console.WriteLine("================================================================================");
            Console.WriteLine("\n  CUSTOMER SUPPORT:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  Phone   : +92-21-111-000-786");
            Console.WriteLine("  WhatsApp: +92-300-1234567");
            Console.WriteLine("  Email   : support@skylinkairlines.com");
            Console.WriteLine("\n  WORKING HOURS:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  Monday - Sunday: 24/7");
            Console.WriteLine("\n  SOCIAL MEDIA:");
            Console.WriteLine("  ------------------------------------------------------------");
            Console.WriteLine("  Facebook : @SkyLinkAirlines");
            Console.WriteLine("  Twitter  : @SkyLinkAir");
            Console.WriteLine("  Instagram: @skylink_airlines");
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        private void TestEmailConfiguration()
        {
            Console.Clear();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                           TEST EMAIL CONFIGURATION                             ");
            Console.WriteLine("================================================================================");

            if (!_notificationService.IsEmailConfigured())
            {
                Console.WriteLine("\n  ❌ Email service is not configured!");
                Console.WriteLine("\n  Please configure your Gmail credentials in NotificationService.cs:");
                Console.WriteLine("  ----------------------------------------");
                Console.WriteLine("  private readonly string senderEmail = \"your-email@gmail.com\";");
                Console.WriteLine("  private readonly string senderPassword = \"your-app-password\";");
                Console.WriteLine("  ----------------------------------------");
                Console.WriteLine("\n  Steps to get App Password:");
                Console.WriteLine("  1. Go to https://myaccount.google.com/");
                Console.WriteLine("  2. Enable 2-Step Verification");
                Console.WriteLine("  3. Generate App Password for Mail");
                Console.WriteLine("  4. Copy the 16-digit password");
                Console.WriteLine("\n  Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n  ✅ Email service is configured!");
            Console.WriteLine($"  📧 Sending from: vishahameed666@gmail.com");
            Console.WriteLine("\n  Enter your email address to receive a test message:");
            Console.Write("  📧 ");
            string testEmail = Console.ReadLine();

            if (string.IsNullOrEmpty(testEmail) || !testEmail.Contains("@"))
            {
                Console.WriteLine("\n  ❌ Invalid email address!");
                Console.WriteLine("\n  Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n  Sending test email...");
            Console.WriteLine("  ⏳ Please wait...");

            try
            {
                // Run async task synchronously
                var task = _notificationService.TestEmailConfiguration(testEmail);
                task.Wait();
                var result = task.Result;

                if (result)
                {
                    Console.WriteLine("\n  ✅ Test email sent successfully!");
                    Console.WriteLine($"  📧 Please check your inbox/spam folder at {testEmail}");
                    Console.WriteLine("\n  ✈️  Your email configuration is working perfectly!");
                }
                else
                {
                    Console.WriteLine("\n  ❌ Failed to send test email.");
                    Console.WriteLine("\n  Troubleshooting tips:");
                    Console.WriteLine("  1. Verify your Gmail address is correct");
                    Console.WriteLine("  2. Check if App Password is valid (spaces included)");
                    Console.WriteLine("  3. Make sure 2-Step Verification is enabled");
                    Console.WriteLine("  4. Check your internet connection");
                    Console.WriteLine("  5. Try regenerating the App Password");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  ❌ Error: {ex.Message}");
                Console.WriteLine("\n  Common issues and solutions:");
                Console.WriteLine("  ----------------------------------------");
                Console.WriteLine("  Issue: Authentication failed");
                Console.WriteLine("  Fix: Check your App Password (spaces matter)");
                Console.WriteLine("");
                Console.WriteLine("  Issue: Connection refused");
                Console.WriteLine("  Fix: Check your internet connection");
                Console.WriteLine("");
                Console.WriteLine("  Issue: MailKit not found");
                Console.WriteLine("  Fix: Install NuGet package: Install-Package MailKit");
                Console.WriteLine("  ----------------------------------------");
            }

            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }
    }
}