//using Khooversoft.MongoDb;
//using Khooversoft.Toolbox;
//using Khooversoft.Toolbox.Actor;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Identity.Repository.Mongo
//{
//    public interface IRoleActor
//    {
//        Task Set(IWorkContext context, UserRoleDoc userRole);

//        Task<UserRoleDoc> Get(IWorkContext context);

//        Task Delete(IWorkContext context);

//        Task<PageResult<UserRoleDoc>> List(IWorkContext context, PageRequest pageRequest);
//    }

//    public class RoleActor : ActorBase, IRoleActor
//    {
//        private readonly CacheObject<UserRoleDoc> _cacheUser = new CacheObject<UserRoleDoc>(TimeSpan.FromMinutes(10));
//        private readonly IIdentityConfiguration _configuration;
//        private readonly IRoleRepository _roleRepository;

//        public RoleActor(ActorKey actorKey, IActorManager actorManager, IRoleRepository roleRepository)
//            : base(actorKey, actorManager)
//        {
//            Verify.IsNotNull(nameof(roleRepository), roleRepository);

//            _roleRepository = roleRepository;
           
//        }

//        public Task Delete(IWorkContext context)
//        {
//            _cacheUser.Clear();

//            var query = new And()
//                + (new Field(nameof(UserRoleDoc.RoleId)) == ActorKey.VectorKey);

//            return _documentServer.
//        }

//        public Task<UserRoleDoc> Get(IWorkContext context)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<PageResult<UserRoleDoc>> List(IWorkContext context, PageRequest pageRequest)
//        {
//            throw new NotImplementedException();
//        }

//        public Task Set(IWorkContext context, UserRoleDoc userRole)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
