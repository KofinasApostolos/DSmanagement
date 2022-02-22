using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using static WebApi.Helpers.MessageLogger;

namespace WebApi.Data
{
    public class RolesRepository : IRolesRepository
    {

        private readonly DanceSchoolContext _context;
        public RolesRepository(DanceSchoolContext context)
        {
            _context = context;
        }

        public async Task<MessageLogger> DeleteRole(short id)
        {
            MessageLogger messageLogger = new MessageLogger();
            var roledeleted = (object)null;
            try
            {
                roledeleted = await (from role in _context.Roles
                                     where role.RoleCode == id
                                     select role).FirstOrDefaultAsync();

                if (roledeleted != null)
                {
                    _context.Remove(roledeleted);
                    await _context.SaveChangesAsync();
                }

                messageLogger.AddMessage("Role deleted", roledeleted, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetRole(short id)
        {
            MessageLogger messageLogger = new MessageLogger();
            var role = (object)null;
            try
            {
                role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleCode == id);

                messageLogger.AddMessage("", role, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> GetRoles()
        {
            MessageLogger messageLogger = new MessageLogger();
            var roles = (object)null;
            try
            {
                roles = await _context.Roles.ToListAsync();

                messageLogger.AddMessage("", roles, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> Register(Roles roles, short RoleCode, string RoleDescr)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                roles.RoleCode = RoleCode;
                roles.RoleDescr = RoleDescr;

                await _context.Roles.AddAsync(roles);
                await _context.SaveChangesAsync();

                messageLogger.AddMessage("Role inserted", roles, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> RoleExists(short id)
        {
            MessageLogger messageLogger = new MessageLogger();
            var role = (object)null;
            try
            {
                role = await (from r in _context.Roles
                              where r.RoleCode == id
                              select r).FirstOrDefaultAsync();

                if (role != null)
                {
                    messageLogger.AddMessage("Role exist", null, MessageCode.Information);
                    return messageLogger;
                }
                else
                {
                    messageLogger.AddMessage("", role, MessageCode.Information);
                    return messageLogger;
                }
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }

        public async Task<MessageLogger> UpdateRole(Roles roles, short RoleCode, string RoleDescr)
        {
            MessageLogger messageLogger = new MessageLogger();
            try
            {
                roles = await (from r in _context.Roles
                                   where r.RoleCode == RoleCode
                                   select r).FirstOrDefaultAsync();

                if (roles != null)
                {

                    roles.RoleDescr = RoleDescr;
                    await _context.SaveChangesAsync();

                }
                messageLogger.AddMessage("Role updated", roles, MessageCode.Information);
                return messageLogger;
            }
            catch (Exception ex)
            {
                messageLogger.AddMessage(ex.InnerException.Message, null, MessageCode.Error);
                return messageLogger;
            }
        }
    }
}
