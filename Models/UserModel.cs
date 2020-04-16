using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsUsers.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt { get; set; }
    }
}
