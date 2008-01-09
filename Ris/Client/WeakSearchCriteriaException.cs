using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public class WeakSearchCriteriaException : Exception
    {
        public WeakSearchCriteriaException()
            :base(SR.ExceptionWeakSearchCriteria)
        {
        }
    }
}
