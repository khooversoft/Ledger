using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Khooversoft.MongoDb;
using Khooversoft.Toolbox;

namespace Identity.Repository.Mongo
{
    public class AdministrationRepository : IAdministrationRepository
    {
        private readonly IIdentityConfiguration _configuration;
        private readonly IDocumentServer _documentServer;
        private readonly CollectionModel[] _models;
        private static readonly Tag _tag = new Tag(nameof(AdministrationRepository));

        public AdministrationRepository(IIdentityConfiguration identityConfiguration)
        {
            Verify.IsNotNull(nameof(identityConfiguration), identityConfiguration);
            Verify.IsNotEmpty(nameof(identityConfiguration.DatabaseName), identityConfiguration.DatabaseName);
            Verify.IsNotEmpty(nameof(identityConfiguration.IdentityRoleCollectionName), identityConfiguration.IdentityRoleCollectionName);
            Verify.IsNotEmpty(nameof(identityConfiguration.IdentityUserCollectionName), identityConfiguration.IdentityUserCollectionName);

            _configuration = identityConfiguration;
            _documentServer = new DocumentServer(_configuration.ConnectionString);

            _models = new CollectionModel[]
            {
                new CollectionModel
                {
                    CollectionName = identityConfiguration.IdentityRoleCollectionName,
                    Indexes = new CollectionIndex[]
                    {
                        new CollectionIndex
                        {
                            Name = $"{nameof(UserRoleDoc.RoleId)}_index",
                            Unique = true,
                            Keys = new IndexKey[]
                            {
                                new IndexKey { FieldName = HeaderDoc.FieldName(nameof(UserRoleDoc.RoleId)), Descending = false },
                            }
                        }
                    }
                },

                new CollectionModel
                {
                    CollectionName = identityConfiguration.IdentityUserCollectionName,
                    Indexes = new CollectionIndex[]
                    {
                        new CollectionIndex
                        {
                            Name = $"{nameof(UserDoc.NormalizedEmail)}_index",
                            Unique = false,
                            Keys = new IndexKey[]
                            {
                                new IndexKey { FieldName = HeaderDoc.FieldName(nameof(UserDoc.NormalizedEmail)), Descending = false },
                            }
                        },
                        new CollectionIndex
                        {
                            Name = $"{nameof(UserDoc.NormalizedUserName)}_index",
                            Unique = true,
                            Keys = new IndexKey[]
                            {
                                new IndexKey { FieldName = HeaderDoc.FieldName(nameof(UserDoc.NormalizedUserName)), Descending = false },
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Apply collection model, create collection and its create indexes
        /// </summary>
        /// <param name="context">context</param>
        /// <returns>task</returns>
        public async Task ApplyCollectionModels(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IDocumentDatabase db = _documentServer.GetDatabase(_configuration.DatabaseName);

            foreach (var model in _models)
            {
                context.Telemetry.Info(context, $"{nameof(ApplyCollectionModels)} for collection={model.CollectionName}");

                var package = new CollectionModelPackage(db, model);
                await package.Apply(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reset collections, destroy and re-create collections
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ResetCollections(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            IDocumentDatabase db = _documentServer.GetDatabase(_configuration.DatabaseName);

            foreach (var model in _models)
            {
                context.Telemetry.Info(context, $"{nameof(ResetCollections)} for collection={model.CollectionName}");
                await db.DropCollection(context, model.CollectionName).ConfigureAwait(false);
            }

            await ApplyCollectionModels(context).ConfigureAwait(false);
        }
    }
}
