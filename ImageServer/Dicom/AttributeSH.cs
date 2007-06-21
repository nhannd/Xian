using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeSH : AttributeMultiValueText
    {
        #region Constructors

        public AttributeSH(uint tag)
            : base(tag)
        {

        }

        public AttributeSH(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SHvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeSH(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeSH(AttributeSH attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeSH(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeSH(this);
        }

    }
}
