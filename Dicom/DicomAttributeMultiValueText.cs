using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;

using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
    #region DicomAttributeMultiValueText
    /// <summary>
    /// <see cref="DicomAttribute"/> derived class for storing character based DICOM value representation attributes with multiple values.
    /// </summary>
    public abstract class DicomAttributeMultiValueText : DicomAttribute
    {
        #region Private Members

        protected String[] _values = null;

        #endregion

        #region Constructors

        internal DicomAttributeMultiValueText(uint tag)
            : base(tag)
        {

        }

        internal DicomAttributeMultiValueText(DicomTag tag)
            : base(tag)
        {

        }

        internal DicomAttributeMultiValueText(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            String valueArray;

            valueArray = item.GetString();

            // store the length before removing pad chars
            StreamLength = (uint) valueArray.Length;

            // Saw some Osirix images that had padding on SH attributes with a null character, just
            // pull them out here.
            valueArray = valueArray.Trim(new char[] {tag.VR.PadChar, '\0'});

            if (valueArray.Length == 0)
            {
                _values = new string[0];
                Count = 1;
                StreamLength = 0;
            }
            else
            {
                _values = valueArray.Split(new char[] {'\\'});

                Count = (long) _values.Length;

                StreamLength = (uint) valueArray.Length;
            }
        }

        internal DicomAttributeMultiValueText(DicomAttributeMultiValueText attrib)
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

        public override void SetNullValue()
        {
            _values = new string[0];
            base.StreamLength = 0;
            base.Count = 1;
        }

        public override uint StreamLength
        {
            get
            {
                if (ParentCollection.SpecificCharacterSet != null)
                {
                    return (uint)GetByteBuffer(TransferSyntax.ExplicitVrBigEndian, ParentCollection.SpecificCharacterSet).Length;
                }
                return base.StreamLength;
            }
        }

        public override string ToString()
        {
            if (_values == null)
                return "";

            // could use: return string.Join("\\", _values);
            StringBuilder value = null;

            foreach (String val in _values)
            {
                if (value == null)
                    value = new StringBuilder(val);
                else
                    value.AppendFormat("\\{0}", val);
            }

            if (value == null) return "";

            return value.ToString();
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeMultiValueText a = (DicomAttributeMultiValueText)obj;

            // check if both values are null
            if (this.Count == 1 && this.StreamLength == 0 && a.Count == 1 && a.StreamLength == 0)
                return true;

            return ToString().Equals(a.ToString());
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public override Type GetValueType()
        {
            return typeof(String);
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
                if (value is string[])
                {
                    _values = (string[])value;
                    Count = _values.Length;
                    StreamLength = (uint)ToString().Length;
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

        public override bool TryGetString(int i, out String value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = "";
                return false;
            }

            value = _values[i];
            return true;
        }

        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
                StreamLength = 0;
                _values = new String[0];
                return;
            }

            _values = stringValue.Split(new char[] { '\\' });

            Count = (long)_values.Length;

            StreamLength = (uint)stringValue.Length;
        }

        public override void AppendString(string stringValue)
        {
            int newArrayLength = 1;
            int oldArrayLength = 0;

            if (_values != null)
            {
                newArrayLength = _values.Length + 1;
                oldArrayLength = _values.Length;
            }

            string[] newArray = new string[newArrayLength];
            if (oldArrayLength > 0)
                _values.CopyTo(newArray, 0);
            newArray[newArrayLength - 1] = (string)stringValue;
            _values = newArray;

            StreamLength = (uint)this.ToString().Length;
        }

        public abstract override DicomAttribute Copy();
        internal abstract override DicomAttribute Copy(bool copyBinary);

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            if (Tag.VR.SpecificCharacterSet)
                bb.SpecificCharacterSet = specificCharacterSet;

            bb.SetString(ToString(), (byte)' ');

            return bb;
        }

        #endregion

    }
    #endregion

    #region DicomAttributeAE
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing AE value representation attributes.
    /// </summary>
    public class DicomAttributeAE : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeAE(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeAE(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.AEvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAE(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeAE(DicomAttributeAE attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAE(this);
        }
        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAE(this);
        }

        #endregion

    }
    #endregion

    #region DicomAttributeAS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing AS value representation attributes.
    /// </summary>
    public class DicomAttributeAS : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeAS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeAS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ASvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeAS(DicomAttributeAS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAS(this);
        }

    }
    #endregion

    #region DicomAttributeCS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing CS value representation attributes.
    /// </summary>
    public class DicomAttributeCS : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeCS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeCS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.CSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeCS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeCS(DicomAttributeCS attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeCS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeCS(this);
        }

    }
    #endregion

    #region DicomAttributeDA
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DA value representation attributes.
    /// </summary>
    public class DicomAttributeDA : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeDA(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeDA(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DAvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeDA(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDA(DicomAttributeDA attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDA(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDA(this);
        }
        public override bool TryGetDateTime(int i, out DateTime value)
        {
            // Dicom recommends we still support the old date format (#2) although it is deprecated.
            // See PS 3.5, table 6.2-1 - 'Dicom Value Representations' under VR DA.
            if (_values == null || _values.Length <= i)
            {
                value = new DateTime();
                return false;
            }

            return DateTime.TryParseExact(_values[i], new string[] { "yyyyMMdd", "yyyy.MM.dd" }, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out value);
        }
    }
    #endregion

    #region DicomAttributeDS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DS value representation attributes.
    /// </summary>
    public class DicomAttributeDS : DicomAttributeMultiValueText
    {
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors

        public DicomAttributeDS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeDS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DSvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeDS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDS(DicomAttributeDS attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Properties
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }
        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDS(this);
        }

        public override bool TryGetFloat32(int i, out float value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            return float.TryParse(_values[i], out value);
        }

        public override bool TryGetFloat64(int i, out double value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0;
                return false;
            }

            return double.TryParse(_values[i], out value);
        }

    }
    #endregion

    #region DicomAttributeDT
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing DT value representation attributes.
    /// </summary>
    public class DicomAttributeDT : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeDT(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeDT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.DTvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeDT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeDT(DicomAttributeDT attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeDT(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeDT(this);
        }

        public override bool TryGetDateTime(int i, out DateTime value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = new DateTime();
                return false;
            }

            return DateTime.TryParseExact(_values[i], "yyyyMMddHHmmss.FFFFFF&ZZZZ", CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out value);
        }
    }
    #endregion

    #region DicomAttributeIS
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing IS value representation attributes.
    /// </summary>
    public class DicomAttributeIS : DicomAttributeMultiValueText
    {
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors

        public DicomAttributeIS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeIS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ISvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeIS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeIS(DicomAttributeIS attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Properties
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }
        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeIS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeIS(this);
        }

        public override void AppendInt32(int intValue)
        {
            int newArrayLength = 1;
            int oldArrayLength = 0;

            if (_values != null)
            {
                newArrayLength = _values.Length + 1;
                oldArrayLength = _values.Length;
            }

            string[] newArray = new string[newArrayLength];
            if (oldArrayLength > 0)
                _values.CopyTo(newArray, 0);
            newArray[newArrayLength - 1] = intValue.ToString();
            _values = newArray;

            Count = _values.Length;
            StreamLength = (uint)ToString().Length;            
        }

        public override bool TryGetFloat32(int i, out float value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            return float.TryParse(_values[i], out value);
        }

        public override bool TryGetFloat64(int i, out double value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            return double.TryParse(_values[i], out value);
        }

        public override bool TryGetUInt16(int i, out ushort value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            return ushort.TryParse(_values[i], NumberStyle, CultureInfo.CurrentCulture, out value);
        }
        public override bool TryGetInt16(int i, out short value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            return short.TryParse(_values[i], NumberStyle, CultureInfo.CurrentCulture, out value);
        }
        public override bool TryGetUInt32(int i, out uint value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            return uint.TryParse(_values[i], NumberStyle, CultureInfo.CurrentCulture, out value);
        }
        public override bool TryGetInt32(int i, out int value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            return int.TryParse(_values[i], NumberStyle, CultureInfo.CurrentCulture, out value);
        }
    }
    #endregion

    #region DicomAttributeLO
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing LO value representation attributes.
    /// </summary>
    public class DicomAttributeLO : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeLO(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeLO(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.LOvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeLO(DicomAttributeLO attrib)
            : base(attrib)
        {
        }

        internal DicomAttributeLO(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeLO(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeLO(this);
        }

    }
    #endregion

    #region DicomAttributePN
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing PN value representation attributes.
    /// </summary>
    public class DicomAttributePN : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributePN(uint tag)
            : base(tag)
        {

        }

        public DicomAttributePN(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.PNvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributePN(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributePN(DicomAttributePN attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributePN(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributePN(this);
        }

    }
    #endregion

    #region DicomAttributeSH
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing SH value representation attributes.
    /// </summary>
    public class DicomAttributeSH : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeSH(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSH(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SHvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSH(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeSH(DicomAttributeSH attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSH(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSH(this);
        }

    }
    #endregion

    #region DicomAttributeTM
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing TM value representation attributes.
    /// </summary>
    public class DicomAttributeTM : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeTM(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeTM(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.TMvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeTM(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeTM(DicomAttributeTM attrib)
            : base(attrib)
        {
        }

        #endregion

        public override DicomAttribute Copy()
        {
            return new DicomAttributeTM(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeTM(this);
        }

        public override bool TryGetDateTime(int i, out DateTime value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = new DateTime();
                return false;
            }

            return DateTime.TryParseExact(_values[i], "HHmmSS.FFFFFF", CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out value);

        }

    }
    #endregion

    #region DicomAttributeUI
    /// <summary>
    /// <see cref="DicomAttributeMultiValueText"/> derived class for storing UI value representation attributes.
    /// </summary>
    public class DicomAttributeUI : DicomAttributeMultiValueText
    {
        #region Constructors

        public DicomAttributeUI(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUI(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.UIvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUI(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUI(DicomAttributeUI attrib)
            : base(attrib)
        {
        }

        #endregion

        public override bool TryGetUid(int i, out DicomUid value)
        {
            SopClass sop = SopClass.GetSopClass(base._values[i]);
            if (sop != null)
            {
                value = sop.DicomUid;
                return true;
            }

            TransferSyntax ts = TransferSyntax.GetTransferSyntax(base._values[i]);
            if (ts != null)
            {
                value = ts.DicomUid;
                return true;
            }

            value = new DicomUid(base._values[i], base._values[i], UidType.Unknown);
            return true;
        }


        public override DicomAttribute Copy()
        {
            return new DicomAttributeUI(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUI(this);
        }


        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.SetString(ToString(), (byte)0x00);

            return bb;
        }

    }
    #endregion
}
