using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using VISHA_HAMEED.Data;
using VISHA_HAMEED.Models;

namespace VISHA_HAMEED.Services
{
    public class FeedbackService
    {
        private readonly DatabaseContext _context;

        public FeedbackService(DatabaseContext context)
        {
            _context = context;
        }

        public bool SubmitFeedback(Feedback feedback)
        {
            try
            {
                _context.Feedbacks.InsertOne(feedback);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Feedback> GetAllFeedbacks()
        {
            return _context.Feedbacks.Find(_ => true).ToList();
        }

        public double GetAverageRating()
        {
            var feedbacks = _context.Feedbacks.Find(_ => true).ToList();
            if (feedbacks.Count == 0) return 0;
            return feedbacks.Average(f => f.Rating);
        }

        public int GetTotalFeedbacks()
        {
            return (int)_context.Feedbacks.CountDocuments(_ => true);
        }
    }
}