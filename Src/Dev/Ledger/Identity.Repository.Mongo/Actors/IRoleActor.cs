using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo
{
    public interface IRoleActor
    {
        Task<int> Delete(IWorkContext context, string eTag = null);

        Task<HeaderDoc<UserRoleDoc>> Get(IWorkContext context);

        Task<bool> Set(IWorkContext context, UserRoleDoc userRole, string eTag = null);
    }
}
