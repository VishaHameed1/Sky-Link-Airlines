// Create database and collections
db = db.getSiblingDB('AirlineDB');

// Create collections
db.createCollection('DomesticBookings');
db.createCollection('InternationalBookings');
db.createCollection('FlightSchedules');
db.createCollection('CancelledBookings');
db.createCollection('Feedbacks');
db.createCollection('Promotions');
db.createCollection('Users');
db.createCollection('FlightStatuses');
db.createCollection('SeatMaps');
db.createCollection('GroupBookings');
db.createCollection('LoyaltyPrograms');
db.createCollection('CheckIns');
db.createCollection('AirportServices');
db.createCollection('RefundRequests');
db.createCollection('TravelPackages');
db.createCollection('ChatMessages');
db.createCollection('ChatSessions');
db.createCollection('FlightAlerts');
db.createCollection('PaymentTransactions');
db.createCollection('NotificationLogs');

// Create indexes for better performance
db.DomesticBookings.createIndex({ PNR: 1 }, { unique: true });
db.InternationalBookings.createIndex({ PnrInt: 1 }, { unique: true });
db.Users.createIndex({ Email: 1 }, { unique: true });
db.Users.createIndex({ Username: 1 }, { unique: true });
db.FlightSchedules.createIndex({ FlightID: 1 }, { unique: true });
db.Promotions.createIndex({ PromoCode: 1 }, { unique: true });

// Insert initial flight data
db.FlightSchedules.insertMany([
    {
        FlightID: "AI-202",
        Type: "Domestic",
        Route: "Karachi -> Lahore",
        Boarding: "09:00 AM",
        Landing: "10:45 AM",
        AvailableSeats: "50",
        Price: 120.00,
        Gate: "A1",
        Terminal: "Domestic Terminal 1"
    },
    {
        FlightID: "IG-405",
        Type: "Domestic",
        Route: "Islamabad -> Karachi",
        Boarding: "02:30 PM",
        Landing: "04:15 PM",
        AvailableSeats: "45",
        Price: 110.00,
        Gate: "A2",
        Terminal: "Domestic Terminal 1"
    },
    {
        FlightID: "EK-601",
        Type: "International",
        Route: "Karachi -> Dubai",
        Boarding: "03:00 AM",
        Landing: "05:30 AM",
        AvailableSeats: "100",
        Price: 450.00,
        Gate: "B1",
        Terminal: "International Terminal"
    },
    {
        FlightID: "QA-992",
        Type: "International",
        Route: "Islamabad -> Doha",
        Boarding: "11:15 PM",
        Landing: "02:45 AM",
        AvailableSeats: "85",
        Price: 500.00,
        Gate: "B2",
        Terminal: "International Terminal"
    }
]);

// Insert initial promotions
db.Promotions.insertMany([
    {
        Title: "Summer Sale",
        Description: "Get flat 20% off on all domestic flights",
        PromoCode: "SUMMER20",
        DiscountPercentage: 20,
        ValidUntil: new Date(new Date().setMonth(new Date().getMonth() + 1))
    },
    {
        Title: "International Offer",
        Description: "15% off on international bookings",
        PromoCode: "INTL15",
        DiscountPercentage: 15,
        ValidUntil: new Date(new Date().setMonth(new Date().getMonth() + 2))
    }
]);

// Create admin user (password: admin123)
db.Users.insertOne({
    Username: "admin",
    Email: "admin@skylink.com",
    FullName: "System Administrator",
    Phone: "+92-21-111-000-786",
    Role: "Admin",
    PasswordHash: "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // SHA256 of "admin123"
    IsActive: true,
    CreatedAt: new Date(),
    FailedLoginAttempts: 0
});

print("Database initialized successfully!");