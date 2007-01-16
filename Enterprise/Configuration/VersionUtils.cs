using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Utilities related to the <see cref="System.Version"/> class.
    /// </summary>
    static class VersionUtils
    {
        /// <summary>
        /// Converts the specified version to a padded version string, which always has the form
        /// xxxxx.xxxxx.xxxxx.xxxx - this format allows version strings to be compared.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ToPaddedVersionString(Version v)
        {
            Platform.CheckForNullReference(v, "value");

            StringBuilder sb = new StringBuilder();
            sb.Append(v.Major.ToString("d5"));
            sb.Append(".");
            sb.Append(v.Minor.ToString("d5"));
            sb.Append(".");
            sb.Append(v.Build.ToString("d5"));
            sb.Append(".");
            sb.Append(v.Revision.ToString("d5"));

            return sb.ToString();
        }

        /// <summary>
        /// Converts a padded version string to a <see cref="System.Version"/>
        /// </summary>
        /// <param name="pvs"></param>
        /// <returns></returns>
        public static Version FromPaddedVersionString(string pvs)
        {
            return new Version(pvs);
        }
    }
}
