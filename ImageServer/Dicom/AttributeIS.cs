using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeIS : AttributeMultiValueText
    {
        #region Constructors

        public AttributeIS(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeIS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ISvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
  
        }

        internal AttributeIS(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeIS(AttributeIS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeIS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeIS(this);
        }

    }
}
