using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeLO : AttributeMultiValueText
    {
        #region Constructors

        public AttributeLO(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeLO(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LOvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeLO(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeLO(AttributeLO attrib)
            : base(attrib)
        {
        }

        #endregion 

        public override AbstractAttribute Copy()
        {
            return new AttributeLO(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeLO(this);
        }

    }
}
