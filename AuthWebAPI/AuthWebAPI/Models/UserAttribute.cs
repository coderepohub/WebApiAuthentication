using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthWebAPI.Models
{
    public class UserAttribute
    {
        public string guId { get; set; }
        public string pan { get; set; }
        public string userId { get; set; }
        public string userType { get; set; }
        public string refNo { get; set; }    

        public string distId { get; set; }
        public string role { get; set; }
    }
}