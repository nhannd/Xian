#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Specialized;
using System.IO;
using System.Net;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ImageStreamingContext : StreamingContext
    {
        private DicomPixelData _pd;
        private string _serverAE;
        private int _frameNumber;
        private readonly string _nextSeriesUid;
        private readonly string _nextSopUid;

        #region PERFORMANCE TESTING STUFF
        private static DicomPixelData _testCompressedImage;
        private static DicomPixelData _testUncompressedImage;
        private readonly bool testCompressed;
        private readonly bool testUncompressed;
        #endregion

        public ImageStreamingContext(HttpListenerContext context)
        {
            Request = context.Request;
            Response = context.Response;
            NameValueCollection query = Request.QueryString;

            #region INIT STUFF FOR PERFORMANCE TESTING
            #if DEBUG 

            if (query["testcompressed"] != null)
            {
                testCompressed= true;
            }
            else if (query["testuncompressed"] != null)
            {
                testUncompressed = true;
            }
            if (_testCompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.compressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testCompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }

            if (_testUncompressedImage == null)
            {
                using (Stream stream = typeof(ImageStreamingContext).Assembly.GetManifestResourceStream("ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Test.TestSamples.uncompressed.dcm"))
                {
                    DicomFile file = new DicomFile();
                    file.Load(stream);

                    _testUncompressedImage = DicomPixelData.CreateFrom(file);
                }
                
            }
            #endif

            #endregion

            _frameNumber = 0;
            if (query["FrameNumber"] != null)
                int.TryParse(query["FrameNumber"], out _frameNumber);

            _nextSeriesUid = query["nextSeriesUid"];
            _nextSopUid = query["nextObjectUid"];

        }
       
        
        public string ImagePath
        {
            get
            {
                return StorageLocation.GetSopInstancePath(SeriesInstanceUid, ObjectUid);
            }
        }
        
        public DicomPixelData PixelData
        {
            get
            {
                #region PERFORMANCE TESTING CODE
                #if DEBUG 
                // If requested, the test images will be streamed directly from memory
                if (testCompressed)
                {
                    return _testCompressedImage;
                }
                else if (testUncompressed)
                {
                    return _testUncompressedImage;
                } 
                #endif
                #endregion

                if (_pd == null)
                {
                    PixelDataManager manager = PixelDataManager.GetInstance(StorageLocation);
                    _pd = manager.GetPixelData(SeriesInstanceUid, ObjectUid, _nextSeriesUid, _nextSopUid);
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

        public string ServerAE
        {
            get { return _serverAE; }
            set { _serverAE = value; }
        }

        public int FrameNumber
        {
            get { return _frameNumber; }
            set { _frameNumber = value; }
        }

    }
}
