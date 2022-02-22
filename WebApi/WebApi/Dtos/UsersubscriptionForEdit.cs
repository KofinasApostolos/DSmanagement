using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UsersubscriptionForEdit
    {
        [Required(ErrorMessage = "ID is required")]
        public string Id { get; set; }
        [Required(ErrorMessage = "User is required")]
        public string Userid { get; set; }
        [Required(ErrorMessage = "Lesson is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Day is required")]
        public string[] Day { get; set; }
        [Required(ErrorMessage = "Duration is required")]
        public string Duration { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public string Price { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public string Date { get; set; }
        public bool Discount { get; set; }
        [Required(ErrorMessage = "State is required")]
        public short State { get; set; }
    }
}
