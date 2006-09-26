using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public static class Format
    {
        public static string Date(DateTime dt)
        {
            return dt.ToShortDateString();
        }

        public static string Date(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToShortDateString();
        }

        public static string Time(DateTime dt)
        {
            return dt.ToShortTimeString();
        }

        public static string Time(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToShortTimeString();
        }

    }
}
