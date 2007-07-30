
namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    [Serializable]
    public class DicomException : Exception
    {
        public DicomException(String desc)
            : base(desc)
        {
        }
        protected DicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
