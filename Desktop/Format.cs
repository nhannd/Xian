using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public static class Format
    {
        private static readonly string _dateFormat = "dd-MMM-yyyy";
        private static readonly string _timeFormat = "hh:mm tt";
        private static readonly string _dateTimeFormat = _dateFormat + " " + _timeFormat;

        public static string DateFormat
        {
            get { return _dateFormat; }
        }

        public static string TimeFormat
        { 
            get { return _timeFormat; } 
        }

        public static string DateTimeFormat
        {
            get { return _dateTimeFormat; }
        }

        public static string Date(DateTime dt)
        {
            return dt.ToString(_dateFormat);
        }

        public static string Date(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(_dateFormat);
        }

        public static string Time(DateTime dt)
        {
            return dt.ToString(_timeFormat);
        }

        public static string Time(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(_timeFormat);
        }

        public static string DateTime(DateTime dt)
        {
            return dt.ToString(_dateTimeFormat);
        }

        public static string DateTime(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(_dateTimeFormat);
        }

    }
}
