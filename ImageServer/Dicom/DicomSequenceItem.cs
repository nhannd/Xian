using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class DicomSequenceItem : DicomAttributeCollection
    {
        public DicomSequenceItem() : base(0x00000000,0xFFFFFFFF)
        {
        }

        internal DicomSequenceItem(DicomAttributeCollection source, bool copyBinary)
            : base(source, copyBinary)
        {
        }

        public override DicomAttributeCollection Copy()
        {
            return Copy(true);
        }

        public override DicomAttributeCollection Copy(bool copyBinary)
        {
            return new DicomSequenceItem(this,copyBinary);
        }

    }
}
