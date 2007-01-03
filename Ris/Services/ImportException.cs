using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Services
{
    public class ImportException : Exception
    {
        public ImportException(string message)
            :base(message)
        {

        }
    }
}
