
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
        public DicomException(String desc, Exception e)
            : base(desc,e)
        {
        }
        protected DicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
