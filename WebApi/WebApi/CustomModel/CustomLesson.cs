using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.CustomModel
{
    public class CustomLesson
    {
        public string Lessonid { get; set; }
        public string Teacherid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ImageurlTeacher { get; set; }
        public string DescrTeacher { get; set; }
        public string ImageurlLesson { get; set; }
        public string Lesson { get; set; }
        public string DescrLesson { get; set; }
        public string Discountprice { get; set; }

        public CustomLesson(string Lessonid, string Teacherid, string Firstname, string Lastname, string ImageurlTeacher,
                            string DescrTeacher, string ImageurlLesson, string Lesson, string DescrLesson, string Discountprice)
        {
            this.Lessonid = Lessonid;
            this.Teacherid = Teacherid;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
            this.ImageurlTeacher = ImageurlTeacher;
            this.DescrTeacher = DescrTeacher;
            this.ImageurlLesson = ImageurlLesson;
            this.Lesson = Lesson;
            this.DescrLesson = DescrLesson;
            this.Discountprice = Discountprice;

        }
    }
}
