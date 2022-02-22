using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Class;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DanceSchoolContext _context;
        public AuthRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> Login(string username, string password, IConfiguration configuration)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                var dbuser = await (from u in _context.User
                                    where u.Username == username
                                    where u.Password == PasswordHash.encryptPassword(password)
                                    select u).FirstOrDefaultAsync();


                if (dbuser == null)
                {
                    messageLogger.AddMessage("Wrong username or password", null, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    User user = new User();
                    user.Email = dbuser.Email;
                    user.FirstName = dbuser.FirstName;
                    user.ImageUrl = dbuser.ImageUrl;
                    user.IsAdmin = dbuser.IsAdmin;
                    user.LastName = dbuser.LastName;
                    user.City = dbuser.City;
                    user.Descr = dbuser.Descr;
                    user.Area = dbuser.Area;
                    user.Phonenumber = dbuser.Phonenumber;
                    user.Street = dbuser.Street;
                    user.Password = PasswordHash.decryptPassword(dbuser.Password);
                    user.PublicId = dbuser.PublicId;
                    user.DateofBirth = dbuser.DateofBirth;
                    user.Id = dbuser.Id;
                    user.Username = dbuser.Username;

                    var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.IsAdmin.ToString()),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = creds
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    messageLogger.AddMessage("Login success", user, MessageCode.Information);
                    messageLogger.AddMessage("Login success", tokenHandler.WriteToken(token), MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Information);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> LoginSocial(string firstname, string lastname, string email, IConfiguration configuration)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                //check if user exist based on email
                var dbuser = await (from u in _context.User
                                    where u.Email == email.Trim()
                                    select u).FirstOrDefaultAsync();

                if (dbuser == null)
                {
                    User user = new User
                    {
                        Email = email,
                        Username = null,
                        LastName = lastname,
                        FirstName = firstname,
                        Password = null,
                        Area = null,
                        City = null,
                        ImageUrl = null,
                        DateofBirth = null,
                        IsAdmin = 3,
                        Phonenumber = null,
                        Street = null,
                        Descr = null,
                        PublicId = null,
                        Provider = "Facebook"
                    };

                    await _context.User.AddAsync(user);
                    await _context.SaveChangesAsync();

                    await Task.Run(() => SetUserToken(user, configuration, messageLogger));
                    //await SetUser(user, configuration, messageLogger);
                    messageLogger.AddMessage("Login success", user, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    await Task.Run(() => SetUserToken(dbuser, configuration, messageLogger));
                    //await SetUser(dbuser, configuration, messageLogger);
                    messageLogger.AddMessage("Login success", dbuser, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }


        private async void SetUserToken(User dbuser, IConfiguration configuration, MessageLogger messageLogger)
        {
            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, dbuser.Id.ToString()),
                    new Claim(ClaimTypes.Name, dbuser.LastName + " " + dbuser.FirstName),
                    new Claim(ClaimTypes.Role, dbuser.IsAdmin.ToString()),
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            messageLogger.AddMessage("Login success", tokenHandler.WriteToken(token), MessageCode.Information);
        }

        public async Task<bool> UserExists(string username)
        {
            try
            {
                if (await _context.User.AnyAsync(x => x.Username == username))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
