using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeTM : AttributeMultiValueText
    {
        #region Constructors

        public AttributeTM(uint tag)
            : base(tag)
        {

        }

        public AttributeTM(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.TMvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeTM(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeTM(AttributeTM attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeTM(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeTM(this);
        }

    }
}
