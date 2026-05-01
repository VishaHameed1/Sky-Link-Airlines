
# ✈️ Sky-Link Airlines Reservation System

[![.NET Version](https://img.shields.io/badge/.NET-6.0-blue.svg)](https://dotnet.microsoft.com/)
[![MongoDB](https://img.shields.io/badge/MongoDB-6.0-green.svg)](https://www.mongodb.com/)
[![Docker](https://img.shields.io/badge/Docker-20.10-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

## 📋 Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [System Architecture](#system-architecture)
- [OOP Concepts Implementation](#oop-concepts-implementation)
- [Installation Guide](#installation-guide)
- [Docker Setup](#docker-setup)
- [Database Schema](#database-schema)
- [User Guide](#user-guide)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Future Enhancements](#future-enhancements)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## 🚀 Project Overview

Sky-Link Airlines Reservation System is a comprehensive, feature-rich airline booking platform developed using **C# .NET 6.0** and **MongoDB**. The system provides complete flight booking management for both domestic and international travel, with integrated loyalty program, real-time check-in, email notifications, and administrative controls.

### 🎯 Key Highlights
- ✅ Complete implementation of **Object-Oriented Programming** concepts (Inheritance, Polymorphism, Encapsulation, Abstraction)
- ✅ **3-tier architecture** for better separation of concerns
- ✅ **MongoDB** for flexible and scalable data storage
- ✅ **Docker containerization** for easy deployment
- ✅ **Real-time email notifications** using SMTP
- ✅ **Loyalty program** with miles accumulation and tier benefits
- ✅ **Web check-in** functionality
- ✅ **Full admin dashboard** with CRUD operations

## ✨ Features

### 👤 Customer Features

| Feature | Description |
|---------|-------------|
| 🔐 **User Authentication** | Secure login/registration with SHA256 password encryption |
| 🏠 **Domestic Booking** | Book domestic flights with seat and meal selection |
| 🌍 **International Booking** | Book international flights with passport/visa validation |
| 💰 **Promo Codes** | Apply discount codes for special offers |
| 🎯 **Loyalty Program** | Earn miles on every booking, redeem for discounts |
| 🔍 **PNR Search** | Check booking status anytime using PNR number |
| ❌ **Cancel Booking** | Cancel bookings with automated refund calculation |
| 💬 **Feedback System** | Rate and review your travel experience |
| 📧 **Email Notifications** | Receive booking confirmations and updates |
| 🎟️ **View My Bookings** | List all bookings associated with email |
| 📊 **Loyalty Status** | Check accumulated miles and current tier benefits |
| 💻 **Web Check-in** | Online check-in 48 hours before departure |

### 🔧 Admin Features

| Feature | Description |
|---------|-------------|
| ✈️ **Flight Schedule Management** | Complete CRUD operations for flights |
| 📋 **Booking Management** | View, update, and cancel all bookings |
| 👥 **User Management** | View all registered users |
| 🎉 **Promotion Management** | Create, update, delete discount codes |
| 👑 **Loyalty Management** | Track top loyalty members and their tiers |
| 📊 **Real-time Flight Status** | Update flight statuses (On Time, Delayed, Boarding, etc.) |
| 📈 **Generate Reports** | Analytics dashboard with revenue and occupancy reports |
| 🗑️ **Delete/Cancel Booking** | Admin forced cancellation with reason |
| 📜 **Cancelled History** | Track all cancelled bookings with refund details |
| 💬 **Customer Feedback** | View and analyze customer ratings |
| 📤 **Export Data** | Export reports to CSV format |
| 📉 **System Statistics** | Comprehensive database and revenue metrics |

## 🛠️ Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **Programming Language** | C# | 10.0 (.NET 6.0) |
| **Database** | MongoDB | 6.0 |
| **ORM/Database Driver** | MongoDB.Driver | 2.19.0 |
| **Email Service** | MailKit & MimeKit | Latest |
| **Containerization** | Docker & Docker Compose | 20.10+ |
| **Version Control** | Git & GitHub | Latest |
| **IDE** | Visual Studio 2022 / VS Code | - |
| **Operating System** | Windows / Linux / macOS | - |

## 🏗️ System Architecture

### 3-Tier Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER (UI)                  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  MainMenu.cs │  │CustomerPortal│  │ AdminPortal  │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 BUSINESS LOGIC LAYER (Services)             │
│  ┌───────────┐ ┌──────────┐ ┌──────────┐ ┌────────────┐   │
│  │ Booking   │ │  Auth    │ │ Loyalty  │ │ Promotion  │   │
│  │ Service   │ │ Service  │ │ Service  │ │ Service    │   │
│  └───────────┘ └──────────┘ └──────────┘ └────────────┘   │
│  ┌───────────┐ ┌──────────┐ ┌──────────┐                  │
│  │ CheckIn   │ │Notification│Analytics │                  │
│  │ Service   │ │ Service  │ │ Service  │                  │
│  └───────────┘ └──────────┘ └──────────┘                  │
└─────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   DATA ACCESS LAYER (Data)                  │
│                    ┌──────────────────┐                     │
│                    │ DatabaseContext  │                     │
│                    └──────────────────┘                     │
└─────────────────────────────────────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      MONGODB DATABASE                        │
│  DomesticBookings | InternationalBookings | Users | etc.   │
└─────────────────────────────────────────────────────────────┘
```

### Class Hierarchy (UML)

```
                    ┌─────────────────────┐
                    │    bookingBase      │
                    │   (Abstract Class)  │
                    ├─────────────────────┤
                    │ - disposedValue     │
                    │ + PassengerName     │
                    │ + Email             │
                    │ + TotalFare         │
                    │ + Status            │
                    ├─────────────────────┤
                    │ + DisplayBookingInfo│
                    │ + CalcCancellationFee│
                    └──────────┬──────────┘
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
    ┌─────────────────┐  ┌─────────────────┐
    │   dom_booking   │  │   int_booking   │
    ├─────────────────┤  ├─────────────────┤
    │ + PNR           │  │ + PnrInt        │
    │ + DepartureTime │  │ + PassportNumber│
    │ + ArrivalTime   │  │ + VisaType      │
    │ + SeatPreference│  │ + PurposeOfTravel│
    ├─────────────────┤  ├─────────────────┤
    │ + SetTravelDetails│ │ + SetIntTravelDetails│
    │ + SetMeal()     │  │ + ValidatePassport│
    └─────────────────┘  └─────────────────┘
```

## 🎯 OOP Concepts Implementation

### 1. Encapsulation (Data Hiding)
```csharp
public class bookingBase
{
    private bool disposedValue = false;  // Private field - cannot access directly
    
    public string? PassengerName { get; set; }  // Public property with controlled access
    
    public string GetMaskedCardNumber()  // Public method hides implementation
    {
        if (string.IsNullOrEmpty(CardNumber) || CardNumber.Length < 4)
            return "****";
        return "**** **** **** " + CardNumber.Substring(CardNumber.Length - 4);
    }
}
```

### 2. Inheritance (Code Reusability)
```csharp
// Base class with common properties
public class bookingBase { ... }

// Derived classes inherit all base properties
public class dom_booking : bookingBase { ... }  // Domestic booking
public class int_booking : bookingBase { ... }  // International booking
```

### 3. Polymorphism (Method Overriding)
```csharp
// Base class virtual method
public virtual void DisplayBookingInfo() { }

// Different implementations for different booking types
public override void DisplayBookingInfo()  // Domestic version
{
    Console.WriteLine($"PNR: {PNR}");
    Console.WriteLine($"Departure: {DepartureTime}");
}

public override void DisplayBookingInfo()  // International version
{
    Console.WriteLine($"PNR: {PnrInt}");
    Console.WriteLine($"Passport: {PassportNumber}");
}
```

### 4. Abstraction (Hiding Implementation)
```csharp
// Interface defines WHAT, not HOW
public interface INotificationService
{
    void SendBookingConfirmation(...);
}

// Implementation hides complex SMTP logic
public class NotificationService : INotificationService
{
    // Complex SMTP implementation hidden from caller
    public void SendBookingConfirmation(...) { ... }
}
```

## 📥 Installation Guide

### Prerequisites

| Requirement | Version | Download Link |
|-------------|---------|---------------|
| .NET SDK | 6.0 or later | [Download](https://dotnet.microsoft.com/download) |
| MongoDB | 6.0 or later | [Download](https://www.mongodb.com/try/download/community) |
| Git | Latest | [Download](https://git-scm.com/downloads) |
| Docker (Optional) | 20.10+ | [Download](https://www.docker.com/products/docker-desktop) |

### Step-by-Step Installation

#### 1. Clone the Repository
```bash
git clone https://github.com/YOUR_USERNAME/Sky-Link-Airlines.git
cd Sky-Link-Airlines
```

#### 2. Install NuGet Packages
```bash
dotnet add package MongoDB.Driver
dotnet add package MailKit
dotnet add package MimeKit
```

#### 3. Configure Database Connection
Update `Data/DatabaseContext.cs` if needed:
```csharp
var client = new MongoClient("mongodb://localhost:27017");
var database = client.GetDatabase("AirlineDB");
```

#### 4. Configure Email Service (Optional)
Update `Services/NotificationService.cs` with your credentials:
```csharp
private readonly string senderEmail = "your-email@gmail.com";
private readonly string senderPassword = "your-app-password";
```

#### 5. Build and Run
```bash
dotnet build
dotnet run
```

## 🐳 Docker Setup

### Using Docker Compose (Recommended)

1. **Start all services:**
```bash
docker-compose up -d
```

2. **View logs:**
```bash
docker-compose logs -f
```

3. **Stop services:**
```bash
docker-compose down
```

4. **Stop and remove volumes:**
```bash
docker-compose down -v
```

### Services Available

| Service | URL | Credentials |
|---------|-----|-------------|
| MongoDB | localhost:27017 | admin/admin123 |
| MongoDB Express | http://localhost:8081 | admin/admin123 |
| Application | Attach to container console | - |

## 🗄️ Database Schema

### Collections Structure

#### DomesticBookings
```json
{
  "PNR": 123456,
  "PassengerName": "John Doe",
  "Email": "john@example.com",
  "Age": 30,
  "FlightNumber": "AI-202",
  "Source": "Karachi",
  "Destination": "Lahore",
  "TravelDate": "15/05/2024",
  "TotalFare": 120.00,
  "Status": "Confirmed",
  "SeatClass": "Economy",
  "MealPreference": "Non-Veg"
}
```

#### InternationalBookings
```json
{
  "PnrInt": 789012,
  "PassengerName": "Jane Smith",
  "PassportNumber": "AB1234567",
  "Nationality": "Pakistani",
  "VisaType": "Tourist",
  "PurposeOfTravel": "Tourism",
  "FlightNumber": "EK-601",
  "Source": "Karachi",
  "Destination": "Dubai",
  "TotalFare": 450.00
}
```

#### Users
```json
{
  "Username": "john_doe",
  "PasswordHash": "sha256_hash",
  "Email": "john@example.com",
  "FullName": "John Doe",
  "Role": "Customer",
  "CreatedAt": "2024-01-01T00:00:00Z"
}
```

## 📖 User Guide

### Customer Portal

#### Login/Register
1. Launch the application
2. Select option `1` (Customer Portal)
3. Choose `1` for Login or `2` for Registration
4. Enter your credentials

#### Booking a Flight
1. After login, select `2` for Domestic or `3` for International
2. Choose a flight from available options
3. Enter passenger details
4. Select seat preference and meal preference
5. Apply promo code (if any)
6. Complete payment
7. **Save your PNR number** for future reference

#### Web Check-in
1. Select option `9` (Web Check-in)
2. Enter your PNR number
3. Select seat from available seats
4. Download boarding pass

#### Check Loyalty Status
1. Select option `8` (View My Loyalty Status)
2. View your miles, tier, and benefits
3. Apply miles discount during next booking

### Admin Portal

#### Admin Login
1. From main menu, select option `2`
2. Enter password: `admin123`

#### Manage Flights
1. Select option `1` (Manage Live Schedules)
2. Choose:
   - `1` to Add New Flight
   - `2` to Update Flight
   - `3` to Delete Flight
   - `4` to Update Flight Status

#### View Reports
1. Select option `6` (Generate Reports)
2. View booking statistics, revenue reports, and occupancy rates
3. Select option `12` to export data to CSV

## 📚 API Documentation

### Database Context Methods

```csharp
// Find booking by PNR
var booking = _context.DomesticBookings.Find(d => d.PNR == pnr).FirstOrDefault();

// Get all bookings
var allBookings = _context.DomesticBookings.Find(_ => true).ToList();

// Insert new booking
_context.DomesticBookings.InsertOne(booking);

// Update booking
_context.DomesticBookings.ReplaceOne(d => d.PNR == pnr, updatedBooking);

// Delete booking
_context.DomesticBookings.DeleteOne(d => d.PNR == pnr);
```

### Service Methods

#### BookingService
| Method | Description |
|--------|-------------|
| `BookDomesticFlight()` | Books a domestic flight |
| `BookInternationalFlight()` | Books an international flight |
| `SearchDomesticBooking(pnr)` | Searches booking by PNR |
| `CancelDomesticBooking(pnr)` | Cancels booking with refund |

#### LoyaltyService
| Method | Description |
|--------|-------------|
| `AddMiles(pnr, miles)` | Adds miles to user account |
| `RedeemMiles(email, miles, desc)` | Redeems miles for discount |
| `GetLoyaltyDetails(email)` | Returns loyalty account details |
| `GetTierBenefits(tier)` | Returns benefits for loyalty tier |

## 🧪 Testing

### Test Results

| Test Case | Expected Result | Status |
|-----------|----------------|--------|
| Valid Login | Login successful | ✅ PASS |
| Invalid Login | Login failed | ✅ PASS |
| User Registration | Account created | ✅ PASS |
| Domestic Booking | Booking confirmed | ✅ PASS |
| International Booking | Booking confirmed | ✅ PASS |
| Promo Code | Discount applied | ✅ PASS |
| PNR Search | Booking found | ✅ PASS |
| Cancel Booking | Refund calculated | ✅ PASS |
| Email Notification | Email received | ✅ PASS |
| Web Check-in | Check-in successful | ✅ PASS |

## 📁 Project Structure

```
Sky-Link-Airlines/
├── 📁 Models/
│   ├── FlightSchedule.cs
│   ├── User.cs
│   ├── LoyaltyProgram.cs
│   ├── CheckIn.cs
│   ├── CancelledBooking.cs
│   ├── Feedback.cs
│   ├── Promotion.cs
│   └── ...
├── 📁 Data/
│   └── DatabaseContext.cs
├── 📁 Services/
│   ├── BookingService.cs
│   ├── AuthService.cs
│   ├── LoyaltyService.cs
│   ├── NotificationService.cs
│   ├── PromotionService.cs
│   ├── CheckInService.cs
│   ├── AdminService.cs
│   ├── FeedbackService.cs
│   ├── AnalyticsService.cs
│   └── ExportService.cs
├── 📁 UI/
│   ├── MainMenu.cs
│   ├── CustomerPortal.cs
│   └── AdminPortal.cs
├── 📄 Program.cs
├── 📄 bookingBase.cs
├── 📄 dom_booking.cs
├── 📄 int_booking.cs
├── 🐳 Dockerfile
├── 🐳 docker-compose.yml
├── 📄 .gitignore
└── 📄 README.md
```

## 🔮 Future Enhancements

- [ ] Mobile application for Android and iOS
- [ ] Payment gateway integration (Stripe, PayPal, EasyPaisa, JazzCash)
- [ ] Real-time flight tracking with GPS and maps
- [ ] AI-based price prediction and dynamic pricing
- [ ] Multi-language support (Urdu, Arabic, English)
- [ ] Biometric authentication (fingerprint, face recognition)
- [ ] Chatbot integration for 24/7 customer support
- [ ] Integration with hotel booking and car rental services
- [ ] WhatsApp notifications
- [ ] Voice-based booking system
- [ ] AR-based seat selection preview

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Coding Standards
- Follow C# coding conventions
- Add comments for complex logic
- Write meaningful commit messages
- Update documentation as needed

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2024 Sky-Link Airlines

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions...
```

## 📞 Contact

### Project Maintainer
- **Name:** VISHA HAMEED
- **Email:** vishahameed666@gmail.com
- **Roll Number:** 232201044
- **University:** Institute of Space and Technology, Islamabad

### Customer Support
- **Phone:** +92-21-111-000-786
- **WhatsApp:** +92-300-1234567
- **Email:** support@skylinkairlines.com
- **Facebook:** @SkyLinkAirlines
- **Twitter:** @SkyLinkAir
- **Instagram:** @skylink_airlines

## 🙏 Acknowledgments

- **Sir Uzair Janjua** - Project Supervisor
- **Institute of Space and Technology, Islamabad** - Institutional Support
- **Microsoft** - For .NET Framework and C#
- **MongoDB, Inc.** - For NoSQL Database
- **Open Source Community** - For various libraries and tools

## 📊 Badges

![GitHub stars](https://img.shields.io/github/stars/YOUR_USERNAME/Sky-Link-Airlines?style=social)
![GitHub forks](https://img.shields.io/github/forks/YOUR_USERNAME/Sky-Link-Airlines?style=social)
![GitHub watchers](https://img.shields.io/github/watchers/YOUR_USERNAME/Sky-Link-Airlines?style=social)
![GitHub last commit](https://img.shields.io/github/last-commit/YOUR_USERNAME/Sky-Link-Airlines)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/YOUR_USERNAME/Sky-Link-Airlines)

---

## ⭐ Show Your Support

If you found this project helpful, please give it a ⭐ on GitHub!

---

**Made with ❤️ by VISHA HAMEED | Sky-Link Airlines**

*Last Updated: May 2024*


