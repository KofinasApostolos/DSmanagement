using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UserForEdit
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(maximumLength: 20, ErrorMessage = "Username length cannot extend 20 characters!")]
        public string username { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Firstname length cannot extend 50 characters or be less than 4 characters!")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Lastname length cannot extend 50 characters or be less than 4 characters!")]
        public string lastname { get; set; }
        [Required(ErrorMessage = "Street is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Street length cannot extend 50 characters or be less than 4 characters!")]
        public string street { get; set; }
        [Required(ErrorMessage = "City is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "City length cannot extend 50 characters or be less than 4 characters!")]
        public string city { get; set; }
        [Required(ErrorMessage = "Area is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Area length cannot extend 50 characters or be less than 4 characters!")]
        public string area { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(maximumLength: 3000, MinimumLength = 50, ErrorMessage = "Description length cannot extend 3000 characters or be less than 50 characters!")]
        public string descr { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "E-mail length cannot extend 50 characters!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }
        [Required(ErrorMessage = "Phonenumber is required")]
        [StringLength(maximumLength: 50, MinimumLength = 4, ErrorMessage = "Phonenumber length cannot extend 50 characters or be less than 4 characters!")]
        public string phonenumber { get; set; }
        [Required(ErrorMessage = "Birthdate is required")]
        public string birthdate { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public short isadmin { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(maximumLength: 50, MinimumLength = 6, ErrorMessage = "Password length cannot extend 50 characters or be less than 6 characters!")]
        public string password { get; set; }
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        [StringLength(maximumLength:50, MinimumLength = 6, ErrorMessage = "Your password length must be between 6 and 50 characters!")]
        public string ConfirmPassword { get; set; }
    }
}
