using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class DicomSequenceItem : AttributeCollection
    {
        public DicomSequenceItem() : base()
        {
        }

        internal DicomSequenceItem(AttributeCollection source, bool copyBinary)
            : base(source, copyBinary)
        {
        }

        public override AttributeCollection Copy()
        {
            return Copy(true);
        }

        public override AttributeCollection Copy(bool copyBinary)
        {
            return new DicomSequenceItem(this,copyBinary);
        }

    }
}
