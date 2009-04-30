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
    public static class HealthcardFormat
    {
        /// <summary>
        /// Formats the healthcard according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc)
        {
            return Format(hc, FormatSettings.Default.HealthcardDefaultFormat);
        }

        /// <summary>
        /// Formats the healthcard number according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - number
        ///     %A - assigning authority
        ///     %V - version code
        ///     %X - expiry date
        /// </remarks>
        /// <param name="hc"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc, string format)
        {
            string result = format;
            result = result.Replace("%N", hc.Id ?? "");
            result = result.Replace("%A", hc.AssigningAuthority == null ? "" : hc.AssigningAuthority.Code);
            result = result.Replace("%V", hc.VersionCode ?? "");
            result = result.Replace("%X", ClearCanvas.Desktop.Format.Date(hc.ExpiryDate));

            return result.Trim();
        }
    }
}
