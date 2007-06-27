using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Dicom.Exceptions
{
    public class NetworkException : DicomException
    {
        public NetworkException(String desc)
            : base(desc)
        {
        }
    }
}
