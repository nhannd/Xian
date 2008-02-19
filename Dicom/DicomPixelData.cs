#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class representing pixel data.
    /// </summary>
    public abstract class DicomPixelData
    {
        #region Private Members
        private int _frames = 1;
        private ushort _width;
        private ushort _height;
        private ushort _highBit;
        private ushort _bitsStored;
        private ushort _bitsAllocated;
        private ushort _samplesPerPixel = 1;
        private ushort _pixelRepresentation;
        private ushort _planarConfiguration;
        private string _photometricInterpretation;
        #endregion

        #region Constructors
        public DicomPixelData(DicomAttributeCollection collection)
        {
            collection.LoadDicomFields(this);
        }

        internal DicomPixelData(DicomPixelData attrib)
        {
            this.NumberOfFrames = attrib.NumberOfFrames;
            this.ImageWidth = attrib.ImageWidth;
            this.ImageHeight = attrib.ImageHeight;
            this.HighBit = attrib.HighBit;
            this.BitsStored = attrib.BitsStored;
            this.BitsAllocated = attrib.BitsAllocated;
            this.SamplesPerPixel = attrib.SamplesPerPixel;
            this.PixelRepresentation = attrib.PixelRepresentation;
            this.PlanarConfiguration = attrib.PlanarConfiguration;
            this.PhotometricInterpretation = attrib.PhotometricInterpretation;
        }

        #endregion

        #region Public Properties
        [DicomField(DicomTags.NumberOfFrames, DefaultValue = DicomFieldDefault.Default)]
        public int NumberOfFrames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        [DicomField(DicomTags.Columns, DefaultValue = DicomFieldDefault.Default)]
        public ushort ImageWidth
        {
            get { return _width; }
            set { _width = value; }
        }

        [DicomField(DicomTags.Rows, DefaultValue = DicomFieldDefault.Default)]
        public ushort ImageHeight
        {
            get { return _height; }
            set { _height = value; }
        }

        [DicomField(DicomTags.HighBit, DefaultValue = DicomFieldDefault.Default)]
        public ushort HighBit
        {
            get { return _highBit; }
            set { _highBit = value; }
        }

        [DicomField(DicomTags.BitsStored, DefaultValue = DicomFieldDefault.Default)]
        public ushort BitsStored
        {
            get { return _bitsStored; }
            set { _bitsStored = value; }
        }

        [DicomField(DicomTags.BitsAllocated, DefaultValue = DicomFieldDefault.Default)]
        public ushort BitsAllocated
        {
            get { return _bitsAllocated; }
            set { _bitsAllocated = value; }
        }

        public int BytesAllocated
        {
            get
            {
                int bytes = BitsAllocated / 8;
                if ((BitsAllocated % 8) > 0)
                    bytes++;
                return bytes;
            }
        }

        [DicomField(DicomTags.SamplesPerPixel, DefaultValue = DicomFieldDefault.Default)]
        public ushort SamplesPerPixel
        {
            get { return _samplesPerPixel; }
            set { _samplesPerPixel = value; }
        }

        [DicomField(DicomTags.PixelRepresentation, DefaultValue = DicomFieldDefault.Default)]
        public ushort PixelRepresentation
        {
            get { return _pixelRepresentation; }
            set { _pixelRepresentation = value; }
        }

        public bool IsSigned
        {
            get { return _pixelRepresentation != 0; }
        }

        [DicomField(DicomTags.PlanarConfiguration, DefaultValue = DicomFieldDefault.Default)]
        public ushort PlanarConfiguration
        {
            get { return _planarConfiguration; }
            set { _planarConfiguration = value; }
        }

        public bool IsPlanar
        {
            get { return _planarConfiguration != 0; }
        }

        [DicomField(DicomTags.PhotometricInterpretation, DefaultValue = DicomFieldDefault.Null)]
        public string PhotometricInterpretation
        {
            get { return _photometricInterpretation; }
            set { _photometricInterpretation = value; }
        }

        public int UncompressedFrameSize
        {
            get
            {
                // ybr full 422 only stores 2/3 of the pixels
                if (_photometricInterpretation.Equals("YBR_FULL_422"))
                    return ImageWidth * ImageHeight * BytesAllocated * 2;

                return ImageWidth * ImageHeight * BytesAllocated * SamplesPerPixel;
            }
        }
        #endregion
    }


    /// <summary>
    /// A <see cref="DicomAttribute"/> representing uncompressed pixel data.
    /// </summary>
    public class DicomUncompressedPixelData : DicomPixelData
    {
        #region Private Members
        private readonly DicomAttribute _pd;
        #endregion

        #region Constructors
        internal DicomUncompressedPixelData(DicomAttributeCollection collection)
            : base(collection)
        {
            _pd = collection[DicomTags.PixelData];
        }
        #endregion

        #region Public Methods
        public byte[] GetFrame(int frame)
        {
            if (frame >= NumberOfFrames)
                throw new ArgumentOutOfRangeException("frame");

            DicomAttributeOB obAttrib = _pd as DicomAttributeOB;
            if (obAttrib != null)
            {
                if (obAttrib._reference != null)
                {
                    ByteBuffer bb;
                    using (FileStream fs = new FileStream(obAttrib._reference.Filename, FileMode.Open))
                    {
                        long offset = obAttrib._reference.Offset + frame*UncompressedFrameSize;
                        fs.Seek(offset, SeekOrigin.Begin);

                        bb = new ByteBuffer();
                        bb.CopyFrom(fs, UncompressedFrameSize);
                        bb.Endian = obAttrib._reference.Endian;
                        fs.Close();
                    }

                    return bb.ToBytes();
                }

                byte[] pixels = new byte[UncompressedFrameSize];

                Buffer.BlockCopy((byte[])obAttrib.Values, frame*UncompressedFrameSize, pixels, 0, UncompressedFrameSize);

                return pixels;
            }

            DicomAttributeOW owAttrib = _pd as DicomAttributeOW;
            if (owAttrib != null)
            {
                if (owAttrib._reference != null)
                {
                    ByteBuffer bb;
                    if ((UncompressedFrameSize % 2 == 1)
                        && (owAttrib._reference.Endian != ByteBuffer.LocalMachineEndian))
                    {
                        // When frames are odd size, and we need to byte swap, we need to get an extra byte.
                        // For odd number frames, we get a byte before the frame
                        // and for even frames we get a byte after the frame.

                        using (FileStream fs = new FileStream(owAttrib._reference.Filename, FileMode.Open))
                        {
                            if (frame%2 == 1)
                                fs.Seek((owAttrib._reference.Offset + frame*UncompressedFrameSize) - 1, SeekOrigin.Begin);
                            else
                                fs.Seek(owAttrib._reference.Offset + frame*UncompressedFrameSize, SeekOrigin.Begin);

                            bb = new ByteBuffer();
                            bb.CopyFrom(fs, UncompressedFrameSize + 1);
                            bb.Endian = owAttrib._reference.Endian;
                            fs.Close();
                        }

                        bb.Swap(owAttrib._reference.Vr.UnitSize);
                        bb.Endian = ByteBuffer.LocalMachineEndian;

                        if (frame%2 == 1)
                            return bb.ToBytes(1, UncompressedFrameSize);
                        return bb.ToBytes(0, UncompressedFrameSize);
                    }

                    using (FileStream fs = new FileStream(owAttrib._reference.Filename, FileMode.Open))
                    {
                        fs.Seek(owAttrib._reference.Offset + frame*UncompressedFrameSize, SeekOrigin.Begin);

                        bb = new ByteBuffer();
                        bb.CopyFrom(fs, UncompressedFrameSize);
                        bb.Endian = owAttrib._reference.Endian;
                        fs.Close();
                    }

                    if (owAttrib._reference.Endian != ByteBuffer.LocalMachineEndian)
                    {
                        bb.Swap(owAttrib._reference.Vr.UnitSize);
                        bb.Endian = ByteBuffer.LocalMachineEndian;
                    }

                    return bb.ToBytes();                    
                }
                byte[] pixels = new byte[UncompressedFrameSize];

                Buffer.BlockCopy((byte[])owAttrib.Values, frame * UncompressedFrameSize, pixels, 0, UncompressedFrameSize);

                return pixels;
            }

            return null;
        }
        #endregion
    }



    /// <summary>
    /// A <see cref="DicomAttribute"/> representing compressed pixel data.
    /// </summary>
    public class DicomCompressedPixelData : DicomPixelData
    {
        #region Protected Members
        protected List<uint> _table;
        protected List<DicomFragment> _fragments = new List<DicomFragment>();
        private DicomFragmentSequence _sq;
        #endregion

        #region Constructors
        public DicomCompressedPixelData(DicomAttributeCollection collection) : base(collection)
        {
            _sq = (DicomFragmentSequence)collection[DicomTags.PixelData];
        }


        #endregion

        #region Public Properties
        #endregion

        #region Public Methods
        #endregion

    }
}
