using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeLT : AttributeSingleValueText
    {
        #region Constructors

        public AttributeLT(uint tag)
            : base(tag)
        {

        }

        public AttributeLT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LTvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }
        internal AttributeLT(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeLT(AttributeLT attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeLT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeLT(this);
        }

    }
}
