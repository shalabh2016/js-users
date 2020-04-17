using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JsUsers.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [Required]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [Required]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [Required]
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt {get;set;}

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
