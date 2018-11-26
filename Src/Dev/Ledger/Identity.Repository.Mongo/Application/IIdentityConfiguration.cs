using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public interface IIdentityConfiguration
    {
        string ConnectionString { get; }

        IActorManager ActorManager { get; }

        string DatabaseName { get; }

        string IdentityRoleCollectionName { get; }

        string IdentityUserCollectionName { get; }
    }
}
