using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UserForLogin
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(maximumLength: 24, ErrorMessage = "Your Username length cannot extend 24 characters!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Your password length must be between 6 and 24 characters!")]
        public string Password { get; set; }
    }
}
