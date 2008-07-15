using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Converters;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ImageStreamingContext : StreamingContext
    {
        private DicomPixelData _pd;

        public string ImagePath
        {
            get
            {
                string path = Path.Combine(StorageLocation.GetStudyPath(), SeriesInstanceUid);
                path = Path.Combine(path, ObjectUid + ".dcm");
                return path;
            }
        }
        
        public DicomPixelData PixelData
        {
            get
            {
                if (_pd == null)
                {
                    _pd = DicomPixelData.CreateFrom(ImagePath);
                    
                }
                return _pd;
            }
        }

        public bool IsMultiFrame
        {
            get
            {
                return PixelData.NumberOfFrames > 1;
            }
        }
    }
}
