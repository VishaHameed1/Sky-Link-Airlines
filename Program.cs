using System;
using VISHA_HAMEED.UI;
using VISHA_HAMEED.Services;
using VISHA_HAMEED.Data;

namespace VISHA_HAMEED
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("================================================================================");
                Console.WriteLine("                    SKY-LINK AIRLINE RESERVATION SYSTEM v3.0                   ");
                Console.WriteLine("================================================================================");
                Console.WriteLine("\n  Initializing system...");
                Console.WriteLine("  Connecting to MongoDB...");

                var mainMenu = new MainMenu();

                Console.WriteLine("  ✓ Connected to MongoDB successfully!");
                Console.WriteLine("  ✓ Database initialized successfully!");
                Console.WriteLine("  ✓ All services loaded successfully!");

                // Check email configuration
                Console.WriteLine("\n  Checking email configuration...");
                var context = new DatabaseContext();
                var notificationService = new NotificationService(context);

                if (notificationService.IsEmailConfigured())
                {
                    Console.WriteLine("  ✓ Email service is configured and ready!");
                    Console.WriteLine($"  📧 Sending emails from: vishahameed666@gmail.com");
                }
                else
                {
                    Console.WriteLine("  ⚠ Email service not configured. Please check NotificationService.cs");
                    Console.WriteLine("  ⚠ Booking confirmations will not be sent.");
                }

                Console.WriteLine("\n  ✈️  Welcome to Sky-Link Airlines!");
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();

                mainMenu.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  ERROR: {ex.Message}");
                Console.WriteLine("\n  Please ensure:");
                Console.WriteLine("  1. MongoDB is installed and running on localhost:27017");
                Console.WriteLine("  2. You have the required MongoDB.Driver NuGet package");
                Console.WriteLine("  3. All project references are correct");
                Console.WriteLine("\n  Troubleshooting:");
                Console.WriteLine("  - Make sure MongoDB service is running (Windows: services.msc)");
                Console.WriteLine("  - Check if port 27017 is not blocked by firewall");
                Console.WriteLine("  - Verify connection string in DatabaseContext.cs");
                Console.WriteLine("\n  Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}