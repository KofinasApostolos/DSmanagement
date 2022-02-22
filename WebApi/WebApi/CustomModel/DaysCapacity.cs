using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public class DaysCapacity
    {
        public string Dayid { get; set; }
        public string Count { get; set; }
        public string Day { get; set; }

        public DaysCapacity(string Dayid, string Count, string Day)
        {
            this.Dayid = Dayid;
            this.Count = Count;
            this.Day = Day;
        }
    }
}
