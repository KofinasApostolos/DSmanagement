using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class RoleForRegister
    {
        [Required(ErrorMessage = "Role Code is required")]
        [RegularExpression("([0-9]+)")]
        [Range(0, 5, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public short RoleCode { get; set; }
        [Required(ErrorMessage = "Role Description is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Length of Role Description cannot extend 50 characters")]
        public string RoleDescr { get; set; }

    }
}
