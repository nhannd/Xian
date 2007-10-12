#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.IO;

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
        private DicomAttributeCollection _parentCollection = null;
        #endregion

        #region Abstract and Virtual Methods
        /// <summary>
        /// Method to return a string representation of the attribute.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
        /// <summary>
        /// Method to check if two attributes are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the attributes are equal.</returns>
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
        public abstract bool IsNull { get; }
        public abstract bool IsEmpty { get; }
        public abstract Object Values { get; set; }
        public abstract DicomAttribute Copy();
        public abstract void SetStringValue(String stringValue);
        public abstract Type GetValueType();
        public abstract void SetNullValue();

        internal abstract ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet);
        internal abstract DicomAttribute Copy(bool copyBinary);

        /// <summary>
        /// Calculate the length to write the attribute.
        /// </summary>
        /// <param name="syntax">The transfer syntax to calculate the length for.</param>
        /// <param name="options">The write options to calculate the length for.</param>
        /// <returns></returns>
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

        public virtual void AppendInt32(int intValue)
        {
            throw new DicomException(SR.InvalidType);
        }

        public virtual void AppendString(string stringValue)
        {
            throw new DicomException(SR.InvalidType);
        }

        /// <summary>
        /// Retrieves a <see cref="DicomUid"/> instance for a value.
        /// </summary>
        /// <remarks>This function only works for <see cref="DicomAttributeUI"/> attributes.</remarks>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns>True on success, false on failure.</returns>
        public virtual bool TryGetUid(int i, out DicomUid value)
        {
            value = null;
            return false;
        }

        /// <summary>
        /// Method to retrieve a <see cref="DateTime"/> attribute for the tag.
        /// </summary>
        /// <param name="i">A zero index value to retrieve.</param>
        /// <param name="value"></param>
        /// <returns>true on success, false on failure.</returns>
        public virtual bool TryGetDateTime(int i, out DateTime value)
        {
            value = new DateTime();
            return false;
        }

        /// <summary>
        /// Method for adding a <see cref="DicomSequenceItem"/> to an attributes value.
        /// </summary>
        /// <remarks>
        /// This method is value for <see cref="DicomAttributeSQ"/> attributes only.
        /// </remarks>
        /// <param name="item">The <see cref="DicomSequenceItem"/> to add to the attribute.</param>
        public virtual void AddSequenceItem(DicomSequenceItem item)
        {
            throw new DicomException(SR.InvalidType);
        }

        #endregion

        #region Internal Properties
        /// <summary>
        /// The <see cref="DicomAttributeCollection"/> that the attribute is contained in.
        /// </summary>
        /// <remarks>
        /// This field is used to determine the Specific Character Set of a string attribute.
        /// </remarks>
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

        /// <summary>
        /// The length in bytes if the attribute was placed in a DICOM stream.
        /// </summary>
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

        /// <summary>
        /// The number of values assigned to the attribute.
        /// </summary>
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
        /// <summary>
        /// Method for dumping the contents of the attribute to a string.
        /// </summary>
        /// <param name="sb">The StringBuilder to write the attribute to.</param>
        /// <param name="prefix">A prefix to place before the value.</param>
        /// <param name="options">The <see cref="DicomDumpOptions"/> to use for the output string.</param>
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
