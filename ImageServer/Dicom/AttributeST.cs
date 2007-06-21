using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeST : AttributeSingleValueText
    {
        #region Constructors

        public AttributeST(uint tag)
            : base(tag)
        {

        }

        public AttributeST(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.STvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeST(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeST(AttributeST attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeST(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeST(this);
        }

    }
}
