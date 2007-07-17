using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom
{

    #region DicomAttributeBinary<T>
    public abstract class DicomAttributeBinary<T> : DicomAttribute
    {
        protected T[] _values;
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

            Count = _values.Length;
            StreamLength = (uint)( Count * tag.VR.UnitSize);
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
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }

        #endregion
        #region private Methods
        private object ParseNumber(string val)
        {
            try
            {
                if (typeof(T) == typeof(byte))
                {
                    return byte.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(sbyte))
                {
                    return sbyte.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(short))
                {
                    return short.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(ushort))
                {
                    return ushort.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(int))
                {
                    return int.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(uint))
                {
                    return uint.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(long))
                {
                    return long.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(ulong))
                {
                    return ulong.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(float))
                {
                    return float.Parse(val, NumberStyle);
                }
                if (typeof(T) == typeof(double))
                {
                    return double.Parse(val, NumberStyle);
                }

            }
            catch { }
            return null;
        }

        protected void SetStreamLength()
        {
            Count = _values.Length;
            StreamLength = (uint)(_values.Length * Tag.VR.UnitSize);
        }

        #endregion

        #region Abstract Methods
        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax)
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

        public override Type GetValueType()
        {
            return typeof(T);
        }

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

        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue.Length == 0)
            {
                Count = 1;
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

        public abstract override Object Values { get; set; }
        public abstract override DicomAttribute Copy();
        internal abstract override DicomAttribute Copy(bool copyBinary);

        #endregion
    }
    #endregion

    #region DicomAttributeAT
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

        public override bool TryGetUInt32(int i, out uint value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
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

        #endregion
    }
    #endregion

    #region DicomAttributeFD
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

        public override bool TryGetFloat32(int i, out float value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = (float)_values[i];
            return true;
        }
        public override bool TryGetFloat64(int i, out double value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = _values[i];
            return true;
        }
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

        #endregion
    }
    #endregion

    #region DicomAttributeFL
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

        public override bool TryGetFloat32(int i, out float value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = _values[i];
            return true;
        }
        public override bool TryGetFloat64(int i, out double value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0.0f;
                return false;
            }

            value = (double)_values[i];
            return true;
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
        #endregion
    }
    #endregion

    #region DicomAttributeOB
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
            return base.Tag + " of length " + base.StreamLength;
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
    public class DicomAttributeOW : DicomAttributeBinary<ushort>
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
            : base(tag, item)
        {
        }


        internal DicomAttributeOW(DicomAttributeOW attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag + " of length " + base.StreamLength;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is ushort[])
                {
                    _values = (ushort[])value;
                    SetStreamLength();
                }
                else if (value is byte[])
                {
                    byte[] vals = (byte[])value;

                    _values = new ushort[vals.Length / Tag.VR.UnitSize];
                    Buffer.BlockCopy(vals, 0, _values, 0, vals.Length);

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

        #endregion


    }
    #endregion

    #region DicomAttributeSL
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

        public override bool TryGetInt32(int i, out int value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }



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

        #endregion
    }
    #endregion

    #region DicomAttributeSS
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

        public override bool TryGetInt16(int i, out short value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }

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

        #endregion
    }
    #endregion

    #region DicomAttributeUL
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

        public override bool TryGetUInt32(int i, out uint value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
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

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUL(this);
        }

        #endregion
    }
    #endregion 

    #region DicomAttributeUN
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
            return base.Tag.ToString(); // TODO
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

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.FromBytes(_values);

            return bb;
        }
        #endregion
    }
    #endregion

    #region DicomAttributeUS
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

        public override bool TryGetUInt16(int i, out ushort value)
        {
            if (_values == null || _values.Length <= i)
            {
                value = 0;
                return false;
            }

            value = _values[i];
            return true;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is ushort[])
                {
                    _values = (ushort[])value;
                    SetStreamLength();
                }
                else if (value is ushort)
                {
                    _values = new ushort[1];
                    _values[0] = (ushort)value;
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
            return new DicomAttributeUS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUS(this);
        }

        #endregion
    }
    #endregion

}
