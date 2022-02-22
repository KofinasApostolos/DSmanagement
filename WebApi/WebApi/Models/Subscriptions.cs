using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class Subscriptions
    {
        public short Id { get; set; }
        public string Lessonid { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        public bool? Discount { get; set; }
        public string Discprice { get; set; }
    }
}
