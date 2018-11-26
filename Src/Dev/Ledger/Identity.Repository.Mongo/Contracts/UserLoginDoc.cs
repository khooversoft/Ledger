using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Repository.Mongo
{
    public class UserLoginDoc
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}
