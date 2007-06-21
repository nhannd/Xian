using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeUI : AttributeMultiValueText
    {
        #region Constructors

        public AttributeUI(uint tag)
            : base(tag)
        {

        }

        public AttributeUI(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UIvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeUI(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
        }

        internal AttributeUI(AttributeUI attrib)
            : base(attrib)
        {
        }

        #endregion

        public override AbstractAttribute Copy()
        {
            return new AttributeUI(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUI(this);
        }

    }
}
