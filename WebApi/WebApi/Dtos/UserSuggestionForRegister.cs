using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UserSuggestionForRegister
    {
        public short Id { get; set; }
        [Required(ErrorMessage = "User ID is required")]
        public string Userid { get; set; }
        [Required(ErrorMessage = "Lesson is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Day is required")]
        public string Dayofweek { get; set; }
        [Required(ErrorMessage = "Lesson Start is required")]
        public string Lessonstart { get; set; }
        [Required(ErrorMessage = "Lesson End is required")]
        public string Lessonend { get; set; }
    }
}
