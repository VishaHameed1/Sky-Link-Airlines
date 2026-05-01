using System;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;
using MongoDB.Driver;

namespace VISHA_HAMEED.Services
{
    public class GroupBookingService
    {
        private readonly DatabaseContext _context;
        private readonly BookingService _bookingService;
        private readonly NotificationService _notificationService;

        public GroupBookingService(DatabaseContext context, BookingService bookingService, NotificationService notificationService)
        {
            _context = context;
            _bookingService = bookingService;
            _notificationService = notificationService;
        }

        public GroupBooking CreateGroupBooking(string contactPerson, string contactEmail, string contactPhone,
            List<GroupMember> members, string flightId, decimal baseFare)
        {
            decimal groupDiscount = CalculateGroupDiscount(members.Count);
            decimal totalAmount = (baseFare * members.Count) * (1 - groupDiscount / 100);

            var groupId = GenerateGroupId();

            var groupBooking = new GroupBooking
            {
                GroupId = groupId,
                ContactPerson = contactPerson,
                ContactEmail = contactEmail,
                ContactPhone = contactPhone,
                NumberOfPassengers = members.Count,
                Members = members,
                GroupDiscount = groupDiscount,
                TotalAmount = totalAmount,
                FlightId = flightId,
                BookingDate = DateTime.Now,
                Status = "Pending"
            };

            _context.GroupBookings.InsertOne(groupBooking);

            _notificationService.SendEmail(contactEmail, "Group Booking Initiated",
                $"Dear {contactPerson},\n\nYour group booking for {members.Count} passengers has been initiated.\nGroup ID: {groupId}\nTotal Amount: ${totalAmount}\n\nWe will contact you shortly for confirmation.");

            return groupBooking;
        }

        public void ConfirmGroupBooking(string groupId, int[] pnrs)
        {
            var groupBooking = _context.GroupBookings.Find(g => g.GroupId == groupId).FirstOrDefault();
            if (groupBooking == null) return;

            for (int i = 0; i < groupBooking.Members.Count && i < pnrs.Length; i++)
            {
                groupBooking.Members[i].PNR = pnrs[i];
            }

            groupBooking.Status = "Confirmed";
            _context.GroupBookings.ReplaceOne(g => g.GroupId == groupId, groupBooking);

            _notificationService.SendEmail(groupBooking.ContactEmail, "Group Booking Confirmed",
                $"Dear {groupBooking.ContactPerson},\n\nYour group booking has been confirmed!\nGroup ID: {groupId}\nPNRs: {string.Join(", ", pnrs)}\n\nThank you for choosing Sky-Link Airlines!");
        }

        private decimal CalculateGroupDiscount(int passengerCount)
        {
            if (passengerCount >= 50) return 20;
            if (passengerCount >= 25) return 15;
            if (passengerCount >= 10) return 10;
            if (passengerCount >= 5) return 5;
            return 0;
        }

        private string GenerateGroupId()
        {
            return "GRP" + DateTime.Now.ToString("yyyyMMdd") + new Random().Next(1000, 9999);
        }

        public GroupBooking GetGroupBooking(string groupId)
        {
            return _context.GroupBookings.Find(g => g.GroupId == groupId).FirstOrDefault();
        }
    }
}