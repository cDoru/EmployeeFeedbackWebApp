//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmployeeFeedbackWebApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EndOfYearFeedback
    {
        public long EOYFeedbackId { get; set; }
        public string EOYOverallPerf { get; set; }
        public string EOYCommentsPositive { get; set; }
        public string EOYCommentsImprove { get; set; }
        public Nullable<long> EOYFeedbackCycleId { get; set; }
        public Nullable<long> EOYFeedbackAssignmentId { get; set; }
    
        public virtual FeedbackAssignment FeedbackAssignment { get; set; }
    }
}