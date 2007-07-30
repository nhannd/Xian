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
        private DicomAttributeCollection _parentCollection = null;
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
        
        internal abstract ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet);
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

        #region Internal Properties
        internal DicomAttributeCollection ParentCollection
        {
            get { return _parentCollection; }
            set { _parentCollection = value; }
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
            _tag = DicomTagDictionary.GetDicomTag(tag);
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

        public virtual uint StreamLength
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

        #region Operators
        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        public static implicit operator String(DicomAttribute attr)
        {
            // Uses the actual ToString implementation of the derived class.
            return attr.ToString();
        }
        #endregion

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
