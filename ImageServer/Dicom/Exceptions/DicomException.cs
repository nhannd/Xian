using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom.Exceptions
{
    public class DicomException : Exception
    {
        public DicomException(String desc)
            : base(desc)
        {
        }

    }
}
