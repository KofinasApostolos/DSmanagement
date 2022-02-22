using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomTeachingProgram
    {
        public string Id { get; set; }
        public string Lessonid { get; set; }
        public string Dayofweek { get; set; }
        public string Lessonstart { get; set; }
        public string Lessonend { get; set; }
        public string Capacity { get; set; }

        public CustomTeachingProgram(string Id, string Lessonid, string Dayofweek, string Lessonstart, string Lessonend, string Capacity)
        {
            this.Id = Id;
            this.Lessonid = Lessonid;
            this.Dayofweek = Dayofweek;
            this.Lessonstart = Lessonstart;
            this.Lessonend = Lessonend;
            this.Capacity = Capacity;
        }
    }
}
