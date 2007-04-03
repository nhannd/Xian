using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class AuthorityTokenAttribute : Attribute
    {
        private string _description;

        public AuthorityTokenAttribute()
        {
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
