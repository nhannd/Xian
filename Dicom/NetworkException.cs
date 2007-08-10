using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public class NetworkException : DicomException
    {
        public NetworkException(String desc)
            : base(desc)
        {
        }
    }
}
