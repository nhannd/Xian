using System;
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class DicomNameUtils
    {
        [Flags]
        public enum NormalizeOptions
        {
            TrimSpaces,
            TrimEmptyEndingComponents
        }

        static bool IsSet(NormalizeOptions value, NormalizeOptions flag)
        {
            return (value & flag) == flag;
        }

        public static String Normalize(string name, NormalizeOptions options)
        {
            string value = name;
            if (IsSet(options, NormalizeOptions.TrimSpaces))
            {
                value = Regex.Replace(value, "[ ]+", " "); // remove 
                value = Regex.Replace(value, "[ ]+\\^[ ]+|\\^[ ]+|[ ]+\\^", "^"); 
            }

            if (IsSet(options, NormalizeOptions.TrimEmptyEndingComponents))
            {
                value = Regex.Replace(value, "[\\^]*$", ""); // remove}
            }
            return value;
        }
    }
}
