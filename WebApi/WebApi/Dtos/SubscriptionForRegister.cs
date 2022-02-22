using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class SubscriptionForRegister
    {
        [Required(ErrorMessage = "Lesson is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public string Duration { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [RegularExpression(@"^\d*\.?\d{0,2}$", ErrorMessage = "Characters are not allowed.")]
        public string Price { get; set; }
        public bool Discount { get; set; }
        [RegularExpression(@"^\d*\.?\d{0,2}$", ErrorMessage = "Characters are not allowed.")]
        public string Discprice { get; set; }
    }
}
