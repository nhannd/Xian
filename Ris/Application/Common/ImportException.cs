using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Application.Common
{
    public class ImportException : Exception
    {
        public ImportException(string message)
            :base(message)
        {

        }
    }
}
