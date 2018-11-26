using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class UserRoleDoc
    {
        // Unique index
        public string RoleId { get; set; }

        public IList<UserClaimDoc> Claims { get; set; }
    }
}
