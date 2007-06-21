using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeUN : AttributeBinary
    {
        #region Constructors

        public AttributeUN(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeUN(DicomTag tag)
            : base(tag)
        {
        }

        internal AttributeUN(DicomTag tag, OffisDcmItem item)
            : base(tag, item)
        {
            //TODO, this fails right now, figure out how to do.
            return;

            OFCondition status;
            IntPtr pUnmanagedPixelData = IntPtr.Zero;

            DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);
            uint length = 0;

            bool tagExists;
            status = item.Item.findAndGetUint8Array(offisTag, ref pUnmanagedPixelData, ref length);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            base._values = new byte[length];
            Marshal.Copy(pUnmanagedPixelData, base._values, 0, (int)length);

            base.Length = (long)length;
            base.Count = 1;
        }

        internal AttributeUN(AttributeUN attrib)
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
            return base.Tag.ToString(); // TODO
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            return false;  //TODO
        }

        public override int GetHashCode()
        {
            if (_values == null)
                return 0; // TODO
            else
                return _values.GetHashCode(); // TODO
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

        public override object Values
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
            return new AttributeUN(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUN(this);
        }

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {

            }
        }
        #endregion
    }
}
