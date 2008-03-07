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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Provides unified methods to convert a datetime object into a string representation suitable for display on a web page.
    /// </summary>
    /// <remarks>
    /// Use <see cref="DateTimeFormatter"/> to ensure consistent date/time format is generated in the entire applications. The date/time 
    /// format is specified in the configuration or taken from the the system region setting.
    /// 
    /// </remarks>
    /// <example>
    /// </example>
    static class DateTimeFormatter
    {
        /// <summary>
        /// The output date time style.
        /// </summary>
        public enum Style
        {
            DateTime, // output both date and time
            DateOnly, // output the date only
            TimeOnly  // output the time only
        }

        #region Private members
        private static string _defaultDateTimeFormat; 
        private static string _defaultDateFormat; 
        private static string _defaultTimeFormat;

        #endregion Private members


        #region Constructor

        static DateTimeFormatter()
        {
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            DateTimeFormatInfo fmt = culture.DateTimeFormat;
            
            DefaultDateFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.DateFormat))
            {
                if (fmt != null)
                {
                    DefaultDateFormat = fmt.LongDatePattern;

                }
                else
                {
                    DefaultDateFormat = "MMM dd, yyyy";

                }
            }
            else
            {
                // use the defined format
                DefaultDateFormat = UISettings.Default.DateFormat;
            }

            DefaultTimeFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.TimeFormat))
            {
                if (fmt != null)
                {
                    DefaultTimeFormat = fmt.LongTimePattern;

                }
                else
                {
                    DefaultTimeFormat = "hh:mm:ss tt";

                }
            }
            else
            {
                // use the defined format
                DefaultTimeFormat = UISettings.Default.TimeFormat;
            }


            _defaultDateTimeFormat = null;
            if (String.IsNullOrEmpty(UISettings.Default.DateTimeFormat))
            {
                if (fmt != null)
                {
                    _defaultDateTimeFormat = fmt.LongDatePattern;

                }
                else
                {
                    _defaultDateTimeFormat = _defaultDateTimeFormat + " " + _defaultTimeFormat;

                }    
            }
            else
            {
                // use the defined format
                _defaultDateTimeFormat = UISettings.Default.DateTimeFormat;
            }


        }

        #endregion Constructor

        #region Public Properties
        static string DefaultDateTimeFormat
        {
            get
            {
                return _defaultDateTimeFormat;
            }
            set
            {
                _defaultDateTimeFormat = value;
            }
        }

        public static string DefaultDateFormat
        {
            get { return _defaultDateFormat; }
            set { _defaultDateFormat = value; }
        }

        public static string DefaultTimeFormat
        {
            get { return _defaultTimeFormat; }
            set { _defaultTimeFormat = value; }
        }
        #endregion Public Properties

        #region Public methods
        static public string Format(DateTime dt)
        {
            return Format(dt, Style.DateTime);
        }

        /// <summary>
        /// Formats the specified date/time
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="outputStyle"></param>
        /// <returns></returns>
        static public string Format(DateTime dt, Style outputStyle)
        {
            switch(outputStyle)
            {
                case Style.DateTime:
                    return dt.ToString(DefaultDateTimeFormat);
                case Style.DateOnly:
                    return dt.ToString(DefaultDateFormat);

                case Style.TimeOnly:
                    return dt.ToString(DefaultTimeFormat);

                default:
                    return dt.ToString();
            }
        }

        #endregion Public methods
    }
}
