using System;
using System.Collections.Generic;

namespace WebApi.Models
{
    public partial class Lessons
    {
        public Lessons()
        {
            TeachingProgram = new HashSet<TeachingProgram>();
        }

        public string Lessonid { get; set; }
        public int Teacherid { get; set; }
        public string Lesson { get; set; }
        public string Descr { get; set; }
        public string PublicId { get; set; }
        public string Utubeurl { get; set; }
        public string ImageUrl { get; set; }

        public User Teacher { get; set; }
        public ICollection<TeachingProgram> TeachingProgram { get; set; }
    }
}
