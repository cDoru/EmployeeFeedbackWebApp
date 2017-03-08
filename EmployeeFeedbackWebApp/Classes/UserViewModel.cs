using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeFeedbackWebApp.Classes
{
    public class UserViewModel
    {
        private long userId;
        private long reportsTo;

        public UserViewModel(long userId, string fullName, long RoleId)
        {
            UserId = (int)userId;
            FullName = fullName;
            RoleId = (int)RoleId;
        }

        public int UserId { get; set; }
        
        public string FullName { get; set; }
        public int RoleId { get; set; }
    }
}