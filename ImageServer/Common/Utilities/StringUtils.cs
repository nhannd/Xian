using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class StringUtils
    {
        public static bool AreEqual(string x, string y)
        {
            if (String.IsNullOrEmpty(x) && String.IsNullOrEmpty(y))
                return true;

            return x.Equals(y);
        }
    }
}
