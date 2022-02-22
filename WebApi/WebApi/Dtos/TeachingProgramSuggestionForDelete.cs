using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class TeachingProgramSuggestionForDelete
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Lessonid is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Day is required")]
        public string Day { get; set; }
        [Required(ErrorMessage = "Lessonstart is required")]
        public string Lessonstart { get; set; }
        [Required(ErrorMessage = "Lessonend is required")]
        public string Lessonend { get; set; }
        [Required(ErrorMessage = "Count is required")]
        public string Count { get; set; }
    }
}
