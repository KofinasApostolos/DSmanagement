using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IAuthRepository
    {
        Task<MessageLogger> Login(string username, string password, IConfiguration configuration);
        Task<MessageLogger> LoginSocial(string firstname, string lastname, string email, IConfiguration configuration);
        Task<bool> UserExists(string username);
    }
}
