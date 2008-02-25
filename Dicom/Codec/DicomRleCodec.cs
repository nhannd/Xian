using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Codec
{
    public class DicomRleCodecParameters : DicomCodecParameters
    {
        #region Private Members
        bool _reverseByteOrder;
        #endregion

        #region Public Members
        public DicomRleCodecParameters()
        {
            if (ByteBuffer.LocalMachineEndian == Endian.Little)
                _reverseByteOrder = false;
            else
                _reverseByteOrder = true;
        }

        public DicomRleCodecParameters(bool reverseByteOrder)
        {
            _reverseByteOrder = reverseByteOrder;
        }
        #endregion

        #region Public Properties
        public bool ReverseByteOrder
        {
            get { return _reverseByteOrder; }
            set { _reverseByteOrder = value; }
        }
        #endregion
    }

    public class DicomRleCodec : IDicomCodec
    {
        public string Name
        {
            get { return "RLE Lossless"; }
        }

        public TransferSyntax TransferSyntax
        {
            get { return TransferSyntax.RleLossless; }
        }

        public DicomCodecParameters GetDefaultParameters()
        {
            return new DicomRleCodecParameters();
        }

        #region Encode
        private class RLEEncoder
        {
            #region Private Members
            private int _count;
            private readonly uint[] _offsets;
            private readonly MemoryStream _stream;
            private readonly BinaryWriter _writer;
            private byte[] _buffer;

            private int _prevByte;
            private int _repeatCount;
            private int _bufferPos;
            #endregion

            #region Public Constructors
            public RLEEncoder()
            {
                _count = 0;
                _offsets = new uint[15];
                _stream = new MemoryStream();
                _writer = EndianBinaryWriter.Create(_stream, Endian.Little);
                _buffer = new byte[132];
                WriteHeader();

                _prevByte = -1;
                _repeatCount = 0;
                _bufferPos = 0;
            }
            #endregion

            #region Public Members
            public int NumberOfSegments
            {
                get { return _count; }
            }

            public long Length
            {
                get { return _stream.Length; }
            }

            public byte[] GetBuffer()
            {
                Flush();
                WriteHeader();
                return _stream.ToArray();
            }

            public void NextSegment()
            {
                Flush();
                if ((Length & 1) == 1)
                    _stream.WriteByte(0x00);
                _offsets[_count++] = (uint)_stream.Length;
            }

            public void Encode(byte b)
            {
                if (b == _prevByte)
                {
                    _repeatCount++;

                    if (_repeatCount > 2 && _bufferPos > 0)
                    {
                        // We're starting a run, flush out the buffer
                        while (_bufferPos > 0)
                        {
                            int count = Math.Min(128, _bufferPos);
                            _stream.WriteByte((byte)(count - 1));
                            MoveBuffer(count);
                        }
                    }
                    else if (_repeatCount > 128)
                    {
                        int count = Math.Min(_repeatCount, 128);
                        _stream.WriteByte((byte)(257 - count));
                        _stream.WriteByte((byte)_prevByte);
                        _repeatCount -= count;
                    }
                }
                else
                {
                    switch (_repeatCount)
                    {
                        case 0:
                            break;
                        case 1:
                            {
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                break;
                            }
                        case 2:
                            {
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                _buffer[_bufferPos++] = (byte)_prevByte;
                                break;
                            }
                        default:
                            {
                                while (_repeatCount > 0)
                                {
                                    int count = Math.Min(_repeatCount, 128);
                                    _stream.WriteByte((byte)(257 - count));
                                    _stream.WriteByte((byte)_prevByte);
                                    _repeatCount -= count;
                                }

                                break;
                            }
                    }

                    while (_bufferPos > 128)
                    {
                        int count = Math.Min(128, _bufferPos);
                        _stream.WriteByte((byte)(count - 1));
                        MoveBuffer(count);
                    }

                    _prevByte = b;
                    _repeatCount = 1;
                }
            }

            public void MakeEvenLength()
            {
                // Make even length
                if (_stream.Length%2 == 1)
                    _stream.WriteByte(0);
            }

            public void Flush()
            {
                if (_repeatCount < 2)
                {
                    while (_repeatCount > 0)
                    {
                        _buffer[_bufferPos++] = (byte)_prevByte;
                        _repeatCount--;
                    }
                }

                while (_bufferPos > 0)
                {
                    int count = Math.Min(128, _bufferPos);
                    _stream.WriteByte((byte)(count - 1));
                    MoveBuffer(count);
                }

                if (_repeatCount >= 2)
                {
                    while (_repeatCount > 0)
                    {
                        int count = Math.Min(_repeatCount, 128);
                        _stream.WriteByte((byte)(257 - count));
                        _stream.WriteByte((byte)_prevByte);
                        _repeatCount -= count;
                    }
                }

                _prevByte = -1;
                _repeatCount = 0;
                _bufferPos = 0;
            }
            #endregion

            #region Private Members
            private void MoveBuffer(int count)
            {
                _stream.Write(_buffer, 0, count);
                for (int i = count, n = 0; i < _bufferPos; i++, n++)
                {
                    _buffer[n] = _buffer[i];
                }
                _bufferPos = _bufferPos - count;
            }

            private void WriteHeader()
            {
                _stream.Seek(0, SeekOrigin.Begin);
                _writer.Write((uint)_count);
                for (int i = 0; i < 15; i++)
                {
                    _writer.Write(_offsets[i]);
                }
            }
            #endregion
        }

        public void Encode(DicomAttributeCollection dataset, DicomUncompressedPixelData oldPixelData, DicomCompressedPixelData newPixelData, DicomCodecParameters parameters)
        {
            DicomRleCodecParameters rleParams = parameters as DicomRleCodecParameters;

            if (rleParams == null)
                rleParams = GetDefaultParameters() as DicomRleCodecParameters;

            int pixelCount = oldPixelData.ImageWidth * oldPixelData.ImageHeight;
            int numberOfSegments = oldPixelData.BytesAllocated * oldPixelData.SamplesPerPixel;

            for (int i = 0; i < oldPixelData.NumberOfFrames; i++)
            {
                RLEEncoder encoder = new RLEEncoder();
                byte[] frameData = oldPixelData.GetFrame(i);

                for (int s = 0; s < numberOfSegments; s++)
                {
                    encoder.NextSegment();

                    int sample = s / oldPixelData.BytesAllocated;
                    int sabyte = s % oldPixelData.BytesAllocated;

                    int pos;
                    int offset;

                    if (newPixelData.PlanarConfiguration == 0)
                    {
                        pos = sample * oldPixelData.BytesAllocated;
                        offset = numberOfSegments;
                    }
                    else
                    {
                        pos = sample * oldPixelData.BytesAllocated * pixelCount;
                        offset = oldPixelData.BytesAllocated;
                    }

                    if (rleParams.ReverseByteOrder)
                        pos += sabyte;
                    else
                        pos += oldPixelData.BytesAllocated - sabyte - 1;

                    for (int p = 0; p < pixelCount; p++)
                    {
                        if (pos >= frameData.Length)
                            throw new DicomCodecException("");
                        encoder.Encode(frameData[pos]);
                        pos += offset;
                    }
                    encoder.Flush();
                }

                encoder.MakeEvenLength();

                newPixelData.AddFrameFragment(encoder.GetBuffer());
            }
        }
        #endregion

        #region Decode
        private class RLEDecoder
        {
            #region Private Members
            private int _count;
            private int[] _offsets;
            private byte[] _data;
            #endregion

            #region Public Constructors
            public RLEDecoder(IList<DicomFragment> data)
            {
                uint size = 0;
                foreach (DicomFragment frag in data)
                    size += frag.Length;
                MemoryStream stream = new MemoryStream(data[0].GetByteArray());
                for (int i = 1; i < data.Count; i++)
                {
                    stream.Seek(0, SeekOrigin.End);
                    byte[] ba = data[i].GetByteArray();
                    stream.Write(ba,0,ba.Length);
                }
                BinaryReader reader = EndianBinaryReader.Create(stream, Endian.Little);
                _count = (int)reader.ReadUInt32();
                _offsets = new int[15];
                for (int i = 0; i < 15; i++)
                {
                    _offsets[i] = reader.ReadInt32();
                }
                _data = new byte[stream.Length-64]; // take off 64 bytes for the offsets
                stream.Read(_data,0,_data.Length);
            }
            #endregion

            #region Public Members
            public int NumberOfSegments
            {
                get { return _count; }
            }

            public void DecodeSegment(int segment, byte[] buffer)
            {
                if (segment < 0 || segment >= _count)
                    throw new IndexOutOfRangeException("Segment number out of range");

                int offset = GetSegmentOffset(segment);
                int length = GetSegmentLength(segment);

                Decode(buffer, _data, offset, length);
            }

            private static void Decode(byte[] buffer, byte[] rleData, int offset, int count)
            {
                int pos = 0;
                int end = offset + count;
                for (int i = offset; i < end; )
                {
                    int n = rleData[i++];
                    if ((n & 0x80) != 0)
                    {
                        int c = 257 - n;
                        byte b = rleData[i++];
                        while (c-- > 0)
                        {
                            buffer[pos++] = b;
                            if (pos > buffer.Length)
                            {
                                DicomLogger.LogError("RLE segment unexpectedly too long.  Ignoring data.");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (n == 0 && i == end) // Single padding char
                            return;
                        int c = (n & 0x7F) + 1;
                        if ((i + c) >= end)
                        {
                            c = offset + count - i;
                        }
                        if (i > rleData.Length || pos + c > buffer.Length)
                        {
                            DicomLogger.LogError("Invalid formatted RLE data.  RLE segment unexpectedly too long.");
                            return;
                        }
                    
                    Buffer.BlockCopy(rleData, i, buffer, pos, c);
                        pos += c;
                        i += c;
                    }
                }
            }
            #endregion

            #region Private Members
            private int GetSegmentOffset(int segment)
            {
                return _offsets[segment] - 64;
            }

            private int GetSegmentLength(int segment)
            {
                int offset = GetSegmentOffset(segment);
                if (segment < (_count - 1))
                {
                    int next = GetSegmentOffset(segment + 1);
                    return next - offset;
                }
                else
                {
                    return _data.Length - offset;
                }
            }
            #endregion
        }

        public void Decode(DicomAttributeCollection dataset, DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
        {
            DicomRleCodecParameters rleParams = parameters as DicomRleCodecParameters;

            if (rleParams == null)
                rleParams = GetDefaultParameters() as DicomRleCodecParameters;

            int pixelCount = oldPixelData.ImageWidth * oldPixelData.ImageHeight;
            int numberOfSegments = oldPixelData.BytesAllocated * oldPixelData.SamplesPerPixel;
            int segmentLength = (pixelCount & 1) == 1 ? pixelCount + 1 : pixelCount;

            byte[] segment = new byte[segmentLength];
            byte[] frameData = new byte[oldPixelData.UncompressedFrameSize];

            for (int i = 0; i < oldPixelData.NumberOfFrames; i++)
            {
                IList<DicomFragment> rleData = oldPixelData.GetFrameFragments(i);
                RLEDecoder decoder = new RLEDecoder(rleData);

                if (decoder.NumberOfSegments != numberOfSegments)
                    throw new DicomCodecException("Unexpected number of RLE segments!");

                for (int s = 0; s < numberOfSegments; s++)
                {
                    decoder.DecodeSegment(s, segment);

                    int sample = s / oldPixelData.BytesAllocated;
                    int sabyte = s % oldPixelData.BytesAllocated;

                    int pos;
                    int offset;

                    if (newPixelData.PlanarConfiguration == 0)
                    {
                        pos = sample * oldPixelData.BytesAllocated;
                        offset = oldPixelData.SamplesPerPixel * oldPixelData.BytesAllocated;
                    }
                    else
                    {
                        pos = sample * oldPixelData.BytesAllocated * pixelCount;
                        offset = oldPixelData.BytesAllocated;
                    }

                    if (rleParams.ReverseByteOrder)
                        pos += sabyte;
                    else
                        pos += oldPixelData.BytesAllocated - sabyte - 1;

                    for (int p = 0; p < pixelCount; p++)
                    {
                        frameData[pos] = segment[p];
                        pos += offset;
                    }
                }

                newPixelData.AppendFrame(frameData);
            }
        }
        #endregion
    }
}
