
using System;
using System.Security.Principal;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Custom Identity
    /// </summary>
    public class CustomIdentity : GenericIdentity
    {
        private readonly string _displayName;

        public CustomIdentity(string loginId, string displayName)
            : base(loginId)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// Formatted name of the identity
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
        }
    }
}