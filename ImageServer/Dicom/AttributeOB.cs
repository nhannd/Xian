using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeOB : AttributeBinary
    {
        #region Constructors

        public AttributeOB(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeOB(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OBvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeOB(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
            OFCondition status;
            IntPtr pUnmanagedPixelData = IntPtr.Zero;

            DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);
            uint length = 0;

            bool tagExists;
            status = item.Item.findAndGetUint8Array(offisTag, ref pUnmanagedPixelData,ref length);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            base._values = new byte[length];
            Marshal.Copy(pUnmanagedPixelData,base._values, 0, (int)length);

            // We don't need the unmanaged pixel data anymore since we've already
            // made a copy so just get rid of it to free up some memory
            status = item.Item.findAndDeleteElement(offisTag);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            base.Length = (long)length;
            base.Count = 1;
            base.Dirty = true;

        }

        internal AttributeOB(AttributeOB attrib)
            : base(attrib)
        {
            byte[] values = (byte[])attrib.Values;

            base._values = new byte[values.Length];

            values.CopyTo(base._values, 0);

        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag + " of length " + base.Length; // TODO
        }

        public override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values != null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is byte[])
                {
                    _values = (byte[])value;
                    Count = 1;
                    Length = _values.Length;
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override AbstractAttribute Copy()
        {
            return new AttributeOB(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOB(this);
        }

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
                DcmTag offisTag = new DcmTag(base.Tag.Group, base.Tag.Element);

                item.Item.putAndInsertUint8Array(offisTag, _values, (uint)_values.Length, true);

                base.Dirty = false;
            }
        }
        #endregion
    }
}
