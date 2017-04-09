using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BBApp.Models {
    public class Song : BaseEntity
    {
        public int id { get; set; }
        public int user_id { get; set; }
        [Required]
        [MinLength(2)]
        public string artist { get; set; }
        [Required]
        [MinLength(2)]
        public string title { get; set; }
        public bool added { get; set; }
        public string youtube { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<Join> Joins { get; set; }
        public Song()
        {
            Joins = new List<Join>();
        }
    }
}