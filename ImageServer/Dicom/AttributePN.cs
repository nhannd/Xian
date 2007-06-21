using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributePN : AttributeMultiValueText
    {
        #region Constructors

        public AttributePN(uint tag)
            : base(tag)
        {

        }

        public AttributePN(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.PNvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributePN(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributePN(AttributePN attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributePN(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributePN(this);
        }

    }
}
