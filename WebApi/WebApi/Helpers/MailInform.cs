using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Classes
{
    public static class MailInform
    {
        static MailInform() { }

        public static void SendMail(string email, string randomPassword, string lastname)
        {
            try
            {
                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Your password changed!";
                //body
                message.Body = new TextPart("plain")
                {
                    Text = "Your old password has been deleted, new password is: " + randomPassword
                };

                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

        public static void SendMailUpdateTeachingProgram(string email, string firstname, string lastname, string lesson,
                                                            string Dayofweek, string Lessonstart, string Lessonend)
        {
            try
            {
                var day = "";
                if (Dayofweek.ToString() == "1")
                    day = "Monday";
                else if (Dayofweek.ToString() == "2")
                    day = "Tuesday";
                else if (Dayofweek.ToString() == "3")
                    day = "Wednesday";
                else if (Dayofweek.ToString() == "4")
                    day = "Thursday";
                else if (Dayofweek.ToString() == "5")
                    day = "Friday";
                else if (Dayofweek.ToString() == "6")
                    day = "Saturday";
                else if (Dayofweek.ToString() == "7")
                    day = "Sunday";

                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Changes in Teaching Program!";
                //body
                var builder = new BodyBuilder();
                builder.HtmlBody = "<html>" +
                               "<body>" + " Hello " + lastname + " " + firstname + "<br>" +
                               " We would like to inform you that changes has been made to the " + lesson + " lesson that you have been registered. See below: " + "<br>" +
                               " Day: " + day + "<br>" +
                               " Hours: " + Lessonstart + " " + Lessonend + "<br>" + "</body></html>";

                message.Body = builder.ToMessageBody();

                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

        public static void SendMailDeleteTeachingProgram(string email, string firstname, string lastname, string lesson,
                                                    string Dayofweek, string Lessonstart, string Lessonend)
        {
            try
            {
                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Teaching Program has been Canceled!";
                //body
                message.Body = new TextPart("plain")
                {
                    Text = "Hello " + lastname + " " + firstname +
                    " We would like to inform you that current " + lesson + " lesson at " + Dayofweek + " at time "
                    + Lessonstart + "-" + Lessonend + " that you have been registered, it will not carried out."
                };

                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

        public static void SendMailRegisterSubscription(string duration, string price, string[] day, bool discount, DateTime date,
                                                        string firstname, string lastname, string email, string lesson, List<string> hours)
        {
            try
            {
                var str = "";
                var foo = String.Join("-", hours);
                if (discount == true)
                    str = "Yes";
                else
                    str = "No";

                var day1 = string.Join(" ", day);

                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Subscription has been made!";
                //body
                var builder = new BodyBuilder();
                builder.HtmlBody = "<html>" +
                               "<body>" + " Hello " + lastname + " " + firstname + "<br>" +
                               " We would like to inform you that your subscription has been successful! Package Info:" + "<br>" +
                               " Lesson: " + lesson + "<br>" +
                               " Duration: " + duration + "<br>" +
                               " Price: " + price + "<br>" +
                               " Discount: " + str + "<br>" +
                               " Teaching Program: " + day1 + "</body></html>";

                message.Body = builder.ToMessageBody();

                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

        public static void SendMailRegisterClass(string email, string firstname, string lastname, string lesson,
                                            string Dayofweek, string Lessonstart, string Lessonend, DateTime dt)
        {
            try
            {
                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Registration to classroom!";
                //body
                var builder = new BodyBuilder();
                builder.HtmlBody = "<html>" +
                               "<body>" + " Hello " + lastname + " " + firstname + "<br>" +
                               " We would like to inform you that current registration was successful. Lesson Infos: " + "<br>" +
                               " Lesson: " + lesson + "<br>" +
                               " Day: " + Dayofweek + "<br>" +
                               " Hours: " + Lessonstart + " " + Lessonend + "<br>" +
                               " Date: " + dt + "</body></html>";

                message.Body = builder.ToMessageBody();
                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

        public static void SendMailRegisterToApp(string firstname, string lastname, string email)
        {
            try
            {
                var message = new MimeMessage();
                //from address
                message.From.Add(new MailboxAddress("Dance School", "Apostolis.kf@gmail.com"));
                //to address
                message.To.Add(new MailboxAddress(lastname, email));
                //subject
                message.Subject = "Welcome to our school!";
                //body
                var builder = new BodyBuilder();
                builder.HtmlBody = "<html>" +
                               "<body>" + " Hello " + lastname + " " + firstname + "<br>" +
                               " We would like to thank you for your registration in our system. Explore yourself, and choose the lesson that fits to you!" + "</body></html>";

                message.Body = builder.ToMessageBody();
                //configure and send email
                using (var client = new SmtpClient())
                {
                    try
                    {
                        //accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("Apostolis.kf@gmail.com", "123456789A!");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error -> " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -> " + ex);
            }
        }

    }
}
