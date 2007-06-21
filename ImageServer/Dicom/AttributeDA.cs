using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeDA : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDA(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeDA(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DAvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDA(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeDA(AttributeDA attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDA(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDA(this);
        }

    }
}
