using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class RoleForEdit
    {
        [Required(ErrorMessage = "Role ID is required")]
        [RegularExpression("([0-9]+)")]
        [Range(0, 5, ErrorMessage = "Please enter valid integer Number")]
        public short RoleCode { get; set; }
        [Required(ErrorMessage = "Role Description is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Length of Role Description cannot extend 50 characters, or be less than 4!")]
        public string RoleDescr { get; set; }
    }
}
