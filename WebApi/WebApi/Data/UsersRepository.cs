using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using WebApi.Dtos;
using CloudinaryDotNet.Actions;
using WebApi.Class;
using WebApi.Dto;
using WebApi.Classes;
using WebApi.CustomModel;
using System.Threading;
using ClosedXML.Excel;
using static WebApi.Helpers.MessageLogger;
using System.IO;

namespace WebApi.Data
{
    public class UsersRepository : ControllerBase, IUsersRepository
    {
        private Cloudinary _cloudinary;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly DanceSchoolContext _context;
        public UsersRepository(DanceSchoolContext context, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret
           );
            _cloudinary = new Cloudinary(acc);
            _context = context;
        }

        public async Task<MessageLogger> UpdateImage(string id, dynamic obj)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var user = await (from u in _context.User
                                  where u.Id == int.Parse(id)
                                  select u).FirstOrDefaultAsync();

                if (user != null)
                {
                    user.ImageUrl = obj.Url;
                    user.PublicId = obj.PublicId;
                    await _context.SaveChangesAsync();
                }
                messageLogger.AddMessage("Image updated", user, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> AddImageForUser(PhotoForCreation photoForCreation)
        {
            return await Task.Run(() =>
            {
                MessageLogger messageLogger = new MessageLogger();
                try
                {
                    var file = photoForCreation.File;
                    var uploadResult = new ImageUploadResult();

                    if (file == null)
                        return null;
                    else
                    {
                        if (file.Length > 0 && file.Length < 24000000)
                        {
                            using (var stream = file.OpenReadStream())
                            {
                                var uploadParams = new ImageUploadParams()
                                {
                                    File = new FileDescription(file.Name, stream),
                                    Transformation = new Transformation()
                                    .Width(500).Height(500).Crop("fill").Gravity("face")
                                };
                                uploadResult = _cloudinary.Upload(uploadParams);
                            }
                        }


                        photoForCreation.Url = uploadResult.Uri.ToString();
                        photoForCreation.PublicId = uploadResult.PublicId;
                        photoForCreation.Description = "Image uploaded";
                        //messageLogger.AddMessage("Image uploaded", photoForCreation.Url, MessageCode.Information);
                        //messageLogger.AddMessage("Image uploaded", photoForCreation.PublicId, MessageCode.Information);
                        messageLogger.AddMessage("Image uploaded", photoForCreation, MessageCode.Information);


                        return messageLogger;

                    }
                }
                catch (Exception ex)
                {
                    messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                    return messageLogger;
                }
            });
        }
        public async Task<MessageLogger> DeleteUser(int id, int id2)
        {
            MessageLogger messageLogger = new MessageLogger();
            var userdeleted = (object)null;
            try
            {
                if (id == id2)
                {
                    messageLogger.AddMessage("Cannot delete this account!", null, MessageCode.Error);
                    return messageLogger;
                }
                else
                {
                    userdeleted = await (from user in _context.User
                                         where user.Id == id
                                         select user).FirstOrDefaultAsync();

                    if (userdeleted != null)
                    {
                        _context.Remove(userdeleted);
                        await _context.SaveChangesAsync();
                    }

                    messageLogger.AddMessage("user deleted", userdeleted, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> UpdateUser(User user, string username, string firstname, string lastname, string city,
                                   string street, string area, string dateofbirth, string email, string phonenumber, string password, string descr, string isadmin)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                DateTime fixeddt = DateTime.Parse(dateofbirth);

                user = await _context.User.FirstOrDefaultAsync(x => x.Username == username);

                var checkMailPhone = await (from v in _context.User
                                            where v.Email == email || v.Phonenumber == phonenumber
                                            where v.Username != username
                                            select v).FirstOrDefaultAsync();

                if (checkMailPhone != null)
                    return null;
                else
                {
                    if (user != null)
                    {
                        user.FirstName = firstname;
                        user.LastName = lastname;
                        user.City = city;
                        user.Area = area;
                        user.Street = street;
                        user.Email = email;
                        user.Phonenumber = phonenumber;
                        user.DateofBirth = fixeddt;
                        user.Password = PasswordHash.encryptPassword(password);
                        user.Descr = descr;
                        user.IsAdmin = Int16.Parse(isadmin);

                        await _context.SaveChangesAsync();

                        messageLogger.AddMessage("User updated", user, MessageCode.Information);
                        return messageLogger;
                    }
                    else
                    {
                        messageLogger.AddMessage("Failed to update", null, MessageCode.Information);
                        return messageLogger;
                    }
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> UserExists(string username)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var user = await _context.User.CountAsync(x => x.Username == username);
                if (user > 0)
                {
                    messageLogger.AddMessage("User already exist", user, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("", null, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> GetUser(int id)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var user = (object)null;

                user = await (from u in _context.User
                              where u.Id == id
                              select new
                              {
                                  u.Area,
                                  u.City,
                                  Birthdate = u.DateofBirth == null ? null : u.DateofBirth.Value.ToShortDateString(),
                                  u.Email,
                                  u.FirstName,
                                  u.Id,
                                  u.IsAdmin,
                                  u.LastName,
                                  u.Phonenumber,
                                  u.Street,
                                  u.Username,
                                  u.PublicId,
                                  u.ImageUrl,
                                  Password = PasswordHash.decryptPassword(u.Password),
                                  Descr = u.Descr == null ? "" : u.Descr
                              }).FirstOrDefaultAsync();

                messageLogger.AddMessage("", user, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> GetUsers()
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var users = (object)null;
                users = await (from u in _context.User
                               join r in _context.Roles
                               on u.IsAdmin equals r.RoleCode
                               select new
                               {
                                   u.Username,
                                   u.Id,
                                   u.Area,
                                   u.City,
                                   Birthdate = u.DateofBirth == null ? null : u.DateofBirth.Value.ToShortDateString(),
                                   u.Email,
                                   u.FirstName,
                                   u.LastName,
                                   u.Phonenumber,
                                   u.Street,
                                   u.IsAdmin,
                                   u.ImageUrl,
                                   Descr = u.Descr == null ? "" : u.Descr,
                                   Password = PasswordHash.decryptPassword(u.Password),
                                   ConfirmPassword = PasswordHash.decryptPassword(u.Password)
                               }).ToListAsync();

                messageLogger.AddMessage("", users, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> ResetPassword(string email)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    messageLogger.AddMessage("User with this password doesnt exist", null, MessageCode.Error);
                    return messageLogger;
                }
                else
                {
                    var randomPassword = GenerateRandomPassword.GeneratePassword();
                    user.Password = PasswordHash.encryptPassword(randomPassword);
                    await _context.SaveChangesAsync();

                    await Task.Run(() => { MailInform.SendMail(email, randomPassword, user.LastName); });

                    messageLogger.AddMessage("Password has been reset", user, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
        public async Task<MessageLogger> Register(User user, string password, string firstname, string lastname, string city, string area, string street,
                            string phonenumber, DateTime birthdate, string email, short isadmin, string imgurl, string descr, string publicid)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var getmailPhone = await _context.User.FirstOrDefaultAsync(x => x.Email == email || x.Phonenumber == phonenumber);

                if (getmailPhone == null)
                {
                    user.Password = PasswordHash.encryptPassword(password);
                    user.Area = area;
                    user.City = city;
                    user.Email = email;
                    user.ImageUrl = imgurl;
                    user.DateofBirth = birthdate;
                    user.IsAdmin = isadmin;
                    user.Phonenumber = phonenumber;
                    user.Email = email;
                    user.LastName = lastname;
                    user.FirstName = firstname;
                    user.Street = street;
                    user.Descr = descr;
                    user.PublicId = publicid;
                    user.Provider = "DanceSchool";

                    await _context.User.AddAsync(user);
                    await _context.SaveChangesAsync();

                    await Task.Run(() => { MailInform.SendMailRegisterToApp(firstname, lastname, email); });


                    messageLogger.AddMessage("Registered successful", user, MessageCode.Information);
                    return messageLogger;
                }
                else if (getmailPhone.Username == null) //user mail exist cause he used the same in social login before he was registered in app
                {
                    getmailPhone.Username = user.Username;
                    getmailPhone.Password = PasswordHash.encryptPassword(password);
                    getmailPhone.Area = area;
                    getmailPhone.City = city;
                    getmailPhone.Email = email;
                    getmailPhone.ImageUrl = imgurl;
                    getmailPhone.DateofBirth = birthdate;
                    getmailPhone.IsAdmin = isadmin;
                    getmailPhone.Phonenumber = phonenumber;
                    getmailPhone.Email = email;
                    getmailPhone.LastName = lastname;
                    getmailPhone.FirstName = firstname;
                    getmailPhone.Street = street;
                    getmailPhone.Descr = descr;
                    getmailPhone.PublicId = publicid;
                    getmailPhone.IsAdmin = 0;
                    getmailPhone.Provider = "DanceSchool";

                    await _context.SaveChangesAsync();

                    await Task.Run(() => { MailInform.SendMailRegisterToApp(firstname, lastname, email); });

                    messageLogger.AddMessage("Registered successful", getmailPhone, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("This email already exist", null, MessageCode.Error);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetTeachers()
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var teachers = (object)null;
                teachers = await (from u in _context.User
                                  join r in _context.Roles
                                  on u.IsAdmin equals r.RoleCode
                                  where u.IsAdmin == 2
                                  select new
                                  {
                                      Teacherid = u.Id,
                                      Firstname = u.FirstName,
                                      Lastname = u.LastName,
                                      Description = u.Descr,
                                      Imageurl = u.ImageUrl,
                                      u.Email
                                  }).ToListAsync();

                messageLogger.AddMessage("", teachers, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Create_And_WriteXLS_Users(string id, List<CustomUsers> customUsers)
        {
            return await Task.Run(() =>
             {
                 MessageLogger messageLogger = new MessageLogger();
                 try
                 {
                     string path = Path.Combine(Environment.CurrentDirectory, @"XlsFiles\" + id);

                     if (System.IO.Directory.Exists(path))
                     {
                         System.IO.Directory.Delete(path, true);
                         System.IO.Directory.CreateDirectory(path);
                     }
                     else
                         System.IO.Directory.CreateDirectory(path);

                     using (var workbook = new XLWorkbook())
                     {
                         var worksheet = workbook.Worksheets.Add("users");
                         var currentRow = 1;

                         #region Header
                         worksheet.Cell(currentRow, 1).Value = "Id";
                         worksheet.Cell(currentRow, 2).Value = "Username";
                         worksheet.Cell(currentRow, 3).Value = "Password";
                         worksheet.Cell(currentRow, 4).Value = "FirstName";
                         worksheet.Cell(currentRow, 5).Value = "LastName";
                         worksheet.Cell(currentRow, 6).Value = "Birthdate";
                         worksheet.Cell(currentRow, 7).Value = "Email";
                         worksheet.Cell(currentRow, 8).Value = "Phonenumber";
                         worksheet.Cell(currentRow, 9).Value = "Descr";
                         worksheet.Cell(currentRow, 10).Value = "Area";
                         worksheet.Cell(currentRow, 11).Value = "City";
                         worksheet.Cell(currentRow, 12).Value = "Street";
                         worksheet.Cell(currentRow, 13).Value = "IsAdmin";
                         worksheet.Cell(currentRow, 14).Value = "Provider";
                         #endregion

                         #region Body
                         foreach (var item in customUsers.ToList())
                         {
                             currentRow++;
                             worksheet.Cell(currentRow, 1).Value = item.Id == null ? "" : item.Id.ToString();
                             worksheet.Cell(currentRow, 2).Value = item.Username == null ? "" : item.Username.ToString();
                             worksheet.Cell(currentRow, 3).Value = item.Password == null ? "" : item.Password.ToString();
                             worksheet.Cell(currentRow, 4).Value = item.FirstName == null ? "" : item.FirstName.ToString();
                             worksheet.Cell(currentRow, 5).Value = item.LastName == null ? "" : item.LastName.ToString();
                             worksheet.Cell(currentRow, 6).Value = item.Birthdate == null ? "" : item.Birthdate.ToString();
                             worksheet.Cell(currentRow, 7).Value = item.Email == null ? "" : item.Email.ToString();
                             worksheet.Cell(currentRow, 8).Value = item.Phonenumber == null ? "" : item.Phonenumber.ToString();
                             worksheet.Cell(currentRow, 9).Value = item.Descr == null ? "" : item.Descr.ToString();
                             worksheet.Cell(currentRow, 10).Value = item.Area == null ? "" : item.Area.ToString();
                             worksheet.Cell(currentRow, 11).Value = item.City == null ? "" : item.City.ToString();
                             worksheet.Cell(currentRow, 12).Value = item.Street == null ? "" : item.Street.ToString();

                             if (item.IsAdmin == null)
                                 worksheet.Cell(currentRow, 13).Value = "";
                             else if (item.IsAdmin == "1")
                                 worksheet.Cell(currentRow, 13).Value = "Administrator";
                             else if (item.IsAdmin == "2")
                                 worksheet.Cell(currentRow, 13).Value = "Teacher";
                             else if (item.IsAdmin == "0")
                                 worksheet.Cell(currentRow, 13).Value = "User";

                             worksheet.Cell(currentRow, 14).Value = item.Provider == null ? "" : item.Provider.ToString();
                         }
                         #endregion

                         workbook.SaveAs(path + @"\users.xlsx");
                         Thread.Sleep(5000);
                         var content = System.IO.File.ReadAllBytes(path + @"\users.xlsx");

                         if (System.IO.Directory.Exists(path))
                             System.IO.Directory.Delete(path, true);

                         messageLogger.AddMessage("xls has been created", System.Convert.ToBase64String(content), MessageCode.Information);
                         return messageLogger;
                     }
                 }
                 catch (Exception ex)
                 {
                     messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                     return messageLogger;
                 }
             });
        }

        public async Task<MessageLogger> UpdateUserProfile(User user, string username, string firstname, string lastname, string city,
                                            string street, string area, string dateofbirth, string email, string phonenumber, string password, string descr)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                DateTime fixeddt = DateTime.Parse(dateofbirth);

                user = await _context.User.FirstOrDefaultAsync(x => x.Username == username);

                var checkMailPhone = await (from v in _context.User
                                            where v.Email == email || v.Phonenumber == phonenumber
                                            where v.Username != username
                                            select v).FirstOrDefaultAsync();

                if (checkMailPhone != null)
                    return null;
                else
                {
                    if (user != null)
                    {
                        user.FirstName = firstname;
                        user.LastName = lastname;
                        user.City = city;
                        user.Area = area;
                        user.Street = street;
                        user.Email = email;
                        user.Phonenumber = phonenumber;
                        user.DateofBirth = fixeddt;
                        user.Password = PasswordHash.encryptPassword(password);
                        user.Descr = descr;

                        await _context.SaveChangesAsync();

                        messageLogger.AddMessage("Profile updated", user, MessageCode.Information);
                        return messageLogger;
                    }
                    else
                    {
                        messageLogger.AddMessage("Profile failed to update", null, MessageCode.Error);
                        return messageLogger;
                    }
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
    }
}
