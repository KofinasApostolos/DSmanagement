using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class UserSubscriptions
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public string Lessonid { get; set; }
        public short Duration { get; set; }
        public string Price { get; set; }
        public DateTime Date { get; set; }
        public short State { get; set; }
        public bool? Discount { get; set; }
        public string Dayid { get; set; }

        public User User { get; set; }
    }
}
