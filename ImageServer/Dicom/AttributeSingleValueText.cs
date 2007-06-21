using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public abstract class AttributeSingleValueText : AbstractAttribute
    {
        private String _value = null;

        #region Constructors

        internal AttributeSingleValueText(uint tag) 
            : base(tag)
        {
            
        }

        internal AttributeSingleValueText(DicomTag tag)
            : base(tag)
        {
            
        }

        internal AttributeSingleValueText(DicomTag tag, OffisDcmItem item)
            : base(tag)
        {
            DcmTagKey offisTag = new DcmTagKey(tag.Group,tag.Element);
            bool tagExists;

            StringBuilder buffer = new StringBuilder((int)base.Tag.VR.MaximumLength);
			OFCondition status = item.Item.findAndGetOFString(offisTag, buffer);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);
			_value = buffer.ToString();

            Count = 1;
            Length = _value.Length;
        }

        internal AttributeSingleValueText(AttributeSingleValueText attrib)
            : base(attrib)
        {
            String value = attrib;

            _value = String.Copy(value);

        }

        #endregion


        #region Abstract Method Implementation

        public override string ToString()
        {
            if (_value == null)
                return "";

            return _value;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AbstractAttribute a = (AbstractAttribute)obj;
            string str = (string)a.Values;

            return _value.Equals(str);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool IsNull
        {
            get
            {
                if ((_value != null) && (_value.Length == 0))
                    return true;
                return false;
            }
        }

        public override Object Values
        {
            get { return _value; }
            set
            {
                if (value is String)
                {
                    SetStringValue((string)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override void SetStringValue(String stringValue)
        {
            Dirty = true;

            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
                Length = 0;
                _value = "";
                return;
            }

            _value = stringValue;

            Count = 1;
            Length = _value.Length;
        }

        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
                 DcmTag offisTag = new DcmTag(base.Tag.Group,base.Tag.Element);

                 // Remove the old value, to make sure its cleared out
                 bool tagExists;
                 OFCondition status = item.Item.findAndDeleteElement(offisTag);
                 OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

                 item.Item.putAndInsertString(offisTag, _value);
            }
        }

        #endregion
    }
}
