using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BBApp.Models {
    public class Join : BaseEntity 
    {
        public int JoinId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Song song { get; set; }
        public int SongId { get; set; } 
    }
}