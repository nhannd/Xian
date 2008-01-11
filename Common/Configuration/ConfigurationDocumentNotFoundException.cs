using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Exception indicates that a requested configuration document does not exist.
    /// </summary>
    public class ConfigurationDocumentNotFoundException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        public ConfigurationDocumentNotFoundException(string name, Version version, string user, string instanceKey)
            :base(FormatMessage(name, version, user, instanceKey))
        {
        }

        private static string FormatMessage(string name, Version version, string user, string instanceKey)
        {
            return string.Format("The document {0}, Version={1}, User={2}, Instance={3} does not exist.",
                          name, version, user, instanceKey);
        }
    }
}
