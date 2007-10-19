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
using System.Globalization;
using System.Text;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{

    #region DicomAttributeBinary<T>
    /// <summary>
    /// <see cref="DicomAttribute"/> derived class used to represent tags with binary values.
    /// </summary>
    /// <typeparam name="T">The type that the attribute is storing.</typeparam>
    
    public abstract class DicomAttributeBinary<T> : DicomAttribute
    {
        protected T[] _values = new T[0];
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors
        internal DicomAttributeBinary(uint tag) 
            : base(tag)
        {            
        }

        internal DicomAttributeBinary(DicomTag tag)
            : base(tag)
        {   
        }

        internal DicomAttributeBinary(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize);
            _values = new T[item.Length / tag.VR.UnitSize];
            Buffer.BlockCopy(item.ToBytes(), 0, _values, 0, _values.Length * tag.VR.UnitSize);

            SetStreamLength();
        }

        internal DicomAttributeBinary(DicomAttributeBinary<T> attrib)
            : base(attrib)
        {
            T[] values = (T[])attrib.Values;

            _values = new T[values.Length];

            values.CopyTo(_values, 0);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number style for the attribute.
        /// </summary>
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }

        #endregion

        #region private Methods
        private object ParseNumber(string val)
        {
            if (typeof (T) == typeof (byte))
            {
                byte parseVal;
                if (false == byte.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid byte format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (sbyte))
            {
                sbyte parseVal;
                if (false == sbyte.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid sbyte format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (short))
            {
                short parseVal;
                if (false == short.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid short format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (ushort))
            {
                ushort parseVal;
                if (false == ushort.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid ushort format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (int))
            {
                int parseVal;
                if (false == int.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid int format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (uint))
            {
                uint parseVal;
                if (false == uint.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid uint format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (long))
            {
                long parseVal;
                if (false == long.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid long format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (ulong))
            {
                ulong parseVal;
                if (false == ulong.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid ulong format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (float))
            {
                float parseVal;
                if (false == float.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid float format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }
            if (typeof (T) == typeof (double))
            {
                double parseVal;
                if (false == double.TryParse(val, NumberStyle, null, out parseVal))
                    throw new DicomDataException(
                        String.Format("Invalid double format value for tag {0}: {1}", Tag.ToString(), val));
                return parseVal;
            }

            return null;
        }
        protected virtual void SetStreamLength()
        {
            Count = _values.Length;
            StreamLength = (uint) (_values.Length*Tag.VR.UnitSize);
        }
        #endregion

        #region Abstract Methods

        public override void SetNullValue()
        {
            _values = new T[0];
            SetStreamLength();
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {    
            int len = _values.Length * Tag.VR.UnitSize;
            byte[] byteVal = new byte[len];

            Buffer.BlockCopy(_values, 0, byteVal, 0, len);

            ByteBuffer bb = new ByteBuffer(byteVal,syntax.Endian);
            if (syntax.Endian != ByteBuffer.LocalMachineEndian)
            {
                bb.Swap(Tag.VR.UnitSize);
            }

            return bb;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeBinary<T> a = (DicomAttributeBinary<T>)obj;
            T[] array = (T[])a.Values;

            if (Count != a.Count)
                return false;
            if (Count == 0 && a.Count == 0)
                return true;
            if (array.Length != _values.Length)
                return false;

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
            {
                int hash = 0;
                for (int i = 0; i < _values.Length; i++)
                {
                    hash += (i + 1) * _values[i].GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// The type that the attribute stores.
        /// </summary>
        /// <returns></returns>
        public override Type GetValueType()
        {
            return typeof(T);
        }

        protected virtual void ValidateStringValue(string value)
        {
        }

        /// <summary>
        /// Retrieve a value as a string.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetString(int i, out String value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = "";
                return false;
            }

            value = _values[i].ToString();

            return true;
        }

        /// <summary>
        /// Set the tag value(s) from a string
        /// If the string cannot be converted into tag's VR, DicomDataException will be thrown
        /// </summary>
        /// <param name="stringValue"></param>
        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 0; // SetStringValue(null) should be interpreted as "Clear"
                StreamLength = 0;
                _values = new T[0];
                return;
            }

            String[] stringValues = stringValue.Split(new char[] { '\\' });

            _values = new T[stringValues.Length];

            for (int i = 0; i < stringValues.Length; i++)
            {
                _values[i] = (T)ParseNumber(stringValues[i]);
            }

            SetStreamLength();
        }

        /// <summary>
        /// Set the value from a string
        /// If the string cannot be converted into tag's VR, DicomDataException will be thrown
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves the same as <seealso cref="AppendString"/>.
        /// If <i>index</i> is less than 0 or greater than <see cref="Count"/>, IndexOutofBoundException will be thrown.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void SetString(int index, string value)
        {
            if (index == Count)
                AppendString(value);
            else
            {
                _values[index] = (T)ParseNumber(value);
                SetStreamLength();
            }
            
        }

        /// <summary>
        /// Append an element from a string
        /// If the string cannot be converted into tag's VR, DicomDataException will be thrown
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void AppendString(string value)
        {
            T v = (T)ParseNumber(value);

            T[] temp = new T[Count + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(temp, 0);

            _values = temp;
            _values[Count++] = v;
            SetStreamLength();
        }

        public override string ToString()
        {
            if (_values == null)
                return "";

            StringBuilder val = null;
            foreach (T i in _values)
            {
                if (val == null)
                    val = new StringBuilder(i.ToString());
                else
                    val.AppendFormat("\\{0}", i.ToString());
            }

            if (val == null)
                return "";

            return val.ToString();
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
        /// <summary>
        /// Abstract property for setting or getting the values associated with the attribute.
        /// </summary>
        public abstract override Object Values { get; set; }
        public abstract override DicomAttribute Copy();
        internal abstract override DicomAttribute Copy(bool copyBinary);

        #endregion

    }
    #endregion

    #region DicomAttributeAT
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing AT value representation tags.
    /// </summary>
    /// 
    public class DicomAttributeAT : DicomAttributeBinary<uint>
    {

        #region Constructors

        public DicomAttributeAT(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeAT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ATvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeAT(DicomAttributeAT attrib)
            : base(attrib)
        {
        }


        #endregion


        #region Operators

        public uint this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion


        #region Abstract Method Implementation

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is uint[])
                {
                    _values = (uint[])value;
                    SetStreamLength();
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAT(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAT(this);
        }



        /// <summary>
        /// Set AT value(s) from a string. Each AT element is a hexadecimal string representing
        /// a tag (ggggeeee) and separated by "\"
        /// </summary>
        /// 
        /// <remarks>
        /// If the string cannot be converted into AT VR, DicomDataException will be thrown
        /// If <i>index</i> equals to <seealso cref="Count"/>, this method behaves the same as <seealso cref="AppendString"/>.
        /// If <i>index</i> is less than 0 or greater than <see cref="Count"/>, IndexOutofBoundException will be thrown.
        /// </remarks>
        /// 
        /// <param name="stringValue"></param>
        /// <exception cref="IndexOutofBoundException"/>
        /// <exception cref="DicomDataException"/>
        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 0;
                StreamLength = 0;
                _values = new uint[0];
                return;
            }

            String[] stringValues = stringValue.Split(new char[] { '\\' });

            _values = new uint[stringValues.Length];

            for (int i = 0; i < stringValues.Length; i++)
            {
                _values[i] = uint.Parse(stringValues[i].Trim(), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            SetStreamLength();
        }

        /// <summary>
        /// Method to retrieve an Int16 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out Int16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            
            value = (Int16) _values[i];

            // If the value cannot fit into the destination, return false
            if (_values[i] < Int16.MinValue || _values[i] > Int16.MaxValue)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }


            value = (Int32)_values[i];

            // If the value cannot fit into the destination, return false
            if (_values[i] < Int32.MinValue || _values[i] > Int32.MaxValue)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Method to retrieve an Int64 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }

        /// <summary>
        /// Method to retrieve an UInt16 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int i, out UInt16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = (UInt16)_values[i];
            
            // If the value cannot fit into the destination, return false
            if (_values[i] < UInt16.MinValue || _values[i] > UInt16.MaxValue)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }
        
        /// <summary>
        /// Return string representation of the AT elements, separated by "\". Each element is a tag in hexadecimal format (ggggeeee)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetString(int i, out string value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = "";
                return false;
            }

            value = _values[i].ToString("X8"); // Convert to HEX
            return true;
        }

        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetUInt32(index, (UInt32)value);
        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {

            if (index == Count)
            {
                AppendUInt32(value);
            }
            else // replace
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetUInt32(index, (UInt32)value);
        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetInt32(index, (Int32)value);
        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        /// 
        public override void SetInt32(int index, Int32 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetUInt32(index, (UInt32)value);
        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetInt32(index, (Int32)value);
        }

        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendUInt16(UInt16 intValue)
        {
            AppendUInt32((UInt32)intValue);
        }
        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// 
        public override void AppendUInt32(UInt32 intValue)
        {
            // If the source value cannot fit into the destination throw exception
            if (intValue < uint.MinValue || intValue > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));


            uint[] temp = new uint[Count + 1];
            if (_values != null && _values.Length > 0)
                Array.Copy(_values, temp, Count);

            temp[Count++] = (uint)intValue;
            _values = temp;
            SetStreamLength();
        }

        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        ///
        public override void AppendUInt64(UInt64 intValue)
        {
            // If the source value cannot fit into the destination throw exception
            if (intValue < UInt32.MinValue || intValue > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));
            AppendUInt32((UInt32) intValue);
        }

        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendInt16(Int16 intValue)
        {
            AppendInt32((Int32)intValue);
        }

        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        ///
        public override void AppendInt32(Int32 intValue)
        {
            // If the source value cannot fit into the destination throw exception
            if (intValue < UInt32.MinValue || intValue > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));

            AppendUInt32((UInt32)intValue);
        }

        /// <summary>
        /// Append an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        ///
        public override void AppendInt64(Int64 intValue)
        {
            // If the source value cannot fit into the destination throw exception
            if (intValue < UInt32.MinValue || intValue > UInt32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));
            AppendInt32((Int32) intValue);
        }


        #endregion

    }
    #endregion

    #region DicomAttributeFD
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing FD value representation tags.
    /// </summary>
    /// 
    public class DicomAttributeFD : DicomAttributeBinary<double>
    {
        #region Constructors

        public DicomAttributeFD(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeFD(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FDvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeFD(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeFD(DicomAttributeFD attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators

        public double this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation
        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is double[])
                {
                    _values = (double[])value;
                    SetStreamLength();
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeFD(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeFD(this);
        }
        
        
        /// <summary>
        /// Set an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat32(int index, float value)
        {
            if (index == Count)
                AppendFloat32(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat64(int index, double value)
        {
            if (index == Count)
                AppendFloat64(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }

        }
        /// <summary>
        /// Set an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat32(float value)
        {
            AppendFloat64(value);

        }
        /// <summary>
        /// Set an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat64(double value)
        {
            double[] temp = new double[Count + 1];
            if (_values != null && _values.Length > 0)
                Array.Copy(_values, temp, _values.Length);
            _values = temp;
            _values[Count++] = value;
            SetStreamLength();
        }
        
        
        #endregion

        /// <summary>
        /// Method to retrieve a float value from an FD attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value is too big or too small to fit into a float (eg, 1E+100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat32(int i, out float value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0.0f;
                return false;
            }

            value =(float) _values[i];
            
            if (_values[i] < float.MinValue || _values[i] > float.MaxValue)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Method to retrieve a double value from an FD attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat64(int i, out double value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0.0f;
                return false;
            }

            value = _values[i];

            if (_values[i] < double.MinValue || _values[i] > double.MaxValue)
            {
                return false;
            }
            else
                return true;
        }
    }
    #endregion

    #region DicomAttributeFL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing FL value representation tags.
    /// </summary>
    public class DicomAttributeFL : DicomAttributeBinary<float>
    {
        #region Constructors

        public DicomAttributeFL(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeFL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeFL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeFL(DicomAttributeFL attrib)
            : base(attrib)
        {
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


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is float[])
                {
                    _values = (float[])value;
                    SetStreamLength();
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeFL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeFL(this);
        }



        /// <summary>
        /// Method to retrieve a float value from an FL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value is infinite
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat32(int i, out float value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = _values[i];
            if (float.IsInfinity(value))
                return false;
            else
                return true;
        }
        /// <summary>
        /// Method to retrieve a double value from an FL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat64(int i, out double value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = (double) (decimal)_values[i]; // casting to decimal then double seems to prevent precision loss
            if (double.IsInfinity(value))
                return false;
            else
                return true;
        }


        /// <summary>
        /// Set an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat32(int index, float value)
        {
            if (value < float.MinValue || value > float.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for FL tag {1}.", value, Tag.ToString()));

            if (index == Count)
                AppendFloat32(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat64(int index, double value)
        {
            SetFloat32(index, (float)value);
        }
        /// <summary>
        /// Append an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat32(float value)
        {
            if (value < float.MinValue || value > float.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for FL tag {1}.", value, Tag.ToString()));
            
            float[] temp = new float[Count + 1];
            if (_values != null && _values.Length > 0)
                Array.Copy(_values, temp, _values.Length);
            _values = temp;
            _values[Count++] = value;
            SetStreamLength();
        }
        /// <summary>
        /// Append an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat64(double value)
        {

            AppendFloat32((float)value);
        }


        
        #endregion
    }
    #endregion

    #region DicomAttributeOB
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OB value representation tags.
    /// </summary>
    public class DicomAttributeOB : DicomAttributeBinary<byte>
    {
        #region Protected Members
        protected List<uint> _table;
        protected List<ByteBuffer> _fragments = new List<ByteBuffer>();
        #endregion

        #region Constructors

        public DicomAttributeOB(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeOB(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OBvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeOB(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeOB(DicomAttributeOB attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is byte[])
                {
                    _values = (byte[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOB(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOB(this);
        }

        #endregion

        #region Public Properties
        public bool HasOffsetTable
        {
            get { return _table != null; }
        }

        public ByteBuffer OffsetTableBuffer
        {
            get
            {
                ByteBuffer offsets = new ByteBuffer();
                if (_table != null)
                {
                    foreach (uint offset in _table)
                    {
                        offsets.Writer.Write(offset);
                    }
                }
                return offsets;
            }
        }

        public List<uint> OffsetTable
        {
            get
            {
                if (_table == null)
                    _table = new List<uint>();
                return _table;
            }
        }

        public IList<ByteBuffer> Fragments
        {
            get { return _fragments; }
        }
        #endregion

        #region Public Methods
        public void SetOffsetTable(ByteBuffer table)
        {
            _table = new List<uint>();
            _table.AddRange(table.ToUInt32s());
        }
        public void SetOffsetTable(List<uint> table)
        {
            _table = new List<uint>(table);
        }

        public void AddFragment(ByteBuffer fragment)
        {
            _fragments.Add(fragment);
        }
        #endregion

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
            length += 4 + 4; // offset tag
            if (Flags.IsSet(options, DicomWriteOptions.WriteFragmentOffsetTable) && _table != null)
                length += (uint)(_table.Count * 4);
            foreach (ByteBuffer bb in _fragments)
            {
                length += 4; // item tag
                length += 4; // fragment length
                length += (uint)bb.Length;
            }
            return length;
        }

        
    }
    #endregion


    #region DicomAttributeOF
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OF value representation tags.
    /// </summary>
    public class DicomAttributeOF : DicomAttributeBinary<float>
    {
        public DicomAttributeOF(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeOF(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OFvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeOF(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeOF(DicomAttributeOF attrib)
            : base(attrib)
        {
        }


        #region Abstract Method Implementation

        public override string ToString()
        {
            return Tag + " of length " + base.StreamLength;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is float[])
                {
                    _values = (float[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOF(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOF(this);
        }

        #endregion
    }
    #endregion

    #region DicomAttributeOW
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OW value representation tags.
    /// </summary>
    public class DicomAttributeOW : DicomAttributeBinary<byte>
    {
        public DicomAttributeOW(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeOW(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OWvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeOW(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            
            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize);

            _values = new byte[item.Length];
            Buffer.BlockCopy(item.ToBytes(), 0, _values, 0, _values.Length);

            SetStreamLength();
        }


        internal DicomAttributeOW(DicomAttributeOW attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        protected override void SetStreamLength()
        {
            Count = _values.Length;
            StreamLength = (uint) (_values.Length);
        }

        public override string ToString()
        {
            return Tag + " of length " + base.StreamLength;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is ushort[])
                {
                    ushort[] vals = (ushort[])value;

                    _values = new byte[vals.Length * Tag.VR.UnitSize];
                    Buffer.BlockCopy(vals, 0, _values, 0, _values.Length);

                    SetStreamLength();
                }
                else if (value is byte[])
                {
                    _values = (byte[])value;

                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOW(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOW(this);
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            int len = _values.Length;
            byte[] byteVal = new byte[len];

            Buffer.BlockCopy(_values, 0, byteVal, 0, len);

            ByteBuffer bb = new ByteBuffer(byteVal, syntax.Endian);
            if (syntax.Endian != ByteBuffer.LocalMachineEndian)
            {
                bb.Swap(Tag.VR.UnitSize);
            }

            return bb;
        }

        #endregion


    }
    #endregion

    #region DicomAttributeSL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing SL value representation tags.
    /// </summary>
    public class DicomAttributeSL : DicomAttributeBinary<int>
    {
        public DicomAttributeSL(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeSL(DicomAttributeSL attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is int[])
                {
                    _values = (int[])value;
                    SetStreamLength();
                }
                else if (value is int)
                {
                    _values = new int[1];
                    _values[0] = (int)value;
                    SetStreamLength();
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSL(this);
        }

        
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid value '{0}' for SL VR {1}.", value, Tag.ToString()));
            
            SetInt32(index, (int)value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));
            
            SetInt32(index, (Int32)value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, int value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            if (index == Count)
                AppendInt32(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            if (index == Count)
                AppendUInt32(value);
            else
            {
                _values[index] = (Int32) value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            SetInt32(index,(Int32)value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value>Int32.MaxValue )
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            SetUInt32(index, (UInt32)value);
        }

        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        /// 
        public override void AppendInt16(Int16 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));
            
            AppendInt32((int)value);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendInt32(int intValue)
        {
            if (intValue < int.MinValue || intValue > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid value '{0}' for tSL VRag {1}.", intValue, Tag.ToString()));
            
            int[] temp = new int[Count + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(temp, 0);

            _values = temp;
            _values[Count++] = intValue;
            SetStreamLength();
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendInt64(Int64 intValue)
        {
            if (intValue < Int32.MinValue || intValue > Int32.MaxValue)
                throw new DicomDataException(string.Format("Invalid value '{0}' for tSL VRag {1}.", intValue, Tag.ToString()));

            AppendInt32((Int32)intValue);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendUInt16(UInt16 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            AppendInt32((Int32)value);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendUInt32(UInt32 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            AppendInt32((Int32)value);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendUInt64(UInt64 value)
        {
            if ( value > Int32.MaxValue)
                throw new DicomDataException(string.Format("Invalid SL value '{0}' for {1}.", value, Tag.ToString()));

            AppendUInt32((UInt32)value);
        }


        /// <summary>
        /// Method to retrieve an Int16 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out Int16 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }

            
            if (_values[i] < Int16.MinValue || _values[i] > Int16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int16)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }
        /// <summary>
        /// Method to retrieve an Int64 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[i] < Int64.MinValue || _values[i] > Int64.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int64)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt16 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int i, out UInt16 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[i] < UInt16.MinValue || _values[i] > UInt16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt16)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[i] < UInt32.MinValue || _values[i] > UInt32.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt32)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from a SL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (i < 0 || i >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[i] < 0 )
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt16)_values[i];
                return true;
            }
        }
        
       
        #endregion
    }
    #endregion

    #region DicomAttributeSS
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing SS value representation tags.
    /// </summary>
    public class DicomAttributeSS : DicomAttributeBinary<short>
    {
        #region Constructors

        public DicomAttributeSS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SSvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeSS(DicomAttributeSS attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators

        public short this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation

        
        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is short[])
                {
                    _values = (short[])value;
                    SetStreamLength();
                }
                else if (value is short)
                {
                    _values = new short[1];
                    _values[0] = (short)value;
                    SetStreamLength();
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSS(this);
        }

        
        
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt16(Int16 intValue)
        {
            int newArrayLength = 1;
            int oldArrayLength = 0;

            if (_values != null)
            {
                newArrayLength = _values.Length + 1;
                oldArrayLength = _values.Length;
            }

            short[] newArray = new short[newArrayLength];
            if (oldArrayLength > 0)
                _values.CopyTo(newArray, 0);
            newArray[newArrayLength - 1] = (short)intValue;
            _values = newArray;
            SetStreamLength();
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt32(Int32 intValue)
        {
            if (intValue < Int16.MinValue || intValue > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));

            AppendInt16((Int16)intValue);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt64(Int64 intValue)
        {
            if (intValue < Int16.MinValue || intValue > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));

            AppendInt32((Int32)intValue);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt16(UInt16 intValue)
        {
            if (intValue < Int16.MinValue || intValue > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", intValue, Tag.ToString()));

            AppendInt16((Int16)intValue);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt32(UInt32 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            AppendUInt16((UInt16)value);
        }
        /// <summary>
        /// Append an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt64(UInt64 value)
        {
            if (value > (UInt64) Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            AppendUInt32((UInt32)value);
        }

        /// <summary>
        /// Method to retrieve an Int16 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out Int16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < Int16.MinValue || _values[i] > Int16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = _values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < Int32.MinValue || _values[i] > Int32.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = _values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an Int64 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < Int64.MinValue || _values[i] > Int64.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = _values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt16 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int i, out UInt16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < UInt16.MinValue || _values[i] > UInt16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt16)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < UInt32.MinValue || _values[i] > UInt32.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt32)_values[i];
                return true;
            }
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if ((UInt64)_values[i] < UInt64.MinValue || (UInt64)_values[i] > UInt64.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt64)_values[i];
                return true;
            }
        }
        
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            if (index == Count)
                AppendInt16(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, Int32 value)
        {
            if (value<Int16.MinValue || value>Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));
            
            SetInt16(index, (Int16) value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetInt32(index, (Int32)value);
        }

        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetInt16(index, (Int16)value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetUInt16(index, (UInt16)value);
        }
        /// <summary>
        /// Set an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value > (UInt64)Int16.MaxValue)
                throw new DicomDataException(string.Format("Invalid value {0} for tag {1}.", value, Tag.ToString()));

            SetUInt32(index, (UInt32)value);
        }
        
        #endregion
    }
    #endregion

    #region DicomAttributeUL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing UL value representation tags.
    /// </summary>
    public class DicomAttributeUL : DicomAttributeBinary<uint>
    {
        #region Constructors

        public DicomAttributeUL(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ULvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUL(DicomAttributeUL attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators

        public uint this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUL(this);
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is uint[])
                {
                    _values = (uint[])value;
                    SetStreamLength();
                }
                else if (value is uint)
                {
                    _values = new uint[1];
                    _values[0] = (uint)value;
                    SetStreamLength();
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
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt16(Int16 intValue)
        {
            AppendInt32((Int32)intValue);
        }
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt32(Int32 intValue)
        {
            if (intValue < uint.MinValue || intValue > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", intValue, Tag.ToString()));

            UInt32[] newArray = new UInt32[Count + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(newArray, 0);
            newArray[Count++] = (UInt32)intValue;
            _values = newArray;
            SetStreamLength();
        }
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt64(Int64 intValue)
        {
            if (intValue < uint.MinValue || intValue > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", intValue, Tag.ToString()));

            AppendInt32((Int32)intValue);
        }
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendUInt16(UInt16 intValue)
        {
            AppendUInt32((UInt32)intValue);
        }
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendUInt32(UInt32 intValue)
        {
            if (intValue < uint.MinValue || intValue > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", intValue, Tag.ToString()));

            UInt32[] newArray = new UInt32[Count + 1];
            if (_values!=null && _values.Length > 0)
                _values.CopyTo(newArray, 0);
            newArray[Count++] = (UInt32)intValue;
            _values = newArray;
            SetStreamLength();
        }
        /// <summary>
        /// Append an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendUInt64(UInt64 intValue)
        {
            if (intValue < uint.MinValue || intValue > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", intValue, Tag.ToString()));

            AppendUInt32((UInt32)intValue);
        }

        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            SetInt32(index, (Int32)value);
        }
        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, Int32 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", value, Tag.ToString()));

            if (index == Count)
                AppendInt32(value);
            else
            {
                _values[index] = (UInt32) value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", value, Tag.ToString()));
            SetInt32(index, (Int32)value);
        }

        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            SetUInt32(index, (UInt32)value);
        }
        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            if (index == Count)
                AppendUInt32(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
        }
        /// <summary>
        /// Set an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(string.Format("Invalid UL value '{0}' for {1}.", value, Tag.ToString()));
            SetUInt32(index, (UInt32)value);
        }

        /// <summary>
        /// Method to retrieve an Int16 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out Int16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] > (UInt32)Int16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int16)_values[i];
                return true;
            }
            
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < Int32.MinValue || _values[i] > Int32.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int32)_values[i];
                return true;
            }

        }
        /// <summary>
        /// Method to retrieve an Int64 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < Int64.MinValue || _values[i] > Int64.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int64)_values[i];
                return true;
            }

        }
        /// <summary>
        /// Set an AT value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        
        /// <summary>
        /// Method to retrieve an UInt16 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int i, out UInt16 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < UInt16.MinValue || _values[i] > UInt16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt16)_values[i];
                return true;
            }

        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            if (_values[i] < UInt64.MinValue || (UInt64)_values[i] > UInt64.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt64)_values[i];
                return true;
            }

        }
        
        

       

        #endregion
    }
    #endregion 

    #region DicomAttributeUN
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing UN value representation tags.
    /// </summary>
    public class DicomAttributeUN : DicomAttributeBinary<byte>
    {
        #region Constructors

        public DicomAttributeUN(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUN(DicomTag tag)
            : base(tag)
        {
        }

        internal DicomAttributeUN(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            _values = item.ToBytes();

            SetStreamLength();
        }

        internal DicomAttributeUN(DicomAttributeUN attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            return Tag.ToString(); // TODO
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            return false;  //TODO
        }

        public override int GetHashCode()
        {
            if (_values == null)
                return 0; // TODO
            else
                return _values.GetHashCode(); // TODO
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

        public override object Values
        {
            get { return _values; }
            set
            {
                if (value is byte[])
                {
                    _values = (byte[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUN(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUN(this);
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.FromBytes(_values);

            return bb;
        }
        #endregion
    }
    #endregion

    #region DicomAttributeUS
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing US value representation tags.
    /// </summary>
    public class DicomAttributeUS : DicomAttributeBinary<ushort>
    {
        #region Constructors

        public DicomAttributeUS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.USvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUS(DicomAttributeUS attrib)
            : base(attrib)
        {
            ushort[] values = (ushort[])attrib.Values;

            _values = new ushort[values.Length];

            values.CopyTo(_values, 0);
        }


        #endregion

        #region Operators

        public ushort this[int val]
        {
            get
            {
                return _values[val];
            }
        }

        #endregion

        #region Abstract Method Implementation
        public override Object Values
        {
            get { return _values; }
            set
            {
                _values = value as ushort[];
                if (_values != null)
                {
                    SetStreamLength();
                    return;
                }

                String str = value as string;
                if (str != null)
                {
                    SetStringValue((String)value);
                    return;
                }

                if (value is ushort)
                {
                    _values = new ushort[1];
                    _values[0] = (ushort)value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUS(this);
        }



        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, short value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
            SetUInt16(index, (UInt16)value);
        }
        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, int value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
            SetInt16(index, (Int16)value);
        }
        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
            SetInt32(index, (Int32)value);
        }
        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
           
            if (index == Count)
                AppendUInt16( value);
            else
            {
                _values[index] = (UInt16)value;
                SetStreamLength();
            }
            
        }
        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, uint value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
            SetUInt16(index,(UInt16)value);
        }
        /// <summary>
        /// Set an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <i>value</i> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <i>index</i> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", value, Tag.ToString()));
            SetUInt32(index, (UInt32)value);
        }

        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendInt16(Int16 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));
            AppendUInt16((UInt16)intValue);
        }
        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendInt32(Int32 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));
            
            AppendInt16((Int16)intValue);
        }
        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendInt64(Int64 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));

            AppendInt32((Int32)intValue);
        }
        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendUInt16(UInt16 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));

            UInt16[] temp = new UInt16[Count + 1];
            if (_values != null && _values.Length > 0)
                _values.CopyTo(temp, 0);

            _values = temp;
            _values[Count++] = intValue;
            SetStreamLength();

        }
        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendUInt32(UInt32 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));
            AppendUInt16((UInt16)intValue);

        }
        /// <summary>
        /// Append an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="intValue"></param>
        /// <exception cref="DicomDataException">If <i>intValue</i> cannot be fit into 16-bit unsigned int</exception>
        /// 
        public override void AppendUInt64(UInt64 intValue)
        {
            if (intValue < UInt16.MinValue || intValue > UInt16.MaxValue)
                throw new DicomDataException(string.Format("Invalid US value '{0}' for  VR {1}.", intValue, Tag.ToString()));
            AppendUInt32((UInt32)intValue);

        }
        
        /// <summary>
        /// Method to retrieve an Int16 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int i, out short value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = (Int16)_values[i];
            if (_values[i] < Int16.MinValue || _values[i] > Int16.MaxValue)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Method to retrieve an Int32 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int i, out Int32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = (Int32)_values[i];
            if (_values[i] < Int32.MinValue || _values[i] > Int32.MaxValue)
                return false;
            else
                return true;
            return true;
        }
        /// <summary>
        /// Method to retrieve an Int64 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int64 (eg, floating-point number 1.102 cannot be converted into Int64)
        ///     The value is an integer but too big or too small to fit into an Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int i, out Int64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = (Int64)_values[i];
            if (_values[i] < Int64.MinValue || _values[i] > Int64.MaxValue)
                return false;
            else
                return true;
            return true;
        }
        
        /// <summary>
        /// Method to retrieve an UInt16 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but too big or too small to fit into an UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int i, out ushort value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            if (_values[i] < UInt16.MinValue || _values[i] > UInt16.MaxValue)
                return false;
            else
                return true;
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt32 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but too big or too small to fit into an UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int i, out UInt32 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            if (_values[i] < UInt32.MinValue || _values[i] > UInt32.MaxValue)
                return false;
            else
                return true;
            return true;
        }
        /// <summary>
        /// Method to retrieve an UInt64 value from an US attribute.
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        /// <returns><i>true</i>if value can be retrieved. <i>false</i> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <i>false</i> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but too big or too small to fit into an UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <i>value</i> should not be trusted.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int i, out UInt64 value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            if (_values[i] < UInt64.MinValue || _values[i] > UInt64.MaxValue)
                return false;
            else
                return true;
            return true;
        }
        
        

        #endregion
    }
    #endregion

}
