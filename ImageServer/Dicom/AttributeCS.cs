using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeCS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeCS(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeCS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.CSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeCS(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeCS(AttributeCS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeCS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeCS(this);
        }

    }
}
