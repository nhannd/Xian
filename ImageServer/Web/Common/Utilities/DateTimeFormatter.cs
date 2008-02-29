#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Globalization;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    /// <summary>
    /// Provides methods to convert date time values into user-friendly format which can be used on the UI
    /// </summary>
    static public class DateTimeFormatter
    {
        /// <summary>
        /// Style of the date/time when rendered on the UI
        /// </summary>
        public enum DateTimeUIStyles
        {
            DATE_ONLY,
            TIME_ONLY
        }

        /// <summary>
        /// Converts the specified date or time object into a string representation suitable for rendering on the UI.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        static public string Format(DateTime dt, DateTimeUIStyles style)
        {
            switch(style)
            {
                case DateTimeUIStyles.DATE_ONLY:
                    return dt.ToString("d");

                case DateTimeUIStyles.TIME_ONLY:
                    return dt.ToString("T");

                default:
                    return dt.ToString();
            }
        }

        /// <summary>
        /// Converts the specified string representation of a date (DICOM DA VR) into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="DAValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        static public bool TryParseDA(string DAValue, out DateTime result)
        {
            DateTime dt;
            if (!String.IsNullOrEmpty(DAValue))
            {
                if (DateTime.TryParseExact(DAValue, "yyyyMMdd", null, DateTimeStyles.AssumeLocal, out result))
                {
                    return true;
                }
            }
            result = DateTime.MinValue;
            return false;
        }

        /// <summary>
        /// Converts the specified string representation of a time (DICOM TM VR) into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="TMValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        static public bool TryParseTM(string TMValue, out DateTime result)
        {
            DateTime dt;
            if (!String.IsNullOrEmpty(TMValue))
            {
                if (DateTime.TryParseExact(TMValue, new string[]{
                                        "HHmmss.ffff",
                                        "HHmmss.fff",
                                        "HHmmss.ff",
                                        "HHmmss.f",
                                        "HHmmss",
                                        "HHmm",
                                        "HHmm",
                                        "HH"
                                    }, null, DateTimeStyles.AssumeLocal, out result))
                {
                    return true;
                }
            }
            result = DateTime.MinValue;
            return false;
        }
        
        /// <summary>
        /// Takes a value of DICOM DA VR (yyyyMMdd) and converts it into a more user-friendly string that's suitable to be used on the UI (eg mm/dd/yyyy)
        /// </summary>
        /// <param name="DAValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        static public bool TryFormatDA(string DAValue, out string result)
        {
            DateTime dt;
            if (!String.IsNullOrEmpty(DAValue))
            {
                if (DateTime.TryParseExact(DAValue, "yyyyMMdd", null, DateTimeStyles.AssumeLocal, out dt))
                {
                    result = Format(dt, DateTimeUIStyles.DATE_ONLY);
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        ///  Takes a value of DICOM DA TM (hhmmss) and converts it into  a more user-friendly string that's suitable to be used on the UI (eg, hh:mm:ss AM/PM)
        /// </summary>
        /// <param name="TMValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        static public bool TryFormatTM(string TMValue, out string result)
        {
            DateTime dt;
            if (!String.IsNullOrEmpty(TMValue))
            {
                if (DateTime.TryParseExact(TMValue, new string[]{
                                        "HHmmss.ffff",
                                        "HHmmss.fff",
                                        "HHmmss.ff",
                                        "HHmmss.f",
                                        "HHmmss",
                                        "HHmm",
                                        "HHmm",
                                        "HH"
                                    }, null, DateTimeStyles.AssumeLocal, out dt))
                {
                    result = Format(dt, DateTimeUIStyles.TIME_ONLY);
                    return true;
                }
            }

            result = null;
            return false;
        }

    }
}
