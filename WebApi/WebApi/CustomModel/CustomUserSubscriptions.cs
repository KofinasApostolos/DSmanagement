using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomUserSubscriptions
    {
        public string Id { get; set; }
        public string Userid { get; set; }
        public string Lessonid { get; set; }
        public string Months { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
        public string ExpDate { get; set; }
        public string Discount { get; set; }
        public string[] Day { get; set; }
        public string State { get; set; }
        public string Duration { get; set; }

        public CustomUserSubscriptions(string Id, string Userid, string Lessonid, string Months, string Price,
                            string Date, string ExpDate, string Discount, string[] Day, string State, string Duration)
        {
            this.Id = Id;
            this.Userid = Userid;
            this.Lessonid = Lessonid;
            this.Months = Months;
            this.Price = Price;
            this.Date = Date;
            this.ExpDate = ExpDate;
            this.Discount = Discount;
            this.Day = Day;
            this.State = State;
            this.Duration = Duration;
        }
    }
}
