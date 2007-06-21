using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeAE : AttributeMultiValueText
    {
        #region Constructors

        public AttributeAE(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeAE(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.AEvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeAE(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeAE(AttributeAE attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override AbstractAttribute Copy()
        {
            return new AttributeAE(this);
        }
        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeAE(this);
        }

        #endregion

    }
}
