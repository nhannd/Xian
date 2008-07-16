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
                    object cached = DicomPixelDataCache.Find(StorageLocation, StudyInstanceUid, SeriesInstanceUid, ObjectUid);
                    if (cached!=null)
                    {
                        _pd = cached as DicomPixelData;
                    }
                    else
                    {
                        _pd = DicomPixelData.CreateFrom(ImagePath);
                        DicomPixelDataCache.Insert(StorageLocation, StudyInstanceUid, SeriesInstanceUid, ObjectUid, _pd);
                    }
                    
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
