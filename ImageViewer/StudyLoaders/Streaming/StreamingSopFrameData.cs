#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using DataCache;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
    public class StreamingPerformanceInfo
    {
        public readonly DateTime EndTime;
        public readonly DateTime StartTime;
        public readonly long TotalBytesTransferred;

        internal StreamingPerformanceInfo(DateTime start, DateTime end, long totalBytesTransferred)
        {
            StartTime = start;
            EndTime = end;
            TotalBytesTransferred = totalBytesTransferred;
        }

        public TimeSpan ElapsedTime
        {
            get { return EndTime.Subtract(StartTime); }
        }
    }

    public interface IStreamingSopFrameData : ISopFrameData
    {
        StreamingPerformanceInfo LastRetrievePerformanceInfo { get; }
        void RetrievePixelData();
    }

    public partial class StreamingSopDataSource
    {
        #region Nested type: FramePixelData

        private class FramePixelData
        {
            public readonly FrameInfo MyFrameInfo;
            public readonly StreamingSopDataSource Parent;
            private volatile StreamingPerformanceInfo _lastRetrievePerformanceInfo;
            private RetrievePixelDataResult _retrieveResult;

            public FramePixelData(StreamingSopDataSource parent, FrameInfo myFrameInfo)
            {
                Parent = parent;
                MyFrameInfo = myFrameInfo;
            }

            public StreamingPerformanceInfo LastRetrievePerformanceInfo
            {
                get { return _lastRetrievePerformanceInfo; }
            }

            public void Unload()
            {
                _retrieveResult = null;
            }

            public void Fetch()
            {
                if (MyFrameInfo.Cached)
                    return;

                var fetcher = new FramePixelDataFetcher(this);
                fetcher.Fetch(ViewerFrameEnumerator.ConcurrentDequeuePolicy.BeginFetch, GetCallback, ViewerFrameEnumerator.ConcurrentDequeuePolicy.EndFetch, true);
            }

            private byte[] Decompress(byte[] inputBuffer, int inputBufferSize)
            {
                var resultMetaInfo = new FrameStreamingResultMetaData {ContentLength = inputBufferSize};
                var result = new RetrievePixelDataResult(
                    DicomPixelData.CreateCompressedPixelData(MyFrameInfo, inputBuffer, inputBufferSize), inputBufferSize,
                    resultMetaInfo);

                return result.GetPixelData();
            }

            private void GetCallback(Stream stream, int streamLength, bool isCompressed)
            {
                if (Frame.IsDiskCacheEnabled())
                {
                    Frame.Cache.Put(MyFrameInfo.CacheIdTopLevel,
                                    MyFrameInfo.CacheId,
                                    new PixelCacheItem
                                        {
                                            PixelStream = stream,
                                            Size = streamLength,
                                            IsCompressed = isCompressed
                                        });
                }
                else
                {
                    if (stream != null)
                    {
                        //store in memory
                        var buffer = new byte[streamLength];
                        int count;
                        int offset = 0;
                        while ((count = stream.Read(buffer, offset, 4096)) > 0)
                        {
                            offset += count;
                        }
                        var resultMetaInfo = new FrameStreamingResultMetaData {ContentLength = streamLength};
                        _retrieveResult = isCompressed
                                              ? new RetrievePixelDataResult(
                                                    DicomPixelData.CreateCompressedPixelData(MyFrameInfo, buffer,
                                                                                             streamLength), streamLength,
                                                    resultMetaInfo)
                                              : new RetrievePixelDataResult(buffer, resultMetaInfo);
                    }
                }
            }

            public byte[] GetUncompressedPixelData(GetContext context)
            {
                RetrievePixelDataResult result;
                if (_retrieveResult != null)
                {
                    result = _retrieveResult;
                    _retrieveResult = null;
                }
                else
                {
                    result = GetFromCache(context);
                    if (result == null)
                    {
                        var fetcher = new FramePixelDataFetcher(this);
                        fetcher.Fetch(ViewerFrameEnumerator.ConcurrentDequeuePolicy.BeginFetch, GetCallback, ViewerFrameEnumerator.ConcurrentDequeuePolicy.EndFetch, false);
                        result = GetFromCache(context);
                    }
                }

                return result.GetPixelData();
            }

            private RetrievePixelDataResult GetFromCache(GetContext context)
            {
                RetrievePixelDataResult result = null;
                if (Frame.Cache.IsDiskCacheEnabled)
                {
                    context.Decompressor = Decompress;
                    var item = Frame.Cache.Get(MyFrameInfo.CacheIdTopLevel, MyFrameInfo.CacheId, context);
                    if (item != null && item.PixelData != null)
                    {
                        var resultMetaInfo = new FrameStreamingResultMetaData {ContentLength = item.Size};
                        result = new RetrievePixelDataResult(item.PixelData, resultMetaInfo);
                    }
                }
                return result;
            }
        }

        #endregion

        #region Nested type: FramePixelDataRetriever

        private class FramePixelDataFetcher
        {
            private readonly string _sopInstanceUid;
            private readonly string _transferSyntaxUid;
            private readonly string _AeTitle;
            private readonly Uri _baseUrl;
            private readonly FrameInfo _frameInfo;
            private readonly string _seriesInstanceUid;
            private readonly string _studyInstanceUid;


            public FramePixelDataFetcher(FramePixelData source)
            {
                _frameInfo = source.MyFrameInfo;
                string host = source.Parent._host;
                string wadoPrefix = source.Parent._wadoUriPrefix;
                int wadoPort = source.Parent._wadoServicePort;

                try
                {
                    _baseUrl = new Uri(String.Format(wadoPrefix, host, wadoPort));
                }
                catch (FormatException ex)
                {
                    // this exception happens if the FormatWadoUriPrefix setting is invalid.
                    throw new Exception(SR.MessageStreamingClientConfigurationException, ex);
                }

                _AeTitle = source.Parent._aeTitle;

                _studyInstanceUid = source.Parent.StudyInstanceUid;
                _seriesInstanceUid = source.Parent.SeriesInstanceUid;
                _sopInstanceUid = source.Parent.SopInstanceUid;
                _transferSyntaxUid = source.Parent.TransferSyntaxUid;
            }

            public void Fetch(Action beginGetCallback,
                              Action<Stream, int, bool> postResponseCallback,
                              Action endGetCallback, bool asynch)
            {
                var streamer = new PixelStreamer(_baseUrl);
                try
                {
                    var args = new PixelStreamer.FetchInfo
                                   {
                                       ServerAE = _AeTitle,
                                       StudyInstanceUID = _studyInstanceUid,
                                       SeriesInstanceUID = _seriesInstanceUid,
                                       SopInstanceUid = _sopInstanceUid,
                                       TransferSyntaxUID = _transferSyntaxUid,
                                       FrameNumber = _frameInfo.FrameNumber - 1,
                                       CacheId = _frameInfo.CacheId,
                                       Asynch = asynch,
                                       BeginGetCallback = beginGetCallback,
                                       PostResponseCallback = postResponseCallback,
                                       EndGetCallback = endGetCallback
                                   };

                    streamer.FetchPixels(args);
                }
                catch (Exception ex)
                {
                    throw TranslateStreamingException(ex);
                }
            }
        }

        #endregion

        #region Nested type: StreamingSopFrameData

        private class StreamingSopFrameData : StandardSopFrameData, IStreamingSopFrameData
        {
            private readonly FramePixelData _framePixelData;
            private readonly byte[][] _overlayData;

            [ThreadStatic]
            private static DynamicBuffer _conversionBuffer;

            public StreamingSopFrameData(FrameInfo frameInfo, StreamingSopDataSource parent)
                : base(frameInfo, parent, RegenerationCost.High)
            {
                _framePixelData = new FramePixelData(Parent, frameInfo);
                _overlayData = new byte[16][];

                string photometricInterpretationCode = Parent[DicomTags.PhotometricInterpretation].ToString();
                PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);
                if (pi.IsColor)
                    FrameInfo.ArgBufferSize = GetArgbBBufferSize(Parent);
            }

            public new StreamingSopDataSource Parent
            {
                get { return (StreamingSopDataSource) base.Parent; }
            }

            #region IStreamingSopFrameData Members

            public StreamingPerformanceInfo LastRetrievePerformanceInfo
            {
                get { return _framePixelData.LastRetrievePerformanceInfo; }
            }

            public void RetrievePixelData()
            {
                _framePixelData.Fetch();
            }

            public override byte[] GetNormalizedPixelData()
            {
                return !Frame.Cache.IsDiskCacheEnabled ? base.GetNormalizedPixelData() : CreateNormalizedPixelData();
            }

            #endregion

            protected override byte[] CreateNormalizedPixelData()
            {
                var context = new GetContext
                                  {
                                      Decompressor = null,
                                      PostProcessor = PostProcess,
                                      ConversionBufferSize = FrameInfo.ArgBufferSize
                                  };
                return _framePixelData.GetUncompressedPixelData(context);
            }

            private byte[] PostProcess(byte[] deserializeBuff)
            {
                string photometricInterpretationCode = Parent[DicomTags.PhotometricInterpretation].ToString();
                PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);
                TransferSyntax ts = TransferSyntax.GetTransferSyntax(Parent.TransferSyntaxUid);
                if (pi.IsColor)
                {
                    if (ts == TransferSyntax.Jpeg2000ImageCompression ||
                        ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
                        ts == TransferSyntax.JpegExtendedProcess24 ||
                        ts == TransferSyntax.JpegBaselineProcess1)
                        pi = PhotometricInterpretation.Rgb;

                    var conversionBuff = GetConversionBuffer();
                    conversionBuff.Resize(FrameInfo.ArgBufferSize, false);
                    ToArgb(Parent, deserializeBuff, conversionBuff.Buffer, pi);
                    return conversionBuff.Buffer;
                }

                var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
                foreach (OverlayPlane overlayPlane in overlayPlaneModuleIod)
                {
                    if (!overlayPlane.HasOverlayData && _overlayData[overlayPlane.Index] == null)
                    {
                        // if the overlay is embedded in pixel data and we haven't cached it yet, extract it now before we normalize the frame pixel data
                        byte[] overlayData = OverlayData.UnpackFromPixelData(overlayPlane.OverlayBitPosition,
                                                                             Parent[DicomTags.BitsAllocated].GetInt32(
                                                                                 0, 0),
                                                                             false, deserializeBuff);
                        _overlayData[overlayPlane.Index] = overlayData;
                    }
                }
                NormalizeGrayscalePixels(Parent, deserializeBuff);
                return deserializeBuff;
            }

            /// <summary>
            /// Called by <see cref="StandardSopFrameData.GetNormalizedOverlayData"/> to create a new byte buffer containing normalized 
            /// overlay pixel data for a particular overlay plane.
            /// </summary>
            /// <remarks>
            /// See <see cref="StandardSopFrameData.GetNormalizedOverlayData"/> for details on the expected format of the byte buffer.
            /// </remarks>
            /// <param name="overlayNumber">The 1-based overlay plane number.</param>
            /// <returns>A new byte buffer containing the normalized overlay pixel data.</returns>
            protected override byte[] CreateNormalizedOverlayData(int overlayNumber)
            {
                int overlayIndex = overlayNumber - 1;

                byte[] overlayData = null;

                // check whether or not the overlay plane exists before attempting to ascertain
                // whether or not the overlay is embedded in the pixel data
                var overlayPlaneModuleIod = new OverlayPlaneModuleIod(Parent);
                if (overlayPlaneModuleIod.HasOverlayPlane(overlayIndex))
                {
                    if (_overlayData[overlayIndex] == null)
                    {
                        OverlayPlane overlayPlane = overlayPlaneModuleIod[overlayIndex];
                        if (!overlayPlane.HasOverlayData)
                        {
                            // if the overlay is embedded, trigger retrieval of pixel data which will populate the cache for us
                            GetNormalizedPixelData();
                        }
                        else
                        {
                            // try to compute the offset in the OverlayData bit stream where we can find the overlay frame that applies to this image frame
                            int overlayFrame;
                            int bitOffset;
                            if (
                                overlayPlane.TryGetRelevantOverlayFrame(FrameNumber, Parent.NumberOfFrames,
                                                                        out overlayFrame) &&
                                overlayPlane.TryComputeOverlayDataBitOffset(overlayFrame, out bitOffset))
                            {
                                // offset found - unpack only that overlay frame
                                var od = new OverlayData(bitOffset,
                                                         overlayPlane.OverlayRows,
                                                         overlayPlane.OverlayColumns,
                                                         overlayPlane.IsBigEndianOW,
                                                         overlayPlane.OverlayData);
                                _overlayData[overlayIndex] = od.Unpack();
                            }
                            else
                            {
                                // no relevant overlay frame found - i.e. the overlay for this image frame is blank
                                _overlayData[overlayIndex] = new byte[0];
                            }
                        }
                    }

                    overlayData = _overlayData[overlayIndex];
                }

                return overlayData;
            }

            protected override void OnUnloaded()
            {
                base.OnUnloaded();

                // dump pixel data retrieve results and stored overlays
                _framePixelData.Unload();
                _overlayData[0x0] = null;
                _overlayData[0x1] = null;
                _overlayData[0x2] = null;
                _overlayData[0x3] = null;
                _overlayData[0x4] = null;
                _overlayData[0x5] = null;
                _overlayData[0x6] = null;
                _overlayData[0x7] = null;
                _overlayData[0x8] = null;
                _overlayData[0x9] = null;
                _overlayData[0xA] = null;
                _overlayData[0xB] = null;
                _overlayData[0xC] = null;
                _overlayData[0xD] = null;
                _overlayData[0xE] = null;
                _overlayData[0xF] = null;
            }

            private static DynamicBuffer GetConversionBuffer()
            {
                return _conversionBuffer ?? (_conversionBuffer = new DynamicBuffer());
            }
        }

        #endregion
    }
}