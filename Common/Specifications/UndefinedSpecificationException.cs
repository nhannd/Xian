using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class UndefinedSpecificationException : SpecificationException
    {
        public UndefinedSpecificationException(string id)
            :base(string.Format("{0} is not a valid specification id", id))
        {
        }
    }
}
