using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BBApp.Models {
    public class User : BaseEntity {
        public int Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<Join> Joins { get; set; }
        public User()
        {
            Joins = new List<Join>();
        }
    }
}