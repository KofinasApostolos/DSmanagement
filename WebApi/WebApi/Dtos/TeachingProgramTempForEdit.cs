using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class TeachingProgramTempForEdit
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Lessonid { get; set; }
        [Required]
        public string Dayofweek { get; set; }
        [Required]
        public string Lessonstart { get; set; }
        [Required]
        public string Lessonend { get; set; }
    }
}
