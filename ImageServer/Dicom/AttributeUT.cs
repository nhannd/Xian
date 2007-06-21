using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeUT : AttributeSingleValueText
    {
        #region Constructors

        public AttributeUT(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeUT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UTvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
            
        }

        internal AttributeUT(AttributeUT attrib)
            : base(attrib)
        {

        }

        #endregion


        internal AttributeUT(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        public override AbstractAttribute Copy()
        {
            return new AttributeUT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUT(this);
        }

    }
}
