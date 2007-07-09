using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Dicom.IO;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// Abstract class representing a DICOM attribute within an attribute collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AbstractAttribute class is a base class that represents a DICOM attribute.  A number of abstract methods are 
    /// defined.  Derived classes exist for each of the VR types.  In addition, the <see cref="AttributeBinary"/>,
    /// <see cref="AttributeMultiValueText"/>, and <see cref="AttributeSingelValueText"/> classes contain generic
    /// implementations for binary VRs, text values that contain multiple values, and text VRs that contain a single
    /// value respectively.
    /// </para>
    /// </remarks>
    public abstract class AbstractAttribute
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
        public abstract Object Values { get; set; }
        public abstract AbstractAttribute Copy();
        public abstract void SetStringValue(String stringValue);
        public abstract Type GetValueType(); 
        
        internal abstract ByteBuffer GetByteBuffer(TransferSyntax syntax);
        internal abstract AbstractAttribute Copy(bool copyBinary);
        
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

        public virtual ushort GetUInt16(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual ushort GetUInt16(int i, ushort defaultVal)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual short GetInt16(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual uint GetUInt32(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual int GetInt32(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual float GetFloat32(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual double GetFloat64(int i)
        {
            throw new DicomException(SR.InvalidType);
        }
        public virtual DicomUid GetUid(int i)
        {
            throw new DicomException(SR.InvalidType);
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
        internal AbstractAttribute(DicomTag tag)
        {
            _tag = tag;
        }

        /// <summary>
        /// Internal constructor when a uint representation of the tag is used to identify the tag being added.
        /// </summary>
        /// <param name="tag">The DICOM tag associated with the attribute being created.</param>
        internal AbstractAttribute(uint tag)
        {
            _tag = DicomTagDictionary.Instance[tag];
        }

        /// <summary>
        /// Internal constructor used when copying an attribute from a pre-existing attribute instance.
        /// </summary>
        /// <param name="attrib">The attribute that is being copied.</param>
        internal AbstractAttribute(AbstractAttribute attrib)
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

        internal static AbstractAttribute NewAttribute(uint tag)
        {
            DicomTag dictionTag = DicomTagDictionary.Instance[tag];
            return NewAttribute(dictionTag);
        }

        internal static AbstractAttribute NewAttribute(DicomTag tag)
        {
            if (tag == null)
                return null;


            switch (tag.VR.Name)
            {
                case "AE": return new AttributeAE(tag);
                case "AS": return new AttributeAS(tag);
                case "AT": return new AttributeAT(tag);
                case "CS": return new AttributeCS(tag);
                case "DA": return new AttributeDA(tag);
                case "DS": return new AttributeDS(tag);
                case "DT": return new AttributeDT(tag);
                case "FL": return new AttributeFL(tag);
                case "FD": return new AttributeFD(tag);
                case "IS": return new AttributeIS(tag);
                case "LO": return new AttributeLO(tag);
                case "LT": return new AttributeLT(tag);
                case "OB": return new AttributeOB(tag);
                case "OF": return new AttributeOF(tag);
                case "OW": return new AttributeOW(tag);
                case "PN": return new AttributePN(tag);
                case "SH": return new AttributeSH(tag);
                case "SL": return new AttributeSL(tag);
                case "SQ": return new AttributeSQ(tag);
                case "SS": return new AttributeSS(tag);
                case "ST": return new AttributeST(tag);
                case "TM": return new AttributeTM(tag);
                case "UI": return new AttributeUI(tag);
                case "UL": return new AttributeUL(tag);
                case "UN": return new AttributeUN(tag);
                case "US": return new AttributeUS(tag);
                case "UT": return new AttributeUT(tag);
            }
            
            return null;
                    
        }

        internal static AbstractAttribute NewAttribute(DicomTag tag, ByteBuffer bb)
        {
            if (tag == null)
                return null;

            switch (tag.VR.Name)
            {
                case "AE": return new AttributeAE(tag, bb);
                case "AS": return new AttributeAS(tag, bb);
                case "AT": return new AttributeAT(tag, bb);
                case "CS": return new AttributeCS(tag, bb);
                case "DA": return new AttributeDA(tag, bb);
                case "DS": return new AttributeDS(tag, bb);
                case "DT": return new AttributeDT(tag, bb);
                case "FL": return new AttributeFL(tag, bb);
                case "FD": return new AttributeFD(tag, bb);
                case "IS": return new AttributeIS(tag, bb);
                case "LO": return new AttributeLO(tag, bb);
                case "LT": return new AttributeLT(tag, bb);
                case "OB": return new AttributeOB(tag, bb);
                case "OF": return new AttributeOF(tag, bb);
                case "OW": return new AttributeOW(tag, bb);
                case "PN": return new AttributePN(tag, bb);
                case "SH": return new AttributeSH(tag, bb);
                case "SL": return new AttributeSL(tag, bb);
                //case "SQ": return new AttributeSQ(tag, bb);
                case "SS": return new AttributeSS(tag, bb);
                case "ST": return new AttributeST(tag, bb);
                case "TM": return new AttributeTM(tag, bb);
                case "UI": return new AttributeUI(tag, bb);
                case "UL": return new AttributeUL(tag, bb);
                case "UN": return new AttributeUN(tag, bb);
                case "US": return new AttributeUS(tag, bb);
                case "UT": return new AttributeUT(tag, bb);
            }

            return null;

        }
        #endregion

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(AbstractAttribute attr)
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
                        AttributeUI ui = this as AttributeUI;
                        DicomUid uid = ui.GetUid(0);
                        if (uid != null && uid.Type != UidType.Unknown)
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
