using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.IO;
//using ClearCanvas.Dicom.Exceptions;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Abstract class representing a DICOM attribute within an attribute collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The DicomAttribute class is a base class that represents a DICOM attribute.  A number of abstract methods are 
    /// defined.  Derived classes exist for each of the VR types.  In addition, the <see cref="DicomAttributeBinary"/>,
    /// <see cref="AttributeMultiValueText"/>, and <see cref="AttributeSingelValueText"/> classes contain generic
    /// implementations for binary VRs, text values that contain multiple values, and text VRs that contain a single
    /// value respectively.
    /// </para>
    /// </remarks>
    public abstract class DicomAttribute
    {
        #region Private Members
        private DicomTag _tag;
        private long _valueCount = 0;
        private uint _length = 0;
        #endregion

        #region Abstract and Virtual Methods
        /// <summary>
        /// Method to return a string representation of the attribute.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        public abstract bool IsNull { get; }
        public abstract bool IsEmpty { get; }
        public abstract Object Values { get; set; }
        public abstract DicomAttribute Copy();
        public abstract void SetStringValue(String stringValue);
        public abstract Type GetValueType();

        internal abstract ByteBuffer GetByteBuffer(TransferSyntax syntax);
        internal abstract DicomAttribute Copy(bool copyBinary);

        internal virtual uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 4; // element tag
            if (syntax.ExplicitVr)
            {
                length += 2; // vr
                if (Tag.VR.Is16BitLengthField)
                    length += 2;
                else
                    length += 6;
            }
            else
            {
                length += 4; // length tag				
            }
            length += (uint)StreamLength;
            if ((length & 0x00000001) != 0)
                length++;

            return length;
        }

        public virtual bool TryGetUInt16(int i, out ushort value)
        {
            value = 0;
            return false;
        }
        public virtual ushort GetUInt16(int i, ushort defaultVal)
        {
            ushort value;
            bool ok = TryGetUInt16(i, out value);
            if (!ok)
                return defaultVal;
            return value;
        }
        public virtual bool TryGetInt16(int i, out short value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetUInt32(int i, out uint value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetInt32(int i, out int value)
        {
            value = 0;
            return false;
        }
        public virtual bool TryGetFloat32(int i, out float value)
        {
            value = 0.0f;
            return false;
        }
        public virtual bool TryGetFloat64(int i, out double value)
        {
            value = 0.0f;
            return false;
        }
        public virtual bool TryGetString(int i, out String value)
        {
            value = "";
            return false;
        }
        public virtual String GetString(int i, String defaultVal)
        {
            String value;
            bool ok = TryGetString(i, out value);
            if (!ok)
                return defaultVal;
            return value;
        }

        public virtual bool TryGetUid(int i, out DicomUid value)
        {
            value = null;
            return false;
        }
        public virtual bool TryGetDateTime(int i, out DateTime value)
        {
            value = new DateTime();
            return false;
        }

        public virtual void AddSequenceItem(DicomSequenceItem item)
        {
            throw new DicomException(SR.InvalidType);
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Internal constructor when a <see cref="DicomTag"/> is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal DicomAttribute(DicomTag tag)
        {
            _tag = tag;
        }

        /// <summary>
        /// Internal constructor when a uint representation of the tag is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal DicomAttribute(uint tag)
        {
            _tag = DicomTagDictionary.Instance[tag];
        }

        /// <summary>
        /// Internal constructor used when copying an attribute from a pre-existing attribute instance.
        /// </summary>
        /// <param name="attrib">The attribute that is being copied.</param>
        internal DicomAttribute(DicomAttribute attrib)
        {
            _tag = attrib.Tag;
            _valueCount = attrib.Count;
            _length = attrib.StreamLength;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieve <see cref="Tag"/> instance for the attribute.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        public uint StreamLength
        {
            get
            {
                if ((_length & 0x00000001) == 1)
                    return _length + 1;

                return _length;
            }
            protected set { _length = value; }
        }

        public long Count
        {
            get { return _valueCount; }
            protected set { _valueCount = value; }
        }

        #endregion

        #region Internal Static Methods

        internal static DicomAttribute NewAttribute(uint tag)
        {
            DicomTag dictionTag = DicomTagDictionary.Instance[tag];
            return NewAttribute(dictionTag);
        }

        internal static DicomAttribute NewAttribute(DicomTag tag)
        {
            if (tag == null)
                return null;


            switch (tag.VR.Name)
            {
                case "AE": return new DicomAttributeAE(tag);
                case "AS": return new DicomAttributeAS(tag);
                case "AT": return new DicomAttributeAT(tag);
                case "CS": return new DicomAttributeCS(tag);
                case "DA": return new DicomAttributeDA(tag);
                case "DS": return new DicomAttributeDS(tag);
                case "DT": return new DicomAttributeDT(tag);
                case "FL": return new DicomAttributeFL(tag);
                case "FD": return new DicomAttributeFD(tag);
                case "IS": return new DicomAttributeIS(tag);
                case "LO": return new DicomAttributeLO(tag);
                case "LT": return new DicomAttributeLT(tag);
                case "OB": return new DicomAttributeOB(tag);
                case "OF": return new DicomAttributeOF(tag);
                case "OW": return new DicomAttributeOW(tag);
                case "PN": return new DicomAttributePN(tag);
                case "SH": return new DicomAttributeSH(tag);
                case "SL": return new DicomAttributeSL(tag);
                case "SQ": return new DicomAttributeSQ(tag);
                case "SS": return new DicomAttributeSS(tag);
                case "ST": return new DicomAttributeST(tag);
                case "TM": return new DicomAttributeTM(tag);
                case "UI": return new DicomAttributeUI(tag);
                case "UL": return new DicomAttributeUL(tag);
                case "UN": return new DicomAttributeUN(tag);
                case "US": return new DicomAttributeUS(tag);
                case "UT": return new DicomAttributeUT(tag);
            }

            return null;

        }

        internal static DicomAttribute NewAttribute(DicomTag tag, ByteBuffer bb)
        {
            if (tag == null)
                return null;

            switch (tag.VR.Name)
            {
                case "AE": return new DicomAttributeAE(tag, bb);
                case "AS": return new DicomAttributeAS(tag, bb);
                case "AT": return new DicomAttributeAT(tag, bb);
                case "CS": return new DicomAttributeCS(tag, bb);
                case "DA": return new DicomAttributeDA(tag, bb);
                case "DS": return new DicomAttributeDS(tag, bb);
                case "DT": return new DicomAttributeDT(tag, bb);
                case "FL": return new DicomAttributeFL(tag, bb);
                case "FD": return new DicomAttributeFD(tag, bb);
                case "IS": return new DicomAttributeIS(tag, bb);
                case "LO": return new DicomAttributeLO(tag, bb);
                case "LT": return new DicomAttributeLT(tag, bb);
                case "OB": return new DicomAttributeOB(tag, bb);
                case "OF": return new DicomAttributeOF(tag, bb);
                case "OW": return new DicomAttributeOW(tag, bb);
                case "PN": return new DicomAttributePN(tag, bb);
                case "SH": return new DicomAttributeSH(tag, bb);
                case "SL": return new DicomAttributeSL(tag, bb);
                //case "SQ": return new AttributeSQ(tag, bb);
                case "SS": return new DicomAttributeSS(tag, bb);
                case "ST": return new DicomAttributeST(tag, bb);
                case "TM": return new DicomAttributeTM(tag, bb);
                case "UI": return new DicomAttributeUI(tag, bb);
                case "UL": return new DicomAttributeUL(tag, bb);
                case "UN": return new DicomAttributeUN(tag, bb);
                case "US": return new DicomAttributeUS(tag, bb);
                case "UT": return new DicomAttributeUT(tag, bb);
            }

            return null;

        }
        #endregion

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(DicomAttribute attr)
        {
            // Uses the actual ToString implementation of the derived class.
            return attr.ToString();
        }

        #region Dump
        public virtual void Dump(StringBuilder sb, string prefix, DicomDumpOptions options)
        {
            int ValueWidth = 40 - prefix.Length;
            int SbLength = sb.Length;

            sb.Append(prefix);
            sb.AppendFormat("({0:x4},{1:x4}) {2} ", Tag.Group, Tag.Element, Tag.VR.Name);
            if (Count == 0)
            {
                String value = "(no value available)";
                sb.Append(value.PadRight(ValueWidth, ' '));
            }
            else
            {
                if (Tag.VR.IsTextVR)
                {
                    String value = null;
                    if (Tag.VR == DicomVr.UIvr)
                    {
                        DicomAttributeUI ui = this as DicomAttributeUI;

                        DicomUid uid;
                        bool ok = ui.TryGetUid(0, out uid);

                        if (ok && uid.Type != UidType.Unknown)
                        {
                            value = "=" + uid.Description;
                            if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                            {
                                if (value.Length > ValueWidth)
                                {
                                    value = value.Substring(0, ValueWidth - 3);
                                }
                            }
                        }
                        else
                        {
                            value = "[" + this.ToString() + "]";
                            if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                            {
                                if (value.Length > ValueWidth)
                                {
                                    value = value.Substring(0, ValueWidth - 4) + "...]";
                                }
                            }
                        }
                    }
                    else
                    {
                        value = "[" + this.ToString() + "]";
                        if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                        {
                            if (value.Length > ValueWidth)
                            {
                                value = value.Substring(0, ValueWidth - 4) + "...]";
                            }
                        }
                    }
                    sb.Append(value.PadRight(ValueWidth, ' '));
                }
                else
                {
                    String value = this.ToString();
                    if (Flags.IsSet(options, DicomDumpOptions.ShortenLongValues))
                    {
                        if (value.Length > ValueWidth)
                        {
                            value = value.Substring(0, ValueWidth - 3) + "...";
                        }
                    }
                    sb.Append(value.PadRight(ValueWidth, ' '));
                }
            }
            sb.AppendFormat(" # {0,4} {2} {1}", StreamLength, Tag.VM, Tag.Name);

            if (Flags.IsSet(options, DicomDumpOptions.Restrict80CharactersPerLine))
            {
                if (sb.Length > (SbLength + 79))
                {
                    sb.Length = SbLength + 79;
                    //sb.Append(">");
                }
            }
        }
        #endregion
    }
}
