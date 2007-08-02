using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public class EnumValueNotFoundException : Exception
    {
        public EnumValueNotFoundException(Type enumClass, string enumCode, Exception inner)
            : base(string.Format(SR.ExceptionEnumValueNotFound, enumCode, enumClass.FullName), inner)
        {
        }
   }
}
