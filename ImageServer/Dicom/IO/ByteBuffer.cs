/*
 * Taken from code Copyright (c) Colby Dillion, 2007
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.ImageServer.Dicom.IO
{
    public class ByteBuffer
    {
        #region Private Members
        private MemoryStream _ms;
        private byte[] _data;
        private BinaryReader _br;
        private BinaryWriter _bw;
        private Endian _endian;
        private Encoding _encoding;
        private String _specificCharacterSet;
        #endregion

        #region Public Constructors
        public ByteBuffer()
            : this(LocalMachineEndian)
        {
        }

        public ByteBuffer(Endian endian)
        {
            _endian = endian;
            _encoding = Encoding.ASCII;
            _specificCharacterSet = null;
        }

        public ByteBuffer(byte[] data)
            : this(data, LocalMachineEndian)
        {
        }

        public ByteBuffer(byte[] data, Endian endian)
        {
            _data = data;
            _endian = endian;
            _encoding = Encoding.ASCII;
            _specificCharacterSet = null;
        }
        #endregion

        #region Public Properties
        public static Endian LocalMachineEndian =
            BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

        public MemoryStream Stream
        {
            get
            {
                if (_ms == null)
                {
                    if (_data == null)
                    {
                        _ms = new MemoryStream();
                    }
                    else
                    {
                        _ms = new MemoryStream(_data);
                        _data = null;
                    }
                }
                return _ms;
            }
        }

        public BinaryReader Reader
        {
            get
            {
                if (_br == null)
                {
                    _br = EndianBinaryReader.Create(Stream, Endian);
                }
                return _br;
            }
        }

        public BinaryWriter Writer
        {
            get
            {
                if (_bw == null)
                {
                    _bw = EndianBinaryWriter.Create(Stream, Endian);
                }
                return _bw;
            }
        }

        public Endian Endian
        {
            get { return _endian; }
            set
            {
                _endian = value;
                _br = null;
                _bw = null;
            }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public String SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

        public int Length
        {
            get
            {
                if (_ms != null)
                    return (int)_ms.Length;
                if (_data != null)
                    return _data.Length;
                return 0;
            }
        }
        #endregion

        #region Public Functions
        public void Clear()
        {
            _ms = null;
            _br = null;
            _bw = null;
            _data = null;
        }

        public void Chop(int count)
        {
            int len = (int)Stream.Length;
            if (len <= count)
            {
                Stream.SetLength(0);
                return;
            }
            byte[] bytes = GetChunk(count, len - count);
            Stream.SetLength(0);
            Stream.Position = 0;
            Stream.Write(bytes, 0, bytes.Length);
        }

        public void Append(byte[] buffer, int offset, int count)
        {
            long pos = Stream.Position;
            Stream.Seek(0, SeekOrigin.End);
            Stream.Write(buffer, offset, count);
            Stream.Position = pos;
        }

        public int CopyFrom(Stream s, int count)
        {
            _ms = null;
            _br = null;
            _bw = null;

            int read = 0;
            _data = new byte[count];

            while (read < count)
            {
                int rd = s.Read(_data, read, count - read);
                if (rd == 0)
                    return read;
                read += rd;
            }

            return read;
        }

        public void CopyTo(Stream s)
        {
            if (_ms != null)
            {
                _ms.WriteTo(s);
                return;
            }
            if (_data != null)
            {
                s.Write(_data, 0, _data.Length);
            }
        }

        public void CopyTo(Stream s, int offset, int count)
        {
            byte[] bytes = new byte[count];
            Stream.Position = offset;
            Stream.Read(bytes, 0, count);
            s.Write(bytes, 0, count);
        }

        public void CopyTo(byte[] buffer, int offset, int count)
        {
            if (_ms != null)
            {
                Stream.Position = offset;
                Stream.Read(buffer, 0, count);
                return;
            }
            if (_data != null)
            {
                Buffer.BlockCopy(_data, offset, buffer, 0, count);
            }
        }

        public byte[] GetChunk(int offset, int count)
        {
            byte[] chunk = new byte[count];
            CopyTo(chunk, offset, count);
            return chunk;
        }

        public void FromBytes(byte[] bytes)
        {
            _data = bytes;
            _ms = null;
        }

        public byte[] ToBytes()
        {
            if (_ms != null)
            {
                _data = _ms.ToArray();
                _ms = null;
                _br = null;
                _bw = null;
            }
            if (_data == null)
            {
                return new byte[0];
            }
            return _data;
        }

        public byte[] ToBytes(int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(ToBytes(), offset, data, 0, count);
            return data;
        }

        public ushort[] ToUInt16s()
        {
            return ByteConverter.ToUInt16Array(ToBytes());
        }

        public short[] ToInt16s()
        {
            return ByteConverter.ToInt16Array(ToBytes());
        }

        public uint[] ToUInt32s()
        {
            return ByteConverter.ToUInt32Array(ToBytes());
        }
        public int[] ToInt32s()
        {
            return ByteConverter.ToInt32Array(ToBytes());
        }

        public float[] ToFloats()
        {
            return ByteConverter.ToFloatArray(ToBytes());
        }

        public double[] ToDoubles()
        {
            return ByteConverter.ToDoubleArray(ToBytes());
        }

        public string GetString()
        {
            if (_specificCharacterSet != null)
                return DicomImplementation.CharacterParser.Decode(ToBytes(),_specificCharacterSet);

            return _encoding.GetString(ToBytes());
        }

        public void SetString(string val)
        {
            if (_specificCharacterSet != null)
                _data = DicomImplementation.CharacterParser.Encode(val, _specificCharacterSet);
            else
                _data = _encoding.GetBytes(val);
            _ms = null;
        }

        public void SetString(string val, byte pad)
        {
            if (_specificCharacterSet != null)
            {
                _data = DicomImplementation.CharacterParser.Encode(val, _specificCharacterSet);
                if ((_data.Length & 1) == 1)
                {
                    byte[] rawBytes = new byte[_data.Length + 1];
                    rawBytes[_data.Length] = pad;

                    _data.CopyTo(rawBytes, 0);
                    _data = rawBytes;
                }
            }
            else
            {
                int count = _encoding.GetByteCount(val);
                if ((count & 1) == 1)
                    count++;

                byte[] bytes = new byte[count];
                if (_encoding.GetBytes(val, 0, val.Length, bytes, 0) < count)
                    bytes[count - 1] = pad;

                _data = bytes;
            }
            _ms = null;
        }

        public void Swap(int bytesToSwap)
        {
            if (bytesToSwap == 1)
                return;
            if (bytesToSwap == 2) { Swap2(); return; }
            if (bytesToSwap == 4) { Swap4(); return; }
            //if (bytesToSwap == 8) { Swap8(); return; }
            ToBytes();
            int l = Length - (Length % bytesToSwap);
            for (int i = 0; i < l; i += bytesToSwap)
            {
                Array.Reverse(_data, i, bytesToSwap);
            }
        }

        public void Swap2()
        {
            ToBytes();
            int l = Length - (Length % 2);
            for (int i = 0; i < l; i += 2)
            {
                byte b = _data[i + 1];
                _data[i + 1] = _data[i];
                _data[i] = b;
            }
        }
        public void Swap4()
        {
            ToBytes();
            int l = Length - (Length % 4);
            for (int i = 0; i < l; i += 4)
            {
                byte b = _data[i + 3];
                _data[i + 3] = _data[i];
                _data[i] = b;
                b = _data[i + 2];
                _data[i + 2] = _data[i + 1];
                _data[i + 1] = b;
            }
        }
        public void Swap8()
        {
            Swap(8);
        }
        #endregion
    }
}
