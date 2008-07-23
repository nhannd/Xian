using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class PixelDataLoader
    {
        private ImageStreamingContext _context;

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