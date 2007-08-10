using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    class DicomDataException : DicomException
    {
        public DicomDataException(String desc)
            : base(desc)
        {
        }
    }
}
