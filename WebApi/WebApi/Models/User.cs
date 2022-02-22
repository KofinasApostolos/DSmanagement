using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class User
    {
        public User()
        {
            Lessons = new HashSet<Lessons>();
            UserSubscriptions = new HashSet<UserSubscriptions>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public DateTime? DateofBirth { get; set; }
        public short? IsAdmin { get; set; }
        public string PublicId { get; set; }
        public string ImageUrl { get; set; }
        public string Provider { get; set; }
        public string Descr { get; set; }

        public ICollection<Lessons> Lessons { get; set; }
        public ICollection<UserSubscriptions> UserSubscriptions { get; set; }
    }
}
