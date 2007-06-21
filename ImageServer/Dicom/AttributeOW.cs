using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeOW : AttributeBinary
    {
        public AttributeOW(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeOW(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OWvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeOW(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
            OFCondition status;
            IntPtr pUnmanagedPixelData = IntPtr.Zero;

            DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);
            uint length = 0;

            bool tagExists;
            status = item.Item.findAndGetUint16Array(offisTag, ref pUnmanagedPixelData, ref length);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            // length is based on 16-bit words
            length = length * 2;

            base._values = new byte[length];
            Marshal.Copy(pUnmanagedPixelData, base._values, 0, (int)length);

            // We don't need the unmanaged pixel data anymore since we've already
            // made a copy so just get rid of it to free up some memory
            status = item.Item.findAndDeleteElement(offisTag);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            base.Length = (long)length;
            base.Count = 1;
            base.Dirty = true;
        }

        internal AttributeOW(AttributeOW attrib)
            : base(attrib)
        {
            byte[] values = (byte[])attrib.Values;

            base._values = new byte[values.Length];

            values.CopyTo(base._values, 0);
        }

        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag + " of length " + base.Length;
        }

        public override int GetHashCode()
        {
            return 0;
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
            return new AttributeOW(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOW(this);
        }

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
                DcmTag offisTag = new DcmTag(base.Tag.Group, base.Tag.Element);

                ushort[] shortVals = new ushort[_values.Length / 2];
                for (int i = 0; i < shortVals.Length; i++)
                {
                    shortVals[i] = _values[i*2+1];
                    shortVals[i] |= (ushort)(((ushort)_values[i * 2]) << 8);
                }

                item.Item.putAndInsertUint16Array(offisTag, shortVals, (uint)shortVals.Length, true);

                base.Dirty = false;
            }
        }
        #endregion


    }
}
