using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo
{
    public interface IUserRepository
    {
        Task<int> Delete(IWorkContext context, string userName, string eTag = null);

        Task<HeaderDoc<UserDoc>> Get(IWorkContext context, string userName);

        Task<PageResult<HeaderDoc<UserDoc>>> List(IWorkContext context, PageRequest pageRequest);

        Task<bool> Set(IWorkContext context, UserDoc userDoc, string eTag = null);
    }
}
