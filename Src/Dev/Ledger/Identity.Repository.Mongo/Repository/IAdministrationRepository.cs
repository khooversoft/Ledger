using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo
{
    public interface IAdministrationRepository
    {
        Task ApplyCollectionModels(IWorkContext context);

        Task ResetCollections(IWorkContext context);
    }
}
