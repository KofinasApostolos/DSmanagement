using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UserLoginSocial
    {
        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Firstname length cannot extend 50 characters or be less than 4 characters!")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Firstname length cannot extend 50 characters or be less than 4 characters!")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "E-mail length cannot extend 50 characters!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        //[Required]
        public string Provider { get; set; }
    }
}
