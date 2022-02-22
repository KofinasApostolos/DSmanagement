using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomMySubscriptions
    {
        public string Lesson { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
        public string ExpDate { get; set; }
        public string State { get; set; }
        public string Discount { get; set; }
        public string Days { get; set; }

        public CustomMySubscriptions(string Lesson, string Duration, string Price, string Date, string ExpDate, string State,
                            string Discount, string Days)
        {
            this.Lesson = Lesson;
            this.Duration = Duration;
            this.Price = Price;
            this.Date = Date;
            this.ExpDate = ExpDate;
            this.State = State;
            this.Discount = Discount;
            this.Days = Days;
        }
    }
}
