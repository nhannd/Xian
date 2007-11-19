using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// When applied to a service contract interface, specifies whether that service requires
    /// authentication or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AuthenticationAttribute : Attribute
    {
        private readonly bool _required;

        public AuthenticationAttribute(bool required)
        {
            _required = required;
        }

        /// <summary>
        /// Gets a value indicating whether authentication is required.
        /// </summary>
        public bool AuthenticationRequired
        {
            get { return _required; }
        }
    }
}
