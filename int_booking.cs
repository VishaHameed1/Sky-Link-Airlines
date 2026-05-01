using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AirlineReservationConsole
{
    public class int_booking : bookingBase
    {
        // International specific properties
        public int PnrInt { get; set; }
        public string? DateInt { get; set; }
        public string? IntDep { get; set; }
        public string? IntArr { get; set; }
        public string? Meal2 { get; set; }
        public string? ExpiryDate { get; set; }
        public string? Cvv { get; set; }

        // Additional International Booking Specific Fields
        public string? VisaType { get; set; }
        public string? VisaNumber { get; set; }
        public DateTime VisaExpiryDate { get; set; }
        public string? PassportExpiryDate { get; set; }
        public string? ResidenceCountry { get; set; }

        // Customs and Immigration
        public string? CustomsDeclaration { get; set; }
        public bool HasVisa { get; set; }
        public bool HasReturnTicket { get; set; }
        public string? PurposeOfTravel { get; set; }
        public int DurationOfStay { get; set; }

        // International Baggage
        public int CheckedBagsInt { get; set; }
        public int CarryOnBagsInt { get; set; }
        public decimal ExcessBaggageFee { get; set; }

        // Special Services
        public bool WheelchairRequired { get; set; }
        public bool UnaccompaniedMinor { get; set; }
        public bool PetInCabin { get; set; }
        public string? PetType { get; set; }
        public decimal PetFee { get; set; }

        // Lounge Access
        public bool LoungeAccess { get; set; }
        public string? LoungeName { get; set; }
        public decimal LoungeFee { get; set; }

        // Airport Transfer
        public bool AirportTransfer { get; set; }
        public string? PickupAddress { get; set; }
        public string? DropoffAddress { get; set; }
        public decimal TransferFee { get; set; }

        // Frequent Flyer Program
        public string? AirlineAlliance { get; set; }
        public int InternationalMiles { get; set; }

        // Insurance
        public bool InternationalInsurance { get; set; }
        public decimal InsuranceAmountInt { get; set; }
        public string? InsuranceProvider { get; set; }

        // Stopover Information
        public bool HasStopover { get; set; }
        public string? StopoverCity { get; set; }
        public int StopoverDuration { get; set; }
        public string? StopoverFlightNumber { get; set; }

        public int_booking()
        {
            HasVisa = false;
            HasReturnTicket = false;
            WheelchairRequired = false;
            UnaccompaniedMinor = false;
            PetInCabin = false;
            LoungeAccess = false;
            AirportTransfer = false;
            InternationalInsurance = false;
            HasStopover = false;
            ExcessBaggageFee = 0;
            Status = "Confirmed";
            BookingDate = DateTime.Now;
        }

        public void SetIntTravelDetails(DateTime date, string flight, string source, string destination)
        {
            DateInt = date.ToString("dd/MM/yyyy");
            FlightNumber = flight;
            Source = source;
            Destination = destination;
            TravelDate = DateInt;

            if (flight.Contains("Emirates"))
            {
                IntDep = "02:00";
                IntArr = "06:30";
                Price = 85000;
                FlightNumber = "EK-601";
                AirlineAlliance = "SkyTeam";
            }
            else if (flight.Contains("Qatar Airways"))
            {
                IntDep = "22:00";
                IntArr = "04:15";
                Price = 78000;
                FlightNumber = "QR-992";
                AirlineAlliance = "OneWorld";
            }
            else if (flight.Contains("British Airways"))
            {
                IntDep = "10:00";
                IntArr = "18:30";
                Price = 95000;
                FlightNumber = "BA-007";
                AirlineAlliance = "OneWorld";
            }
            else if (flight.Contains("Singapore Airlines"))
            {
                IntDep = "20:00";
                IntArr = "14:30";
                Price = 89000;
                FlightNumber = "SQ-321";
                AirlineAlliance = "Star Alliance";
            }
            else if (flight.Contains("Turkish Airlines"))
            {
                IntDep = "16:00";
                IntArr = "23:45";
                Price = 72000;
                FlightNumber = "TK-789";
                AirlineAlliance = "Star Alliance";
            }
            else
            {
                IntDep = "12:00";
                IntArr = "18:00";
                Price = 65000;
                FlightNumber = "INT-001";
                AirlineAlliance = "None";
            }

            TotalFare = (decimal)Price;
            CalculateInternationalMiles();
        }

        public void SetMeal(string meal)
        {
            Meal2 = meal;
            MealPreference = meal;

            if (!string.IsNullOrEmpty(meal))
            {
                if (meal.Contains("Premium") || meal.Contains("Business"))
                {
                    Price += 2500;
                    TotalFare += 2500;
                }
                else if (meal.Contains("Special") || meal.Contains("Dietary"))
                {
                    Price += 1500;
                    TotalFare += 1500;
                }
            }
        }

        public bool ValidatePassport()
        {
            if (string.IsNullOrEmpty(PassportNumber) || string.IsNullOrEmpty(PassportExpiryDate))
                return false;

            try
            {
                DateTime expiry = DateTime.Parse(PassportExpiryDate);
                if (expiry <= DateTime.Now.AddMonths(6))
                {
                    Console.WriteLine("Warning: Passport expires in less than 6 months!");
                    return false;
                }
                return true;
            }
            catch
            {
                Console.WriteLine("Invalid passport expiry date format!");
                return false;
            }
        }

        public bool ValidateVisa()
        {
            if (!HasVisa)
            {
                Console.WriteLine("Visa required for international travel!");
                return false;
            }

            if (VisaExpiryDate <= DateTime.Now)
            {
                Console.WriteLine("Visa has expired!");
                return false;
            }

            return true;
        }

        public void CalculateInternationalMiles()
        {
            InternationalMiles = (int)(TotalFare / 100);
            RewardPointsEarned = InternationalMiles;
        }

        public void AddStopover(string city, int hours, string connectingFlight)
        {
            HasStopover = true;
            StopoverCity = city;
            StopoverDuration = hours;
            StopoverFlightNumber = connectingFlight;

            decimal stopoverDiscount = TotalFare * 0.15m;
            TotalFare -= stopoverDiscount;
            Price = (double)TotalFare;
            Console.WriteLine($"Stopover added in {city} for {hours} hours. Discount: ${stopoverDiscount:F2}");
        }

        public void AddLoungeAccess(string loungeName)
        {
            LoungeAccess = true;
            LoungeName = loungeName;
            LoungeFee = 5000;
            TotalFare += LoungeFee;
            Price = (double)TotalFare;
        }

        public void AddAirportTransfer(string pickup, string dropoff)
        {
            AirportTransfer = true;
            PickupAddress = pickup;
            DropoffAddress = dropoff;
            TransferFee = 3000;
            TotalFare += TransferFee;
            Price = (double)TotalFare;
        }

        public void AddPetInCabin(string petType)
        {
            if (PetInCabin)
            {
                Console.WriteLine("Pet already added!");
                return;
            }

            PetInCabin = true;
            PetType = petType;
            PetFee = 15000;
            TotalFare += PetFee;
            Price = (double)TotalFare;
        }

        public void AddInternationalInsurance()
        {
            InternationalInsurance = true;
            InsuranceAmountInt = TotalFare * 0.05m;
            InsuranceProvider = "Sky-Link International Insurance";
            TotalFare += InsuranceAmountInt;
            Price = (double)TotalFare;
        }

        public void CalculateExcessBaggage(int extraBags)
        {
            const decimal feePerBag = 5000;
            ExcessBaggageFee = extraBags * feePerBag;
            TotalFare += ExcessBaggageFee;
            Price = (double)TotalFare;
        }

        public void SetInternationalBaggage(int checkedBags, int carryOnBags)
        {
            CheckedBagsInt = checkedBags;
            CarryOnBagsInt = carryOnBags;

            int allowedChecked = (SeatClass == "Business" || SeatClass == "First") ? 2 : 1;

            if (checkedBags > allowedChecked)
            {
                int extraBags = checkedBags - allowedChecked;
                CalculateExcessBaggage(extraBags);
            }

            TotalBaggageWeight = (checkedBags * 30) + (carryOnBags * 7);
        }

        public string GetInternationalBaggageAllowance()
        {
            if (SeatClass == "First")
                return "2 bags (40kg each) + 10kg carry-on";
            else if (SeatClass == "Business")
                return "2 bags (30kg each) + 8kg carry-on";
            else
                return "1 bag (23kg) + 7kg carry-on";
        }

        public decimal CancelInternationalBooking()
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

        public void InternationalCheckIn(string seatNumber, string gate)
        {
            SeatNumber = seatNumber;
            BoardingGate = gate;
            Status = "Checked-in";
            Console.WriteLine($"Check-in successful! Seat: {seatNumber}, Gate: {gate}");
        }

        public override void DisplayBookingInfo()
        {
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("              SKY-LINK INTERNATIONAL FLIGHT TICKET");
            Console.WriteLine(new string('═', 80));

            Console.WriteLine($"\n  PNR Number        : {PnrInt}");
            Console.WriteLine($"  Booking Status    : {Status ?? "N/A"}");
            Console.WriteLine($"  Booking Date      : {BookingDate:dd/MM/yyyy HH:mm}");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  PASSENGER DETAILS");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Name              : {PassengerName ?? "N/A"}");
            Console.WriteLine($"  Gender/Age        : {Gender ?? "N/A"} / {Age}");
            Console.WriteLine($"  Nationality       : {Nationality ?? "N/A"}");
            Console.WriteLine($"  Residence         : {ResidenceCountry ?? "N/A"}");
            Console.WriteLine($"  Email             : {Email ?? "N/A"}");
            Console.WriteLine($"  Contact           : {ContactNo ?? "N/A"}");
            Console.WriteLine($"  Passport No.      : {PassportNumber ?? "N/A"}");
            Console.WriteLine($"  Passport Expiry   : {PassportExpiryDate ?? "N/A"}");

            if (HasVisa)
            {
                Console.WriteLine($"  Visa Type         : {VisaType ?? "N/A"}");
                Console.WriteLine($"  Visa Number       : {VisaNumber ?? "N/A"}");
                Console.WriteLine($"  Visa Expiry       : {VisaExpiryDate:dd/MM/yyyy}");
            }

            Console.WriteLine($"  Purpose of Travel : {PurposeOfTravel ?? "N/A"}");
            Console.WriteLine($"  Duration of Stay  : {DurationOfStay} days");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  FLIGHT DETAILS");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Flight            : {FlightNumber ?? "N/A"}");
            Console.WriteLine($"  Airline Alliance  : {AirlineAlliance ?? "N/A"}");
            Console.WriteLine($"  Route             : {Source ?? "N/A"} → {Destination ?? "N/A"}");
            Console.WriteLine($"  Date              : {DateInt ?? "N/A"}");
            Console.WriteLine($"  Departure         : {IntDep ?? "N/A"} ({Source ?? "N/A"} Local Time)");
            Console.WriteLine($"  Arrival           : {IntArr ?? "N/A"} ({Destination ?? "N/A"} Local Time)");
            Console.WriteLine($"  Seat Class        : {SeatClass ?? "Economy"}");

            if (HasStopover)
            {
                Console.WriteLine("\n  STOPOVER INFORMATION");
                Console.WriteLine($"  Stopover City     : {StopoverCity ?? "N/A"}");
                Console.WriteLine($"  Duration          : {StopoverDuration} hours");
                Console.WriteLine($"  Connecting Flight : {StopoverFlightNumber ?? "N/A"}");
            }

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  BAGGAGE & SERVICES");
            Console.WriteLine(new string('─', 80));
            Console.WriteLine($"  Baggage Allowance : {GetInternationalBaggageAllowance()}");
            Console.WriteLine($"  Checked Bags      : {CheckedBagsInt} pcs");
            Console.WriteLine($"  Carry-On Bags     : {CarryOnBagsInt} pcs");
            Console.WriteLine($"  Meal Preference   : {Meal2 ?? "Standard"}");

            if (PetInCabin)
                Console.WriteLine($"  Pet in Cabin      : {PetType ?? "N/A"} (Fee: Rs.{PetFee:F2})");

            if (WheelchairRequired)
                Console.WriteLine("  Wheelchair        : Required");

            if (UnaccompaniedMinor)
                Console.WriteLine("  Unaccompanied Minor Service : Included");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  ADD-ON SERVICES");
            Console.WriteLine(new string('─', 80));
            if (LoungeAccess)
                Console.WriteLine($"  Lounge Access     : {LoungeName ?? "N/A"} (Rs.{LoungeFee:F2})");

            if (AirportTransfer)
                Console.WriteLine($"  Airport Transfer  : {PickupAddress ?? "N/A"} → {DropoffAddress ?? "N/A"} (Rs.{TransferFee:F2})");

            if (InternationalInsurance)
                Console.WriteLine($"  Travel Insurance  : {InsuranceProvider ?? "N/A"} (Rs.{InsuranceAmountInt:F2})");

            Console.WriteLine("\n" + new string('─', 80));
            Console.WriteLine("  PAYMENT SUMMARY");
            Console.WriteLine(new string('─', 80));

            decimal taxesAndSurcharge = TotalFare - (decimal)Price;
            decimal baseFare = (decimal)Price - taxesAndSurcharge;

            Console.WriteLine($"  Base Fare         : Rs.{baseFare:F2}");
            Console.WriteLine($"  Taxes & Surcharge : Rs.{taxesAndSurcharge:F2}");

            if (ExcessBaggageFee > 0)
                Console.WriteLine($"  Excess Baggage    : Rs.{ExcessBaggageFee:F2}");
            Console.WriteLine($"  Total Amount      : Rs.{TotalFare:F2}");
            Console.WriteLine($"  Payment Mode      : {ModeOfPayment ?? "N/A"}");
            Console.WriteLine($"  Card Number       : {GetMaskedCardNumber()}");

            Console.WriteLine($"\n  International Miles Earned : {InternationalMiles}");
            Console.WriteLine($"  Alliance Benefits         : Eligible for lounge, upgrades, priority boarding");

            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("  IMPORTANT INTERNATIONAL TRAVEL NOTES:");
            Console.WriteLine("  * Report 4 hours before departure for international flights");
            Console.WriteLine("  * Carry passport with minimum 6 months validity");
            Console.WriteLine("  * Valid visa required for destination country");
            Console.WriteLine("  * Complete customs declaration form");
            Console.WriteLine("  * Check COVID-19 requirements for destination");
            Console.WriteLine("  * Happy International Journey!");
            Console.WriteLine(new string('═', 80) + "\n");
        }

        public void PrintInternationalBoardingPass()
        {
            Console.Clear();
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("                  INTERNATIONAL BOARDING PASS");
            Console.WriteLine("                    SKY-LINK AIRLINES");
            Console.WriteLine(new string('═', 80));
            Console.WriteLine($"\n  Passenger: {PassengerName ?? "N/A"}");
            Console.WriteLine($"  Passport: {PassportNumber ?? "N/A"}");
            Console.WriteLine($"  PNR: {PnrInt}                        Class: {SeatClass ?? "Economy"}");
            Console.WriteLine($"  Flight: {FlightNumber ?? "N/A"}          Date: {DateInt ?? "N/A"}");
            Console.WriteLine($"  From: {Source ?? "N/A"}");
            Console.WriteLine($"  To: {Destination ?? "N/A"}");
            Console.WriteLine($"  Departure: {IntDep ?? "TBA"}              Gate: {BoardingGate ?? $"T{new Random().Next(1, 30)}"}");
            Console.WriteLine($"  Seat: {SeatNumber ?? "TBA"}");

            if (!string.IsNullOrEmpty(IntDep))
            {
                try
                {
                    Console.WriteLine($"  Boarding Time: {DateTime.Parse(IntDep).AddMinutes(-45):HH:mm}");
                }
                catch
                {
                    Console.WriteLine($"  Boarding Time: Check at counter");
                }
            }

            Console.WriteLine($"  Terminal: International Terminal");
            Console.WriteLine("\n" + new string('═', 80));
            Console.WriteLine("  Have a wonderful international journey!");
            Console.WriteLine(new string('═', 80) + "\n");
        }

        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrEmpty(PassportNumber) &&
                   !string.IsNullOrEmpty(PassportExpiryDate) &&
                   ValidatePassport() &&
                   (!HasVisa || ValidateVisa()) &&
                   !string.IsNullOrEmpty(FlightNumber) &&
                   !string.IsNullOrEmpty(IntDep) &&
                   !string.IsNullOrEmpty(IntArr);
        }

        public override string ToString()
        {
            return $"PNR: {PnrInt} | {PassengerName} | {FlightNumber} | {Source}→{Destination} | {DateInt} | {Nationality} | Status: {Status}";
        }
    }
}