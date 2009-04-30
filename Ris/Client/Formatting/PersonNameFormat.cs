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
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class PersonNameFormat
    {
        /// <summary>
        /// Formats the person name according to the default person name format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        public static string Format(PersonNameDetail pn)
        {
            return Format(pn, FormatSettings.Default.PersonNameDefaultFormat);
        }

        /// <summary>
        /// Formats the specified person name according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %F - full family name
        ///     %f - family name initial
        ///     %G - full given name
        ///     %g - given name initial
        ///     %M - full middle name
        ///     %m - middle initial
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(PersonNameDetail pn, string format)
        {
            // G g F f M m
            string result = format;
            result = result.Replace("%G", pn.GivenName == null ? "" : pn.GivenName);
            result = result.Replace("%g", pn.GivenName == null || pn.GivenName.Length == 0 ? "" : pn.GivenName.Substring(0, 1));
            result = result.Replace("%F", pn.FamilyName == null ? "" : pn.FamilyName);
            result = result.Replace("%f", pn.FamilyName == null || pn.FamilyName.Length == 0 ? "" : pn.FamilyName.Substring(0, 1));
            result = result.Replace("%M", pn.MiddleName == null ? "" : pn.MiddleName);
            result = result.Replace("%m", pn.MiddleName == null || pn.MiddleName.Length == 0 ? "" : pn.MiddleName.Substring(0, 1));

            return result.Trim();
        }
    }
}
