using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AirlineReservationConsole
{
    public class dom_booking : bookingBase
    {
        public int PNR { get; set; }
        public string? DepartureTime { get; set; }
        public string? ArrivalTime { get; set; }
        public string? MealPlan { get; set; }

        // Additional Domestic Booking Specific Fields
        public string? BoardingPoint { get; set; }
        public string? DroppingPoint { get; set; }
        public string? SeatPreference { get; set; }
        public bool IsSeniorCitizen { get; set; }
        public decimal DiscountApplied { get; set; }
        public string? IDProofType { get; set; }
        public string? IDProofNumber { get; set; }
        public bool IsRoundTrip { get; set; }
        public string? ReturnFlightNumber { get; set; }
        public string? ReturnDepartureTime { get; set; }
        public string? ReturnArrivalTime { get; set; }
        public string? ReturnDate { get; set; }

        // Loyalty Points for Domestic Bookings
        public int LoyaltyPointsEarned { get; set; }
        public int LoyaltyPointsRedeemed { get; set; }

        // Insurance
        public bool TravelInsurance { get; set; }
        public decimal InsuranceAmount { get; set; }

        public dom_booking()
        {
            IsRoundTrip = false;
            TravelInsurance = false;
            DiscountApplied = 0;
            LoyaltyPointsEarned = 0;
            LoyaltyPointsRedeemed = 0;
            Status = "Confirmed";
            BookingDate = DateTime.Now;
        }

        public void SetTravelDetails(DateTime date, string flight, string source, string destination)
        {
            TravelDate = date.ToString("dd/MM/yyyy");
            FlightNumber = flight;
            Source = source;
            Destination = destination;
            BoardingPoint = source;
            DroppingPoint = destination;

            if (flight.Contains("SpiceJet"))
            {
                DepartureTime = "19:00";
                ArrivalTime = "22:05";
                Price = 6000;
                FlightNumber = "SG-202";
            }
            else if (flight.Contains("Air India"))
            {
                DepartureTime = "08:00";
                ArrivalTime = "11:05";
                Price = 7500;
                FlightNumber = "AI-405";
            }
            else if (flight.Contains("IndiGo"))
            {
                DepartureTime = "14:00";
                ArrivalTime = "17:05";
                Price = 5500;
                FlightNumber = "6E-118";
            }
            else if (flight.Contains("GoAir"))
            {
                DepartureTime = "06:30";
                ArrivalTime = "09:15";
                Price = 4800;
                FlightNumber = "G8-301";
            }
            else if (flight.Contains("Vistara"))
            {
                DepartureTime = "11:00";
                ArrivalTime = "14:30";
                Price = 8200;
                FlightNumber = "UK-955";
            }
            else
            {
                DepartureTime = "12:00";
                ArrivalTime = "15:00";
                Price = 5000;
                FlightNumber = "DOM-001";
            }

            TotalFare = (decimal)Price;
            CalculateRewardPoints();
        }

        public void SetMeal(string meal)
        {
            MealPlan = meal;
            MealPreference = meal;

            if (!string.IsNullOrEmpty(meal))
            {
                if (meal.Contains("Non-Veg") || meal.Contains("Non-Vegetarian"))
                {
                    Price += 450;
                    TotalFare += 450;
                }
                else if (meal.Contains("Premium"))
                {
                    Price += 750;
                    TotalFare += 750;
                }
            }
        }

        public void SetSeatPreference(string preference)
        {
            SeatPreference = preference;
            SeatClass = preference;
        }

        public void ApplySeniorCitizenDiscount()
        {
            if (IsSeniorCitizen && Age >= 60)
            {
                DiscountApplied = TotalFare * 0.15m;
                TotalFare -= DiscountApplied;
                Price = (double)TotalFare;
                Console.WriteLine($"Senior Citizen Discount Applied: Rs.{DiscountApplied:F2}");
            }
        }

        public void AddRoundTrip(string returnFlight, DateTime returnDateTime, string returnSource, string returnDest)
        {
            IsRoundTrip = true;
            ReturnFlightNumber = returnFlight;
            ReturnDepartureTime = returnDateTime.ToString("HH:mm");
            ReturnArrivalTime = returnDateTime.AddHours(3).ToString("HH:mm");
            ReturnDate = returnDateTime.ToString("dd/MM/yyyy");

            decimal returnFare = TotalFare * 0.8m;
            TotalFare += returnFare;
            Price = (double)TotalFare;
        }

        public void AddTravelInsurance()
        {
            TravelInsurance = true;
            InsuranceAmount = 500;
            TotalFare += InsuranceAmount;
            Price = (double)TotalFare;
        }

        public void RedeemLoyaltyPoints(int points)
        {
            if (points <= LoyaltyPointsEarned)
            {
                decimal discount = points * 0.5m;
                DiscountApplied += discount;
                TotalFare -= discount;
                LoyaltyPointsRedeemed = points;
                LoyaltyPointsEarned -= points;
                Price = (double)TotalFare;
                Console.WriteLine($"Redeemed {points} points. Discount: Rs.{discount:F2}");
            }
            else
            {
                Console.WriteLine("Insufficient loyalty points!");
            }
        }

        public new void CalculateRewardPoints()
        {
            LoyaltyPointsEarned = (int)(TotalFare / 10);
            RewardPointsEarned = LoyaltyPointsEarned;
        }

        public bool ValidateIDProof()
        {
            if (string.IsNullOrEmpty(IDProofNumber))
                return false;

            switch (IDProofType?.ToLower())
            {
                case "aadhar":
                    return IDProofNumber.Length == 12;
                case "pan":
                    return IDProofNumber.Length == 10;
                case "driving license":
                    return IDProofNumber.Length >= 10 && IDProofNumber.Length <= 16;
                default:
                    return true;
            }
        }

        public string GetBaggageAllowance()
        {
            if (SeatClass == "Business" || SeatClass == "First")
                return "25 KG Checked + 8 KG Carry-on";
            else
                return "15 KG Checked + 7 KG Carry-on";
        }

        public decimal CancelBooking()
        {
            if (Status == "Cancelled")
            {
                Console.WriteLine("Booking already cancelled!");
                return 0;
            }

            decimal refundAmount = TotalFare - CalculateCancellationFee();
            Status = "Cancelled";
            return refundAmount;
        }

        public override void DisplayBookingInfo()
        {
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("                    SKY-LINK DOMESTIC FLIGHT TICKET");
            Console.WriteLine(new string('═', 80));

            Console.WriteLine($"\n  PNR Number        : {PNR}");
            Console.WriteLine($"  Booking Status    : {Status}");
            Console.WriteLine($"  Booking Date      : {BookingDate:dd/MM/yyyy HH:mm}");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  PASSENGER DETAILS");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Name              : {PassengerName ?? "N/A"}");
            Console.WriteLine($"  Gender/Age        : {Gender ?? "N/A"} / {Age}");
            Console.WriteLine($"  Email             : {Email ?? "N/A"}");
            Console.WriteLine($"  Contact           : {ContactNo ?? "N/A"}");
            Console.WriteLine($"  ID Proof          : {IDProofType ?? "N/A"} - {IDProofNumber ?? "N/A"}");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  FLIGHT DETAILS");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Flight            : {FlightNumber ?? "N/A"}");
            Console.WriteLine($"  Route             : {Source ?? "N/A"} → {Destination ?? "N/A"}");
            Console.WriteLine($"  Date              : {TravelDate ?? "N/A"}");
            Console.WriteLine($"  Departure         : {DepartureTime ?? "N/A"} from {BoardingPoint ?? "N/A"}");
            Console.WriteLine($"  Arrival           : {ArrivalTime ?? "N/A"} at {DroppingPoint ?? "N/A"}");
            Console.WriteLine($"  Seat Preference   : {SeatPreference ?? "Standard"}");
            Console.WriteLine($"  Seat Class        : {SeatClass ?? "Economy"}");

            if (IsRoundTrip)
            {
                Console.WriteLine("\n  RETURN FLIGHT DETAILS");
                Console.WriteLine($"  Return Flight     : {ReturnFlightNumber ?? "N/A"}");
                Console.WriteLine($"  Return Date       : {ReturnDate ?? "N/A"}");
                Console.WriteLine($"  Return Departure  : {ReturnDepartureTime ?? "N/A"}");
                Console.WriteLine($"  Return Arrival    : {ReturnArrivalTime ?? "N/A"}");
            }

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  BAGGAGE & MEALS");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Baggage Allowance : {GetBaggageAllowance()}");
            Console.WriteLine($"  Meal Preference   : {MealPlan ?? "Standard"}");

            if (TravelInsurance)
            {
                Console.WriteLine($"  Travel Insurance  : Included (Rs.{InsuranceAmount:F2})");
            }

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  PAYMENT SUMMARY");
            Console.WriteLine(new string('─', 80));

            decimal addOns = TotalFare - (decimal)Price + DiscountApplied;
            Console.WriteLine($"  Base Fare         : Rs.{(decimal)Price - addOns:F2}");
            Console.WriteLine($"  Add-ons           : Rs.{addOns:F2}");
            if (DiscountApplied > 0)
                Console.WriteLine($"  Discount          : -Rs.{DiscountApplied:F2}");
            Console.WriteLine($"  Total Amount      : Rs.{TotalFare:F2}");
            Console.WriteLine($"  Payment Mode      : {ModeOfPayment ?? "N/A"}");
            Console.WriteLine($"  Card Number       : {GetMaskedCardNumber()}");

            if (LoyaltyPointsEarned > 0)
            {
                Console.WriteLine($"\n  Loyalty Points Earned : {LoyaltyPointsEarned}");
                Console.WriteLine($"  Reward Value          : Rs.{LoyaltyPointsEarned / 2:F2}");
            }

            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("  IMPORTANT NOTES:");
            Console.WriteLine("  * Please report 2 hours before departure");
            Console.WriteLine("  * Carry valid ID proof for verification");
            Console.WriteLine("  * Web check-in available 48 hours before departure");
            Console.WriteLine("  * Happy Journey!");
            Console.WriteLine(new string('═', 80) + "\n");
        }

        public new void PrintBoardingPass()
        {
            Console.Clear();
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("                        BOARDING PASS");
            Console.WriteLine("                        SKY-LINK AIRLINES");
            Console.WriteLine("                        DOMESTIC FLIGHT");
            Console.WriteLine(new string('═', 80));
            Console.WriteLine($"\n  Passenger: {PassengerName ?? "N/A"}");
            Console.WriteLine($"  PNR: {PNR}                        Class: {SeatClass ?? "Economy"}");
            Console.WriteLine($"  Flight: {FlightNumber ?? "N/A"}          Date: {TravelDate ?? "N/A"}");
            Console.WriteLine($"  From: {Source ?? "N/A"} ({BoardingPoint ?? "N/A"})");
            Console.WriteLine($"  To: {Destination ?? "N/A"} ({DroppingPoint ?? "N/A"})");
            Console.WriteLine($"  Departure: {DepartureTime ?? "TBA"}      Gate: A{new Random().Next(1, 20)}");
            Console.WriteLine($"  Seat: {SeatNumber ?? "TBA"}");

            if (!string.IsNullOrEmpty(DepartureTime))
            {
                try
                {
                    Console.WriteLine($"  Boarding Time: {DateTime.Parse(DepartureTime).AddMinutes(-30):HH:mm}");
                }
                catch
                {
                    Console.WriteLine($"  Boarding Time: Check at counter");
                }
            }

            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("  Thank you for flying with Sky-Link Airlines!");
            Console.WriteLine(new string('═', 80) + "\n");
        }

        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrEmpty(FlightNumber) &&
                   !string.IsNullOrEmpty(DepartureTime) &&
                   !string.IsNullOrEmpty(ArrivalTime) &&
                   ValidateIDProof();
        }

        public override string ToString()
        {
            return $"PNR: {PNR} | {PassengerName} | {FlightNumber} | {Source}→{Destination} | {TravelDate} | Status: {Status}";
        }
    }
}