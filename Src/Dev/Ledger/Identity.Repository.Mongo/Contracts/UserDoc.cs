using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class UserDoc
    {
        // Unique index
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        // Non-unique index
        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.Int64)]
        public DateTime? LockoutEnd { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public IList<UserTokenDoc> Tokens { get; set; }

        public IList<UserClaimDoc> UserTokens { get; set; }

        public IList<string> Roles { get; set; }

        public IList<UserLoginDoc> Logins { get; set; }
    }
}
