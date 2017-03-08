using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeFeedbackWebApp.Classes
{
    public class FeedbackCycleViewModel
    {
        public FeedbackCycleViewModel(long feedbackCycleId, string feedbackCycleName)
        {
            FeedbackCycleId = (int)feedbackCycleId;
            FeedbackCycleName = feedbackCycleName;
        }

        public int FeedbackCycleId { get; set; }
        public string FeedbackCycleName { get; set; }
    }
}