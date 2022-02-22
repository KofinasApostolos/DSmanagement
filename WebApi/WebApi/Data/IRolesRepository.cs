using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Data
{
    public interface IRolesRepository
    {
        Task<MessageLogger> Register(Roles roles, short RoleCode, string RoleDescr);
        Task<MessageLogger> GetRoles();
        Task<MessageLogger> GetRole(short id);
        Task<MessageLogger> DeleteRole(short id);
        Task<MessageLogger> RoleExists(short id);
        Task<MessageLogger> UpdateRole(Roles roles, short RoleCode, string RoleDescr);
    }
}
