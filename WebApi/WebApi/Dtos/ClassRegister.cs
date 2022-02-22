using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class ClassRegister
    {
        [Required]
        public short Capacity { get; set; }
        [Required]
        public string Day { get; set; }
        [Required]
        public string Lessonstart { get; set; }
        [Required]
        public string Lessonsend { get; set; }
        [Required]
        public string Lessonid { get; set; }
        [Required]
        public string Userid { get; set; }
    }
}
