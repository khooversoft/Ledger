using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class UserClaimDoc
    {
        /// <summary>
        /// The type of the claim.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The value of the claim.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The issuer of the claim.
        /// </summary>
        public string Issuer { get; set; }
    }
}
