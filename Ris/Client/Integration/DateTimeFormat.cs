#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.Integration
{
    public static class DateTimeFormat
    {
        /// <summary>
        /// Formats a date according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     YYYY - year
        ///     MM   - month
        ///     DD   - day
        ///     hh   - hour
        ///     mm   - minute
        ///     ss   - second
        ///     xxx   - millisecond
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(DateTime dateTime, string format)
        {
            string result = format;
            result = result.Replace("YYYY", dateTime.Year.ToString());
            result = result.Replace("MM", dateTime.Month.ToString("00"));
            result = result.Replace("DD", dateTime.Day.ToString("00"));
            result = result.Replace("hh", dateTime.Hour.ToString("00"));
            result = result.Replace("mm", dateTime.Minute.ToString("00"));
            result = result.Replace("ss", dateTime.Second.ToString("00"));
            result = result.Replace("xxx", dateTime.Millisecond.ToString("000"));
            return result.Trim();
        }
    }
}
