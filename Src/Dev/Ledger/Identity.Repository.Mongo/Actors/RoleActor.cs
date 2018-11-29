using Khooversoft.MongoDb;
using Khooversoft.Toolbox;
using Khooversoft.Toolbox.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Repository.Mongo
{
    public class RoleActor : ActorBase, IRoleActor
    {
        private readonly CacheObject<HeaderDoc<UserRoleDoc>> _cache = new CacheObject<HeaderDoc<UserRoleDoc>>(TimeSpan.FromMinutes(10));
        private readonly IRoleRepository _roleRepository;

        public RoleActor(ActorKey actorKey, IActorManager actorManager, IRoleRepository roleRepository)
            : base(actorKey, actorManager)
        {
            Verify.IsNotNull(nameof(roleRepository), roleRepository);

            _roleRepository = roleRepository;

        }

        public Task<int> Delete(IWorkContext context, string eTag = null)
        {
            if (eTag.IsEmpty() && _cache.TryGetValue(out HeaderDoc<UserRoleDoc> value))
            {
                eTag = value?.ETag;
            }

            _cache.Clear();
            return _roleRepository.Delete(context, ActorKey.VectorKey, eTag);
        }

        public async Task<HeaderDoc<UserRoleDoc>> Get(IWorkContext context)
        {
            if (_cache.TryGetValue(out HeaderDoc<UserRoleDoc> value))
            {
                return value;
            }

            HeaderDoc<UserRoleDoc> result = await _roleRepository.Get(context, ActorKey.VectorKey);
            if (result == null)
            {
                return null;
            }

            _cache.Set(result);
            return result;
        }

        public async Task<bool> Set(IWorkContext context, UserRoleDoc userRole, string eTag = null)
        {
            if (eTag.IsEmpty() && _cache.TryGetValue(out HeaderDoc<UserRoleDoc> value))
            {
                eTag = value?.ETag;
            }

            _cache.Clear();
            return await _roleRepository.Set(context, userRole, eTag);
        }
    }
}
