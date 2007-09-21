using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public class ModellingException : Exception
    {
        public ModellingException(string message)
            :base(message)
        {

        }
    }
}
