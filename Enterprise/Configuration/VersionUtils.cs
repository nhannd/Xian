#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        /// xxxxx.xxxxx.xxxxx.xxxxx - this format allows version strings to be compared.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ToPaddedVersionString(Version v)
        {
        	return ToPaddedVersionString(v, true, true);
        }

		/// <summary>
		/// Converts the specified version to a padded version string, which always has the form
		/// xxxxx.xxxxx.xxxxx.xxxxx, optionally including the build and revision parts.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="includeBuildPart"></param>
		/// <param name="includeRevisionPart"></param>
		/// <returns></returns>
        public static string ToPaddedVersionString(Version v, bool includeBuildPart, bool includeRevisionPart)
        {
			Platform.CheckForNullReference(v, "value");

			//major.minor.build.revision
			StringBuilder sb = new StringBuilder();
			sb.Append(v.Major.ToString("d5"));
			sb.Append(".");
			sb.Append(v.Minor.ToString("d5"));

			if(includeBuildPart)
			{
				sb.Append(".");
				sb.Append(v.Build.ToString("d5"));
				if (includeRevisionPart)
				{
					sb.Append(".");
					sb.Append(v.Revision.ToString("d5"));
				}
			}


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
