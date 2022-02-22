using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomUsers
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Street { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public string Birthdate { get; set; }
        public string IsAdmin { get; set; }
        public string Provider { get; set; }
        public string Descr { get; set; }

        public CustomUsers(string Id, string Username, string Password, string FirstName, string LastName,
                            string City, string Area, string Street, string Email, string Phonenumber,
                            string Birthdate, string Provider, string Descr)
        {
            this.Id = Id;
            this.Username = Username;
            this.Password = Password;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.City = City;
            this.Area = Area;
            this.Street = Street;
            this.Email = Email;
            this.Phonenumber = Phonenumber;
            this.Birthdate = Birthdate;
            this.IsAdmin = IsAdmin;
            this.Provider = Provider;
            this.Descr = Descr;
        }
    }
}
