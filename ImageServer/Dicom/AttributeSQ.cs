using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.Offis;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    public class AttributeSQ : AbstractAttribute
    {
        SequenceItem[] _values = null;

        #region Constructors

        public AttributeSQ(uint tag)
            : base(tag)
        {

        }

        public AttributeSQ(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SQvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeSQ(DicomTag tag, OffisDcmItem wrapperItem)
            : base(tag)
        {
            List<SequenceItem> list = new List<SequenceItem>();

            DcmTagKey offisTag = new DcmTagKey(tag.Group, tag.Element);

            for (int num = 0; ; num++)
            {
                DcmItem offisItem = OffisDcm.findAndGetSequenceItemFromItem(wrapperItem.Item, offisTag, num);
                if (offisItem == null)
                    break;
                SequenceItem item = new SequenceItem(offisItem, wrapperItem.FileFormat);
                list.Add(item);
            }
         
            _values = list.ToArray();

            base.Count = _values.Length;
        }

        internal AttributeSQ(AttributeSQ attrib, bool copyBinary)
            : base(attrib)
        {
            SequenceItem[] items = (SequenceItem[])attrib.Values;

            _values = new SequenceItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                _values[i] = (SequenceItem)items[i].Copy(copyBinary);
            }
        }

        #endregion

        #region Public Methods

        public void AddSequenceItem(SequenceItem item)
        {
            if (_values == null)
            {
                _values = new SequenceItem[1];
                _values[0] = item;
                return;
            }

            SequenceItem[] oldValues = _values;

            _values = new SequenceItem[oldValues.Length + 1];
            for (int i = 0; i < oldValues.Length; i++)
            {
                _values[i] = oldValues[i];
            }
            _values[oldValues.Length] = item;

            base.Count = _values.Length;
            base.Length = base.Count;

            base.Dirty = true;
        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AttributeSQ a = (AttributeSQ)obj;
            SequenceItem[] array = (SequenceItem[])a.Values;

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
                if (value is SequenceItem[])
                {
                    _values = (SequenceItem[])value;
                    base.Count = _values.Length;
                    base.Length = base.Count;
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override AbstractAttribute Copy()
        {
            return new AttributeSQ(this, true);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeSQ(this, copyBinary);
        }

        public override void SetStringValue(String stringValue)
        {
            throw new DicomException("Function all incompativle with SQ VR type");
        }

        internal override void FlushAttribute(OffisDcmItem item)
        {
            if (base.Dirty)
            {
		    //TODO
            }
        }
        #endregion
    }
}
