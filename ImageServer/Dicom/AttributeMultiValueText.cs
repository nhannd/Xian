using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageServer.Dicom
{
    public abstract class AttributeMultiValueText : AbstractAttribute
    {
        #region Private Members

        private String[] _values = null;

        #endregion

        #region Constructors

        internal AttributeMultiValueText(uint tag) 
            : base(tag)
        {
            
        }

        internal AttributeMultiValueText(DicomTag tag)
            : base(tag)
        {
            
        }

        internal AttributeMultiValueText(DicomTag tag, OffisDcmItem item)
            : base(tag)
        {
            DcmTagKey offisTag = new DcmTagKey(tag.Group,tag.Element);
            bool tagExists;

            // Get the tag length
            DcmElement element = OffisDcm.findAndGetElementFromItem(item.Item, offisTag);

            uint length = element.getLength();
            StringBuilder buffer = new StringBuilder((int)length + 10); // +`10 to be safe

            OFCondition status = item.Item.findAndGetOFStringArray(offisTag, buffer);
            OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

            String valueArray = buffer.ToString();

            _values = valueArray.Split(new char[] { '\\' });

            Count = (long)_values.Length;

            Length = valueArray.Length;
        }

        internal AttributeMultiValueText(AttributeMultiValueText attrib)
            : base(attrib)
        {
            String[] values = (String[])attrib.Values;

            _values = new String[values.Length];

            for (int i = 0; i < values.Length; i++)
                _values[i] = String.Copy(values[i]);
        }

        #endregion

        #region Operators

        public String this[int val]
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

            String value = null;

            foreach (String val in _values)
            {
                if (value == null)
                    value = val;
                else 
                    value += "\\" + val;
            }

            if (value == null) return "";
             
            return value;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AttributeMultiValueText a = (AttributeMultiValueText)obj;

            // check if both values are null
            if (this.Count == 1 && this.Length == 0 && a.Count == 1 && a.Length == 0)
                return true;

            return ToString().Equals(a.ToString());
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values!=null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is string[])
                {
                    _values = (string[])value;
                    Count = _values.Length;
                    Length = ToString().Length;
                }
                else if (value is string)
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
                _values = new String[0];
                return;
            }

            _values = stringValue.Split(new char[] { '\\' });

            Count = (long)_values.Length;

            Length = stringValue.Length;
        }

        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
                DcmTag offisTag = new DcmTag(base.Tag.Group, base.Tag.Element);

                // Remove the old value, to make sure its cleared out
                bool tagExists;
                OFCondition status = item.Item.findAndDeleteElement(offisTag);
                OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

                status = item.Item.putAndInsertString(offisTag, ToString());
                OffisHelper.CheckReturnValue(status, offisTag, out tagExists);

                base.Dirty = false;
            }
        }

        #endregion

    }
}
