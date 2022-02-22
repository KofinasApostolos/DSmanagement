using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class TeachingProgramForEdit
    {
        [Required(ErrorMessage = "ID is required")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Lesson is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Day is required")]
        public short Dayofweek { get; set; }
        [Required(ErrorMessage = "Lesson Start is required")]
        public string Lessonstart { get; set; }
        [Required(ErrorMessage = "Lesson End is required")]
        public string Lessonend { get; set; }
        [Required(ErrorMessage = "Capacity is required")]
        [RegularExpression("([0-9]+)")]
        [Range(0, 50, ErrorMessage = "Please enter valid integer Number")]
        public short Capacity { get; set; }
    }
}
