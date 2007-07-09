using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;

using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;

namespace ClearCanvas.ImageServer.Dicom
{

    #region AttributeBinary<T>
    public abstract class AttributeBinary<T> : AbstractAttribute
    {
        protected T[] _values;
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors
        internal AttributeBinary(uint tag) 
            : base(tag)
        {            
        }

        internal AttributeBinary(DicomTag tag)
            : base(tag)
        {   
        }

        internal AttributeBinary(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize);
            _values = new T[item.Length / tag.VR.UnitSize];
            Buffer.BlockCopy(item.ToBytes(), 0, _values, 0, _values.Length * tag.VR.UnitSize);

            Count = _values.Length;
            StreamLength = (uint)( Count * tag.VR.UnitSize);
        }

        internal AttributeBinary(AttributeBinary<T> attrib)
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

            AttributeBinary<T> a = (AttributeBinary<T>)obj;
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

            String val = null;
            foreach (T i in _values)
            {
                if (val == null)
                    val = i.ToString();
                else
                    val += "\\" + i.ToString();
            }

            if (val == null)
                val = "";

            return val;
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

        public abstract override Object Values { get; set; }
        public abstract override AbstractAttribute Copy();
        internal abstract override AbstractAttribute Copy(bool copyBinary);

        #endregion
    }
    #endregion

    #region AttributeAT
    public class AttributeAT : AttributeBinary<uint>
    {

        #region Constructors

        public AttributeAT(uint tag)
            : base(tag)
        {
        }

        public AttributeAT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ATvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeAT(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeAT(AttributeAT attrib)
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

        public override uint GetUInt32(int i)
        {
            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeAT(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeAT(this);
        }

        #endregion
    }
    #endregion

    #region AttributeFD
    public class AttributeFD : AttributeBinary<double>
    {
        #region Constructors

        public AttributeFD(uint tag)
            : base(tag)
        {
        }

        public AttributeFD(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FDvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeFD(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeFD(AttributeFD attrib)
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

        public override float GetFloat32(int i)
        {
            return (float)_values[i];
        }
        public override double GetFloat64(int i)
        {
            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeFD(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeFD(this);
        }

        #endregion
    }
    #endregion

    #region AttributeFL
    public class AttributeFL : AttributeBinary<float>
    {
        #region Constructors

        public AttributeFL(uint tag)
            : base(tag)
        {
        }

        public AttributeFL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeFL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeFL(AttributeFL attrib)
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

        public override float GetFloat32(int i)
        {
            return _values[i];
        }
        public override double GetFloat64(int i)
        {
            return (double)_values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeFL(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeFL(this);
        }
        #endregion
    }
    #endregion

    #region AttributeOB
    public class AttributeOB : AttributeBinary<byte>
    {
        #region Protected Members
        protected List<uint> _table;
        protected List<ByteBuffer> _fragments = new List<ByteBuffer>();
        #endregion

        #region Constructors

        public AttributeOB(uint tag)
            : base(tag)
        {

        }

        public AttributeOB(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OBvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeOB(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeOB(AttributeOB attrib)
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

        public override AbstractAttribute Copy()
        {
            return new AttributeOB(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOB(this);
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

    #region AttributeOF
    public class AttributeOF : AttributeBinary<float>
    {
        public AttributeOF(uint tag)
            : base(tag)
        {

        }

        public AttributeOF(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OFvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeOF(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributeOF(AttributeOF attrib)
            : base(attrib)
        {
        }


        #region Abstract Method Implementation

        public override string ToString()
        {
            return base.Tag; // TODO
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

        public override AbstractAttribute Copy()
        {
            return new AttributeOF(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOF(this);
        }

        #endregion
    }
    #endregion

    #region AttributeOW
    public class AttributeOW : AttributeBinary<ushort>
    {
        public AttributeOW(uint tag)
            : base(tag)
        {
        }

        public AttributeOW(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OWvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal AttributeOW(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal AttributeOW(AttributeOW attrib)
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

        public override AbstractAttribute Copy()
        {
            return new AttributeOW(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeOW(this);
        }

        #endregion


    }
    #endregion

    #region AttributeSL
    public class AttributeSL : AttributeBinary<int>
    {
        public AttributeSL(uint tag)
            : base(tag)
        {

        }

        public AttributeSL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeSL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeSL(AttributeSL attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        public override int GetInt32(int i)
        {
            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeSL(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeSL(this);
        }

        #endregion
    }
    #endregion

    #region AttributeSS
    public class AttributeSS : AttributeBinary<short>
    {
        #region Constructors

        public AttributeSS(uint tag)
            : base(tag)
        {

        }

        public AttributeSS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SSvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeSS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeSS(AttributeSS attrib)
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

        public override short GetInt16(int i)
        {
            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeSS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeSS(this);
        }

        #endregion
    }
    #endregion

    #region AttributeUL
    public class AttributeUL : AttributeBinary<uint>
    {
        #region Constructors

        public AttributeUL(uint tag)
            : base(tag)
        {

        }

        public AttributeUL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ULvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeUL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeUL(AttributeUL attrib)
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

        public override uint GetUInt32(int i)
        {
            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeUL(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUL(this);
        }

        #endregion
    }
    #endregion 

    #region AttributeUN
    public class AttributeUN : AttributeBinary<byte>
    {
        #region Constructors

        public AttributeUN(uint tag)
            : base(tag)
        {

        }

        public AttributeUN(DicomTag tag)
            : base(tag)
        {
        }

        internal AttributeUN(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            _values = item.ToBytes();

            SetStreamLength();
        }

        internal AttributeUN(AttributeUN attrib)
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

        public override AbstractAttribute Copy()
        {
            return new AttributeUN(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUN(this);
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

    #region AttributeUS
    public class AttributeUS : AttributeBinary<ushort>
    {
        #region Constructors

        public AttributeUS(uint tag)
            : base(tag)
        {

        }

        public AttributeUS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.USvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal AttributeUS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal AttributeUS(AttributeUS attrib)
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

        public override ushort GetUInt16(int i)
        {
            return _values[i];
        }

        public override ushort GetUInt16(int i, ushort defaultVal)
        {
            if ((_values == null) || (_values.Length == 0))
                return defaultVal;

            return _values[i];
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

        public override AbstractAttribute Copy()
        {
            return new AttributeUS(this);
        }

        internal override AbstractAttribute Copy(bool copyBinary)
        {
            return new AttributeUS(this);
        }

        #endregion
    }
    #endregion

}
