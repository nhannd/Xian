#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class PixelDataLoader
    {
        private readonly ImageStreamingContext _context;

        internal PixelDataLoader(ImageStreamingContext context)
        {
            _context = context;
        }

        public byte[] ReadFrame(int frame)
        {
            DicomPixelData pd = _context.PixelData;
            if (pd is DicomCompressedPixelData)
            {
                byte[] buffer = (pd as DicomCompressedPixelData).GetFrameFragmentData(frame);
                return buffer;
            }
            else
            {
                return pd.GetFrame(frame);
            }

        }

        public byte[] ReadUncompressedFrame(int frame)
        {
            return _context.PixelData.GetFrame(frame);
        }

    }
}