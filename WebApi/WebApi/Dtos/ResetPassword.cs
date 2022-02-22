using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "E-mail length cannot extend 50 characters!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
