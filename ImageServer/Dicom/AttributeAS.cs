using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;


namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeAS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeAS(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeAS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ASvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeAS(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeAS(AttributeAS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeAS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeAS(this);
        }

    }
}
