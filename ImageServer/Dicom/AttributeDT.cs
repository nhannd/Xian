using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeDT : AttributeMultiValueText
    {
        #region Constructors

        public AttributeDT(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeDT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DTvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeDT(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeDT(AttributeDT attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeDT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeDT(this);
        }

    }
}
