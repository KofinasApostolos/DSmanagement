using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class LessonsForEdit
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public string Lessonid { get; set; }
        [Required(ErrorMessage = "Teacher is required")]
        public string Teacherid { get; set; }
        [Required(ErrorMessage = "Lesson is required")]
        [StringLength(maximumLength: 100, MinimumLength = 4, ErrorMessage = "Length of Lesson Name cannot extend 100 characters or be less than 4!")]
        public string Lesson { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(maximumLength: 3000, MinimumLength = 50, ErrorMessage = "Length of Description cannot extend 3000 characters or be less than 50")]
        public string Descr { get; set; }
        public string Imageurl { get; set; }
        [StringLength(maximumLength: 500, MinimumLength = 3, ErrorMessage = "Length of Utube URL cannot extend 500 characters or be less than 3")]
        public string Utubeurl { get; set; }
    }
}
