using System;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;
using MongoDB.Driver;

namespace VISHA_HAMEED.Services
{
    public class ChatService
    {
        private readonly DatabaseContext _context;
        private readonly NotificationService _notificationService;

        public ChatService(DatabaseContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public ChatSession StartChatSession(string customerId, string customerEmail)
        {
            var session = new ChatSession
            {
                SessionId = GenerateSessionId(),
                CustomerId = customerId,
                CustomerEmail = customerEmail,
                StartedAt = DateTime.Now,
                Status = "Active",
                MessageCount = 0
            };

            _context.ChatSessions.InsertOne(session);
            return session;
        }

        public ChatMessage SendMessage(string sessionId, string senderId, string senderName, string message, bool isFromCustomer = true)
        {
            var chatMessage = new ChatMessage
            {
                MessageId = GenerateMessageId(),
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = isFromCustomer ? "Support" : senderId,
                Message = message,
                Timestamp = DateTime.Now,
                IsRead = false,
                IsFromCustomer = isFromCustomer,
                SessionId = sessionId
            };

            _context.ChatMessages.InsertOne(chatMessage);

            // Update session message count
            var session = _context.ChatSessions.Find(s => s.SessionId == sessionId).FirstOrDefault();
            if (session != null)
            {
                session.MessageCount++;
                _context.ChatSessions.ReplaceOne(s => s.SessionId == sessionId, session);
            }

            return chatMessage;
        }

        public List<ChatMessage> GetMessages(string sessionId)
        {
            return _context.ChatMessages.Find(m => m.SessionId == sessionId)
                .SortBy(m => m.Timestamp).ToList();
        }

        public void MarkMessagesAsRead(string sessionId, string readerId)
        {
            var messages = _context.ChatMessages.Find(m => m.SessionId == sessionId && m.ReceiverId == readerId).ToList();
            foreach (var msg in messages)
            {
                msg.IsRead = true;
                _context.ChatMessages.ReplaceOne(m => m.MessageId == msg.MessageId, msg);
            }
        }

        public void EndChatSession(string sessionId)
        {
            var session = _context.ChatSessions.Find(s => s.SessionId == sessionId).FirstOrDefault();
            if (session != null)
            {
                session.EndedAt = DateTime.Now;
                session.Status = "Closed";
                _context.ChatSessions.ReplaceOne(s => s.SessionId == sessionId, session);
            }
        }

        private string GenerateSessionId()
        {
            return "CHAT" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }

        private string GenerateMessageId()
        {
            return "MSG" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999);
        }
    }
}