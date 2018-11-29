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
    public class UserRepository : IUserRepository
    {
        private readonly IIdentityConfiguration _configuration;
        private readonly IDocumentCollection<HeaderDoc<UserDoc>> _collection;
        private readonly Tag _tag;

        public UserRepository(IIdentityConfiguration identityConfiguration)
        {
            Verify.IsNotNull(nameof(identityConfiguration), identityConfiguration);

            _configuration = identityConfiguration;

            _collection = new DocumentServer(_configuration.ConnectionString)
                .GetDatabase(_configuration.DatabaseName)
                .GetCollection<HeaderDoc<UserDoc>>(_configuration.IdentityUserCollectionName);

            _tag = new Tag($"{nameof(UserRepository)}/{_configuration.DatabaseName}/{_configuration.IdentityRoleCollectionName}");
        }

        public Task<int> Delete(IWorkContext context, string userName, string eTag = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(userName), userName);
            context = context.WithTag(_tag);

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserDoc.NormalizedUserName))) == userName.ToLowerInvariant());

            if (eTag.IsNotEmpty())
            {
                query += (new Field(nameof(HeaderDoc<UserDoc>.ETag)) == eTag);
            }

            return _collection.Delete(context, query.ToDocument());
        }

        public async Task<HeaderDoc<UserDoc>> Get(IWorkContext context, string userName)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotEmpty(nameof(userName), userName);
            context = context.WithTag(_tag);

            var options = new driver.FindOptions<HeaderDoc<UserDoc>>
            {
                Limit = 1,
                Projection = "{'_id': 0}"
            };

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserDoc.NormalizedUserName))) == userName.ToLowerInvariant());

            var result = await _collection.Find(context, query.ToDocument(), options).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        public async Task<PageResult<HeaderDoc<UserDoc>>> List(IWorkContext context, PageRequest pageRequest)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(pageRequest), pageRequest);
            context = context.WithTag(_tag);

            var options = new driver.FindOptions<HeaderDoc<UserDoc>>
            {
                Limit = pageRequest.Limit,
                Projection = "{'_id': 0}"
            };

            IEnumerable<HeaderDoc<UserDoc>> result;

            if (pageRequest.Index.IsEmpty())
            {
                result = await _collection.Find(context, new BsonDocument(), options);
            }
            else
            {
                var query = new And()
                    + (new Field(HeaderDoc.FieldName(nameof(UserDoc.NormalizedUserName))) > pageRequest.Index);

                result = await _collection.Find(context, query.ToDocument(), options).ConfigureAwait(false);
            }

            return new PageResult<HeaderDoc<UserDoc>>(result.LastOrDefault()?.Payload?.NormalizedUserName, result);
        }

        public async Task<bool> Set(IWorkContext context, UserDoc userDoc, string eTag = null)
        {
            Verify.IsNotNull(nameof(context), context);
            Verify.IsNotNull(nameof(userDoc), userDoc);
            context = context.WithTag(_tag);

            userDoc.NormalizedUserName = userDoc.UserName.ToLowerInvariant();
            var envelope = new HeaderDoc<UserDoc>(userDoc);

            var query = new And()
                + (new Field(HeaderDoc.FieldName(nameof(UserDoc.NormalizedUserName))) == userDoc.NormalizedUserName);

            if (eTag.IsEmpty())
            {
                return await _collection.Upsert(context, query.ToDocument(), envelope).ConfigureAwait(false);
            }

            query += (new Field(nameof(HeaderDoc<UserDoc>.ETag)) == eTag);
            return await _collection.Update(context, query.ToDocument(), envelope).ConfigureAwait(false);
        }
    }
}
