using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class BookingForRegister
    {
        [Required]
        public string Userid { get; set; }
        [Required]
        public string Lessonid { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required]
        public string[] Day { get; set; }
        [Required]
        public bool Discount { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
