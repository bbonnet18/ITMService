using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITMService.Models
{
    public class User
    {
        public int userID { get; set; }
        public string userName { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string password { get; set; }

    }
}