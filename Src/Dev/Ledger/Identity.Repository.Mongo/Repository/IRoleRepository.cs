using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo
{
    public interface IRoleRepository
    {
        Task<int> Delete(IWorkContext context, string roleId, string eTag = null);

        Task<HeaderDoc<UserRoleDoc>> Get(IWorkContext context, string roleId);

        Task<PageResult<HeaderDoc<UserRoleDoc>>> List(IWorkContext context, PageRequest pageRequest);

        Task<bool> Set(IWorkContext context, UserRoleDoc userRole, string eTag = null);
    }
}
