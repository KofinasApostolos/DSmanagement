using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class TeachingProgramTemp
    {
        public short Id { get; set; }
        public short Userid { get; set; }
        public string Lessonid { get; set; }
        public short Dayofweek { get; set; }
        public string Lessonstart { get; set; }
        public string Lessonend { get; set; }
        public int Count { get; set; }
    }
}
