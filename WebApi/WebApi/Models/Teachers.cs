using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class Teachers
    {
        public Teachers()
        {
            Lessons = new HashSet<Lessons>();
        }

        public string Teacherid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Imageurl { get; set; }
        public string Descr { get; set; }
        public string Email { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Phonenumber { get; set; }

        public ICollection<Lessons> Lessons { get; set; }
    }
}
