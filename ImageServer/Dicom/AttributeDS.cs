using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeDS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDS(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeDS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDS(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeDS(AttributeDS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDS(this);
        }

    }
}
