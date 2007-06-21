using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeOF : AbstractAttribute
    {
        float[] _values = null;

        public AttributeOF(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeOF(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OFvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
   
        }

        internal AttributeOF(DicomTag tag, OffisDcmItem item)
            : base(tag)
        {
            IntPtr array = IntPtr.Zero;
            bool tagExists;

            DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);
            uint length = 0;

            OFCondition status = item.Item.findAndGetFloat32Array(offisTag, ref array, ref length);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            _values = new float[length];

            Marshal.Copy(array, _values, 0, (int)length);

            Count = (long)length;
            Length = Count * 4;
        }

        internal AttributeOF(AttributeOF attrib)
            : base(attrib)
        {
            float[] values = (float[])attrib.Values;

            _values = new float[values.Length];

            values.CopyTo(_values, 0);
        }


        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag; // TODO
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AbstractAttribute a = (AbstractAttribute)obj;
            float[] array = (float[])a.Values;

            if (Count != a.Count)
                return false;
            if (Count == 0 && a.Count == 0)
                return true;

            for (int i = 0; i < a.Count; i++)
                if (!array[i].Equals(_values[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return 0; // TODO
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
                if (value is float[])
                {
                    _values = (float[])value;
                    Count = _values.Length;
                    Length = Count * 4;
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override AbstractAttribute Copy()
        {
            return new AttributeOF(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOF(this);
        }

        public override void SetStringValue(String stringValue)
        {
            throw new DicomException(SR.InvalidType);
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
