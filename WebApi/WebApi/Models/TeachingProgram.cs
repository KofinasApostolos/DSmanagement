using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class TeachingProgram
    {
        public short Id { get; set; }
        public string Lessonid { get; set; }
        public short Dayofweek { get; set; }
        public string Lessonstart { get; set; }
        public string Lessonend { get; set; }
        public short Capacity { get; set; }

        public Lessons Lesson { get; set; }
    }
}
