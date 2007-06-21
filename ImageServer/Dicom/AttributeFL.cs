using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeFL : AbstractAttribute
    {
        public float[] _values = null;

        #region Constructors

        public AttributeFL(uint tag) 
            : base(tag)
        {
            
        }

        public AttributeFL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeFL(DicomTag tag, OffisDcmItem item)
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

        internal AttributeFL(AttributeFL attrib)
            : base(attrib)
        {
            float[] values = (float[])attrib.Values;

            _values = new float[values.Length];

            values.CopyTo(_values, 0);
        }

        #endregion

        #region Operators

        public float this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            if (_values == null)
                return "";

            String val = null;
            foreach (float i in _values)
            {
                if (val == null)
                    val = i.ToString();
                else
                    val += "\\" + i.ToString();
            }

            if (val == null)
                val = "";

            return val;
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
            if (_values == null)
                return 0; // TODO
            else
                return _values.GetHashCode();
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
                else if (value is String)
                {
                    SetStringValue((String)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override AbstractAttribute Copy()
        {
            return new AttributeFL(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeFL(this);
        }

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
                DcmTag offisTag = new DcmTag(base.Tag.Group, base.Tag.Element);

                // Remove the old value, to make sure its cleared out
                bool tagExists;
                OFCondition status = item.Item.findAndDeleteElement(offisTag);
                OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

                for (int i = 0; i < _values.Length; i++)
                    item.Item.putAndInsertFloat32(offisTag, _values[i], (uint)i);

                base.Dirty = false;
            }
        }

        public override void SetStringValue(String stringValue)
        {
            Dirty = true;

            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
                Length = 0;
                _values = new float[0];
                return;
            }

            String[] stringValues = stringValue.Split(new char[] { '\\' });

            Count = (long)stringValues.Length;

            _values = new float[Count];

            for (int i = 0; i < stringValues.Length; i++)
            {
                _values[i] = float.Parse(stringValues[i]);
            }

            Length = Count * 4;
        }

        #endregion
    }
}
