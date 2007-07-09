using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom.Exceptions
{
    class DicomDataException : Exception
    {
        public DicomDataException(String desc)
            : base(desc)
        {
        }
    }
}
