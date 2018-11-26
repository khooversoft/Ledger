using Khooversoft.MongoDb;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Services;
using driver = MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Identity.Repository.Mongo
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IIdentityConfiguration _configuration;
        private readonly IDocumentCollection<HeaderDoc<UserRoleDoc>> _collection;
        private readonly Tag _tag;

        public RoleRepository(IIdentityConfiguration identityConfiguration)
        {
            Verify.IsNotNull(nameof(identityConfiguration), identityConfiguration);

            _configuration = identityConfiguration;

            _collection = new DocumentServer(_configuration.ConnectionString)
                .GetDatabase(_configuration.DatabaseName)
                .GetCollection<HeaderDoc<UserRoleDoc>>(_configuration.IdentityRoleCollectionName);

            _tag = new Tag($"{nameof(RoleRepository)}/{_configuration.DatabaseName}/{_configuration.IdentityRoleCollectionName}");
        }

        public Task Delete(IWorkContext context, string roleId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(roleId), roleId);
            context = context.WithTag(_tag);

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserRoleDoc.RoleId))) == roleId);

            return _collection.Delete(context, query.ToDocument());
        }

        public async Task<HeaderDoc<UserRoleDoc>> Get(IWorkContext context, string roleId)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(roleId), roleId);
            context = context.WithTag(_tag);

            var options = new driver.FindOptions<HeaderDoc<UserRoleDoc>>
            {
                Limit = 1,
                Projection = "{'_id': 0}"
            };

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserRoleDoc.RoleId))) == roleId);

            var result = await _collection.Find(context, query.ToDocument(), options).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<PageResult<HeaderDoc<UserRoleDoc>>> List(IWorkContext context, PageRequest pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(pageRequest), pageRequest);
            context = context.WithTag(_tag);

            var options = new driver.FindOptions<HeaderDoc<UserRoleDoc>>
            {
                Limit = pageRequest.Limit,
                Projection = "{'_id': 0}"
            };

            IEnumerable<HeaderDoc<UserRoleDoc>> result;

            if (pageRequest.Index.IsEmpty())
            {
                result = await _collection.Find(context, new BsonDocument(), options);
            }
            else
            {
                var query = new And()
                    + (new Field(HeaderDoc.FieldName(nameof(UserRoleDoc.RoleId))) > pageRequest.Index);

                result = await _collection.Find(context, query.ToDocument(), options).ConfigureAwait(false);
            }

            return new PageResult<HeaderDoc<UserRoleDoc>>(result.LastOrDefault()?.Payload?.RoleId, result);
        }

        public async Task<bool> Set(IWorkContext context, UserRoleDoc userRole, string eTag = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(userRole), userRole);
            context = context.WithTag(_tag);

            var envelope = new HeaderDoc<UserRoleDoc>(userRole);

            if (eTag.IsEmpty())
            {
                await _collection.Insert(context, envelope).ConfigureAwait(false);
                return true;
            }

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserRoleDoc.RoleId))) == userRole.RoleId)
                + (new Field(nameof(HeaderDoc<UserRoleDoc>.ETag)) == eTag);

            bool result = await _collection.Update(context, query.ToDocument(), envelope).ConfigureAwait(false);
            if (!result)
            {
                throw new ETagException($"UserRole.RoleId={userRole.RoleId} is not up to date, request eTag={eTag}", context);
            }

            return result;
        }
    }
}
