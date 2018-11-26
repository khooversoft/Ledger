using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class UserTokenDoc
    {
        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public string Value { get; set; }
    }
}
