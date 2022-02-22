using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class TempRegisters
    {
        public int Id { get; set; }
        public short? Teachingprogramid { get; set; }
        public int? Userid { get; set; }
        public string Lessonid { get; set; }
        public DateTime? Date { get; set; }
    }
}
