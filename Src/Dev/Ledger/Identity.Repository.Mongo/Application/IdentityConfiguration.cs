using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class IdentityConfiguration : IIdentityConfiguration
    {
        public IdentityConfiguration(string connectionString, string databaseName, IActorManager actorManager = null)
        {
            Verify.IsNotEmpty(nameof(connectionString), connectionString);
            Verify.IsNotEmpty(nameof(databaseName), databaseName);

            ConnectionString = connectionString;
            DatabaseName = databaseName;
            ActorManager = actorManager ?? new ActorManager();
        }

        public string ConnectionString { get; }

        public IActorManager ActorManager { get; }

        public string DatabaseName { get; }

        public string IdentityRoleCollectionName { get; } = "IdentityRoles";

        public string IdentityUserCollectionName { get; } = "IdentityUser";
    }
}
