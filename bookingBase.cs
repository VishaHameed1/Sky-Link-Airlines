using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AirlineReservationConsole
{
    // Yeh class common attributes hold karti hai jo Domestic aur International dono flights ke liye hain
    public class bookingBase : IDisposable
    {
        // 1. MongoDB Identity
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // 2. Personal Information (As per Screenshot 1)
        public string PassengerName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string PassportNumber { get; set; } // Needed for International/Domestic records
        public int Age { get; set; } // Added Age field
        public string Nationality { get; set; } // Added Nationality field

        // 3. Flight & Payment Details (As per Screenshots 4 & 5)
        public string FlightNumber { get; set; }
        public string Destination { get; set; }
        public string Source { get; set; }
        public double Price { get; set; }
        public decimal TotalFare { get; set; } // Added Total Fare with proper calculation
        public string ModeOfPayment { get; set; }
        public string CardNumber { get; set; } // Masking can be applied later for security

        // 4. Additional Booking Details
        public string SeatClass { get; set; } // Economy, Business, First
        public string MealPreference { get; set; } // Veg, Non-Veg, Halal
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } // Confirmed, Cancelled, Waiting, Checked-in
        public string SpecialRequests { get; set; } // Any special requests from passenger
        public string TravelDate { get; set; } // ADDED: Travel date as string for display

        // 5. Baggage Information
        public int CheckedBags { get; set; }
        public int CarryOnBags { get; set; }
        public double TotalBaggageWeight { get; set; } // in KG

        // 6. Seat Assignment
        public string SeatNumber { get; set; }
        public string BoardingGate { get; set; }
        public string BoardingTime { get; set; }

        // 7. Frequent Flyer Information
        public string FrequentFlyerNumber { get; set; }
        public int RewardPointsEarned { get; set; }

        // 8. Emergency Contact
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string EmergencyContactRelation { get; set; }

        // 9. Resource Management (IDisposable Pattern)
        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Yahan managed objects clean up kiye ja sakte hain
                    // Clear sensitive data
                    CardNumber = null;
                    PassengerName = null;
                    Email = null;
                    ContactNo = null;
                }
                disposedValue = true;
            }
        }

        ~bookingBase()
        {
            Dispose(false);
        }

        // 10. Method to mask sensitive information
        public string GetMaskedCardNumber()
        {
            if (string.IsNullOrEmpty(CardNumber) || CardNumber.Length < 4)
                return "****";
            return "**** **** **** " + CardNumber.Substring(CardNumber.Length - 4);
        }

        // 11. Calculate Reward Points based on fare
        public void CalculateRewardPoints()
        {
            // 5 points per dollar spent
            RewardPointsEarned = (int)(TotalFare * 5);
        }

        // 12. Display Method (Formatted like Ticket Details in Screenshot 6)
        public virtual void DisplayBookingInfo()
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    SKY-LINK AIRLINES                         ║");
            Console.WriteLine("║                    ELECTRONIC TICKET                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n✈️  PASSENGER INFORMATION");
            Console.WriteLine("   ═══════════════════════════════════════════════════════════");
            Console.WriteLine($"   Name              : {PassengerName}");
            Console.WriteLine($"   Gender            : {Gender}");
            Console.WriteLine($"   Age               : {Age}");
            Console.WriteLine($"   Nationality       : {Nationality ?? "N/A"}");
            Console.WriteLine($"   Email             : {Email}");
            Console.WriteLine($"   Contact           : {ContactNo}");

            if (!string.IsNullOrEmpty(PassportNumber))
                Console.WriteLine($"   Passport No.      : {PassportNumber}");

            if (!string.IsNullOrEmpty(FrequentFlyerNumber))
                Console.WriteLine($"   Frequent Flyer    : {FrequentFlyerNumber}");

            Console.WriteLine("\n✈️  FLIGHT DETAILS");
            Console.WriteLine("   ═══════════════════════════════════════════════════════════");
            Console.WriteLine($"   Flight Number     : {FlightNumber}");
            Console.WriteLine($"   Route             : {Source} → {Destination}");

            // Use TravelDate if available, otherwise use BookingDate
            if (!string.IsNullOrEmpty(TravelDate))
                Console.WriteLine($"   Date              : {TravelDate}");
            else
                Console.WriteLine($"   Date              : {BookingDate:dddd, MMMM dd, yyyy}");

            Console.WriteLine($"   Boarding Gate     : {BoardingGate ?? "To be announced"}");
            Console.WriteLine($"   Boarding Time     : {BoardingTime ?? "Check at counter"}");
            Console.WriteLine($"   Seat Number       : {SeatNumber ?? "To be assigned"}");
            Console.WriteLine($"   Seat Class        : {SeatClass ?? "Economy"}");

            Console.WriteLine("\n🎒 BAGGAGE INFORMATION");
            Console.WriteLine("   ═══════════════════════════════════════════════════════════");
            Console.WriteLine($"   Checked Bags      : {CheckedBags} pcs");
            Console.WriteLine($"   Carry-On Bags     : {CarryOnBags} pcs");
            Console.WriteLine($"   Total Weight      : {TotalBaggageWeight} KG");

            Console.WriteLine("\n🍽️  MEAL PREFERENCE");
            Console.WriteLine($"   {MealPreference ?? "Standard Meal"}");

            if (!string.IsNullOrEmpty(SpecialRequests))
            {
                Console.WriteLine("\n📝 SPECIAL REQUESTS");
                Console.WriteLine($"   {SpecialRequests}");
            }

            Console.WriteLine("\n💰 PAYMENT INFORMATION");
            Console.WriteLine("   ═══════════════════════════════════════════════════════════");
            Console.WriteLine($"   Total Fare        : ${TotalFare:F2} (Rs. {TotalFare * 280:F2})");
            Console.WriteLine($"   Payment Mode      : {ModeOfPayment}");
            Console.WriteLine($"   Card Number       : {GetMaskedCardNumber()}");
            Console.WriteLine($"   Reward Points     : {RewardPointsEarned} points");

            Console.WriteLine("\n🚨 EMERGENCY CONTACT");
            Console.WriteLine($"   Name              : {EmergencyContactName ?? "Not provided"}");
            Console.WriteLine($"   Relation          : {EmergencyContactRelation ?? "N/A"}");
            Console.WriteLine($"   Contact           : {EmergencyContactNumber ?? "N/A"}");

            Console.WriteLine("\n📋 BOOKING STATUS");
            Console.WriteLine($"   Status            : {Status ?? "Confirmed"}");
            Console.WriteLine($"   Booking Date      : {BookingDate:dd/MM/yyyy HH:mm}");

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("  Thank you for choosing Sky-Link Airlines! Safe Journey! ✈️");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
        }

        // 13. Method to display summary for list views
        public virtual void DisplaySummary()
        {
            Console.WriteLine($"│ {PassengerName,-20} │ {FlightNumber,-10} │ {Source,3}→{Destination,-3} │ {SeatClass,-10} │ {Status,-10} │");
        }

        // 14. Method to validate mandatory fields
        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(PassengerName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(ContactNo) &&
                   !string.IsNullOrEmpty(FlightNumber) &&
                   TotalFare > 0;
        }

        // 15. Method to update booking status
        public virtual void UpdateStatus(string newStatus)
        {
            Status = newStatus;
            Console.WriteLine($"Booking status updated to: {newStatus}");
        }

        // 16. Method to assign seat
        public virtual void AssignSeat(string seatNumber, string gate, string boardingTime)
        {
            SeatNumber = seatNumber;
            BoardingGate = gate;
            BoardingTime = boardingTime;
            Console.WriteLine($"Seat {seatNumber} assigned successfully!");
        }

        // 17. Method to add baggage
        public virtual void AddBaggage(int checkedBags, int carryOnBags)
        {
            CheckedBags = checkedBags;
            CarryOnBags = carryOnBags;
            TotalBaggageWeight = (checkedBags * 23) + (carryOnBags * 7); // Standard weight limits
            Console.WriteLine($"Baggage added: {checkedBags} checked, {carryOnBags} carry-on (Total: {TotalBaggageWeight} KG)");
        }

        // 18. Method to calculate cancellation fee
        public virtual decimal CalculateCancellationFee()
        {
            TimeSpan timeUntilFlight = BookingDate - DateTime.Now;

            if (timeUntilFlight.TotalDays > 7)
                return TotalFare * 0.1m; // 10% fee
            else if (timeUntilFlight.TotalDays > 2)
                return TotalFare * 0.25m; // 25% fee
            else if (timeUntilFlight.TotalDays > 1)
                return TotalFare * 0.5m; // 50% fee
            else
                return TotalFare * 0.75m; // 75% fee
        }

        // 19. Method to get boarding pass
        public virtual void PrintBoardingPass()
        {
            Console.Clear();
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    BOARDING PASS                              ║");
            Console.WriteLine("║                    SKY-LINK AIRLINES                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\n  Passenger: {PassengerName}");
            Console.WriteLine($"  Flight: {FlightNumber}     Date: {BookingDate:dd/MM/yyyy}");
            Console.WriteLine($"  From: {Source}          To: {Destination}");
            Console.WriteLine($"  Gate: {BoardingGate ?? "TBA"}      Seat: {SeatNumber ?? "TBA"}");
            Console.WriteLine($"  Boarding: {BoardingTime ?? "Check-in"}    Class: {SeatClass}");
            Console.WriteLine($"\n  ════════════════════════════════════════════════════════");
            Console.WriteLine("  Thank you for flying with Sky-Link Airlines!");
            Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
        }

        // 20. Method to add emergency contact
        public virtual void AddEmergencyContact(string name, string relation, string number)
        {
            EmergencyContactName = name;
            EmergencyContactRelation = relation;
            EmergencyContactNumber = number;
        }
    }
}