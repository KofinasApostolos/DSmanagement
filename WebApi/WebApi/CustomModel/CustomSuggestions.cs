using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomSuggestions
    {
        public string Id { get; set; }
        public string Lessonid { get; set; }
        public short Dayofweek { get; set; }
        public string Lessonstart { get; set; }
        public string Lessonend { get; set; }
        public string Count { get; set; }

        public CustomSuggestions(string Id, string Lessonid, short Dayofweek, string Lessonstart, string Lessonend, string Count)
        {
            this.Id = Id;
            this.Lessonid = Lessonid;
            this.Dayofweek = Dayofweek;
            this.Lessonstart = Lessonstart;
            this.Lessonend = Lessonend;
            this.Count = Count;

        }
    }
}
