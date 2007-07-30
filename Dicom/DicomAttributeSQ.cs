using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
    public class DicomAttributeSQ : DicomAttribute
    {
        DicomSequenceItem[] _values = null;

        #region Constructors

        public DicomAttributeSQ(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSQ(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SQvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSQ(DicomAttributeSQ attrib, bool copyBinary)
            : base(attrib)
        {
            DicomSequenceItem[] items = (DicomSequenceItem[])attrib.Values;

            _values = new DicomSequenceItem[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                _values[i] = (DicomSequenceItem)items[i].Copy(copyBinary);
            }
        }

        #endregion

        #region Public Methods

        public override void AddSequenceItem(DicomSequenceItem item)
        {
            if (_values == null)
            {
                _values = new DicomSequenceItem[1];
                _values[0] = item;
                if (item.SpecificCharacterSet == null)
                    item.SpecificCharacterSet = this.ParentCollection.SpecificCharacterSet;
                return;
            }

            DicomSequenceItem[] oldValues = _values;

            _values = new DicomSequenceItem[oldValues.Length + 1];
            oldValues.CopyTo(_values, 0);
            _values[oldValues.Length] = item;

            if (item.SpecificCharacterSet == null)
                item.SpecificCharacterSet = this.ParentCollection.SpecificCharacterSet;

            base.Count = _values.Length;
            base.StreamLength = (uint)base.Count;
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

            DicomAttributeSQ a = (DicomAttributeSQ)obj;
            DicomSequenceItem[] array = (DicomSequenceItem[])a.Values;

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

        public override Type GetValueType()
        {
            return typeof(DicomSequenceItem);
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

        public override bool IsEmpty
        {
            get
            {
                if ((Count == 0) && (_values == null))
                    return true;
                return false;
            }
        }


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is DicomSequenceItem[])
                {
                    _values = (DicomSequenceItem[])value;
                    base.Count = _values.Length;
                    base.StreamLength = (uint)base.Count;
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSQ(this, true);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSQ(this, copyBinary);
        }

        public override void SetStringValue(String stringValue)
        {
            throw new DicomException("Function all incompatible with SQ VR type");
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            throw new DicomException("Unexpected call to GetByteBuffer() for a SQ attribute");
        }
        internal override uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 0;
            length += 4; // element tag
            if (syntax.ExplicitVr)
            {
                length += 2; // vr
                length += 6; // length
            }
            else
            {
                length += 4; // length
            }
            foreach (DicomSequenceItem item in _values)
            {
                length += 4 + 4; // Sequence Item Tag
                length += item.CalculateWriteLength(syntax, options & ~DicomWriteOptions.CalculateGroupLengths);
                if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequenceItem))
                    length += 4 + 4; // Sequence Item Delimitation Item
            }
            if (!Flags.IsSet(options, DicomWriteOptions.ExplicitLengthSequence))
                length += 4 + 4; // Sequence Delimitation Item
            return length;
        }
        #endregion

        #region Dump
        public override void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            sb.Append(prefix);
            sb.AppendFormat("({0:x4},{1:x4}) {2} ", Tag.Group, Tag.Element, Tag.VR.Name);
            foreach (DicomSequenceItem item in _values)
            {
                sb.AppendLine().Append(prefix).Append(" Item:").AppendLine();
                item.Dump(sb, prefix + "  > ", options);
                sb.Length = sb.Length - 1;
            }
        }
        #endregion
    }
}
