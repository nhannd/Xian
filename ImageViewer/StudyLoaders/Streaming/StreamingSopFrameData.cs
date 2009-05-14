#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal interface IStreamingSopFrameData : ISopFrameData
	{
		bool PixelDataRetrieved { get; }
		void RetrievePixelData();
	}

	internal partial class StreamingSopDataSource
	{
		private class StreamingSopFrameData : StandardSopFrameData, IStreamingSopFrameData
		{
			private readonly FramePixelData _framePixelData;
			private readonly byte[][] _overlayData;

			public StreamingSopFrameData(int frameNumber, StreamingSopDataSource parent) : base(frameNumber, parent)
			{
				_framePixelData = new FramePixelData(this.Parent, frameNumber);
				_overlayData = new byte[16][];
			}

			public new StreamingSopDataSource Parent
			{
				get { return (StreamingSopDataSource) base.Parent; }
			}

			public bool PixelDataRetrieved
			{
				get { return _framePixelData.AlreadyRetrieved; }
			}
			public void RetrievePixelData()
			{
				_framePixelData.Retrieve();
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				byte[] pixelData = _framePixelData.GetUncompressedPixelData();

				string photometricInterpretationCode = this.Parent[DicomTags.PhotometricInterpretation].ToString();
				PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);

				TransferSyntax ts = TransferSyntax.GetTransferSyntax(this.Parent.TransferSyntaxUid);
				if (pi.IsColor)
				{
					if (ts == TransferSyntax.Jpeg2000ImageCompression ||
					    ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
					    ts == TransferSyntax.JpegExtendedProcess24 ||
					    ts == TransferSyntax.JpegBaselineProcess1 ||
					    ts == TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1)
						pi = PhotometricInterpretation.Rgb;

					pixelData = ToArgb(this.Parent, pixelData, pi);
				}
				else
				{
					OverlayPlaneModuleIod opmi = new OverlayPlaneModuleIod(this.Parent);
					foreach (OverlayPlane overlayPlane in opmi)
					{
						if (overlayPlane.IsEmbedded && _overlayData[overlayPlane.Index] == null)
						{
							byte[] overlayData = OverlayData.Extract(overlayPlane.OverlayBitPosition, overlayPlane.OverlayBitsAllocated, false, pixelData);
							_overlayData[overlayPlane.Index] = overlayData;
						}
					}

					NormalizeGrayscalePixels(this.Parent, pixelData, ts.Endian);
				}

				return pixelData;
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				int frameIndex = overlayFrameNumber - 1;
				int overlayIndex = overlayGroupNumber - 1;

				byte[] overlayData = null;

				OverlayPlaneModuleIod opmi = new OverlayPlaneModuleIod(this.Parent);
				if (opmi.HasOverlayPlane(overlayIndex))
				{
					OverlayPlane overlayPlane = opmi[overlayIndex];

					if (_overlayData[overlayIndex] == null)
					{
						if (overlayPlane.IsEmbedded)
						{
							this.GetNormalizedPixelData();
						}
						else
						{
							OverlayData od = new OverlayData(overlayPlane.TryComputeOverlayDataBitOffset(frameIndex),
							                                 overlayPlane.OverlayRows,
							                                 overlayPlane.OverlayColumns,
							                                 overlayPlane.IsBigEndianOW,
							                                 overlayPlane.OverlayData);
							_overlayData[overlayIndex] = od.Unpack();
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
		}

		private class FramePixelDataRetriever
		{
			public readonly string StudyInstanceUid;
			public readonly string SeriesInstanceUid;
			public readonly string SopInstanceUid;
			public readonly int FrameNumber;
			public readonly string TransferSyntaxUid;
			public readonly string AETitle;
			public readonly Uri BaseUrl;

			public FramePixelDataRetriever(FramePixelData source)
			{
				string host = source.Parent._host;
				string wadoPrefix = source.Parent._wadoUriPrefix;
				int wadoPort = source.Parent._wadoServicePort;
				BaseUrl = new Uri(String.Format(wadoPrefix, host, wadoPort));

				AETitle = source.Parent._aeTitle;

				StudyInstanceUid = source.Parent.StudyInstanceUid;
				SeriesInstanceUid = source.Parent.SeriesInstanceUid;
				SopInstanceUid = source.Parent.SopInstanceUid;
				FrameNumber = source.FrameNumber;
				TransferSyntaxUid = source.Parent.TransferSyntaxUid;
			}

			public RetrievePixelDataResult Retrieve()
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				StreamingClient client = new StreamingClient(BaseUrl);
				RetrievePixelDataResult result =
					client.RetrievePixelData(AETitle, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, FrameNumber - 1);

				clock.Stop();

				string message = String.Format("[Retrieve Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Bytes transferred: {3}, Elapsed (s): {4}",
											   SopInstanceUid, FrameNumber, TransferSyntaxUid,
											   result.MetaData.ContentLength, clock.Seconds);

				Platform.Log(LogLevel.Debug, message);

				return result;
			}
		}
		
		private class FramePixelData
		{
			private readonly object _syncLock = new object();
			private volatile bool _alreadyRetrieved;
			private RetrievePixelDataResult _retrieveResult;

			public readonly StreamingSopDataSource Parent;
			public readonly int FrameNumber;

			public FramePixelData(StreamingSopDataSource parent, int frameNumber)
			{
				Parent = parent;
				FrameNumber = frameNumber;
			}

			public bool AlreadyRetrieved
			{
				get { return _alreadyRetrieved; }
			}

			public void Retrieve()
			{
				if (!_alreadyRetrieved)
				{
					//construct this object before the lock so there's no chance of deadlocking
					//with the parent data source (because we are accessing it's tags at the 
					//same time as it's trying to get the pixel data).
					FramePixelDataRetriever retriever = new FramePixelDataRetriever(this);

					lock (_syncLock)
					{
						if (!_alreadyRetrieved)
						{
							_retrieveResult = retriever.Retrieve();
							_alreadyRetrieved = true;
						}
					}
				}
			}

			public byte[] GetUncompressedPixelData()
			{
				//construct this object before the lock so there's no chance of deadlocking
				//with the parent data source (because we are accessing it's tags at the 
				//same time as it's trying to get the pixel data).
				FramePixelDataRetriever retriever = new FramePixelDataRetriever(this);

				lock (_syncLock)
				{
					RetrievePixelDataResult result;
					if (_retrieveResult == null)
						result = retriever.Retrieve();
					else
						result = _retrieveResult;

					//free this memory up in case it's holding a compressed buffer.
					_retrieveResult = null;

					CodeClock clock = new CodeClock();
					clock.Start();

					//synchronize the call to decompress; it's really already synchronized by
					//the parent b/c it's only called from CreateFrameNormalizedPixelData, but it doesn't hurt.
					byte[] pixelData = result.GetPixelData();

					clock.Stop();

					string message = String.Format("[Decompress Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Uncompressed bytes: {3}, Elapsed (s): {4}",
												   retriever.SopInstanceUid, FrameNumber, retriever.TransferSyntaxUid,
					                               pixelData.Length, clock.Seconds);

					Platform.Log(LogLevel.Debug, message);

					return pixelData;
				}
			}

			public void Unload()
			{
				lock (_syncLock)
				{
					_alreadyRetrieved = false;
					_retrieveResult = null;
				}
			}
		}
	}
}