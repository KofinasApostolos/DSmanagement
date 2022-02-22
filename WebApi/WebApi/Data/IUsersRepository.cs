using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.CustomModel;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IUsersRepository
    {
        Task<MessageLogger> GetUsers();
        Task<MessageLogger> GetTeachers();
        Task<MessageLogger> GetUser(int id);
        Task<MessageLogger> DeleteUser(int id, int id2);
        Task<MessageLogger> UserExists(string username);
        Task<MessageLogger> UpdateImage(string id, dynamic obj);
        Task<MessageLogger> UpdateUser(User user, string username, string firstname, string lastname, string city, string street,
                      string area, string dateofbirth, string email, string phonenumber, string password, string descr, string isadmin);
        Task<MessageLogger> UpdateUserProfile(User user, string username, string firstname, string lastname, string city, string street,
              string area, string dateofbirth, string email, string phonenumber, string password, string descr);
        Task<MessageLogger> Register(User user, string password, string firstname, string lastname, string city, string area, string street,
                            string phonenumber, DateTime birthdate, string email, short isadmin, string imgurl, string descr, string publicid);
        Task<MessageLogger> ResetPassword(string email);
        Task<MessageLogger> AddImageForUser(PhotoForCreation photoForCreation);
        Task<MessageLogger> Create_And_WriteXLS_Users(string id, List<CustomUsers> customUsers);
    }
}
