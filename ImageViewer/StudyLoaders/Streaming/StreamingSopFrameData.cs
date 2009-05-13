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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal partial class StreamingSopDataSource
	{
		private class StreamingSopFrameData : StandardSopFrameData
		{
			private readonly FramePixelData _retrieveResults;
			private readonly byte[][] _overlayData;

			public StreamingSopFrameData(int frameNumber, StreamingSopDataSource parent) : base(frameNumber, parent)
			{
				_retrieveResults = new FramePixelData(this.Parent, frameNumber);
				_overlayData = new byte[16][];
			}

			public new StreamingSopDataSource Parent
			{
				get { return (StreamingSopDataSource) base.Parent; }
			}

			public FramePixelData RetrieveResult
			{
				get { return _retrieveResults; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				byte[] pixelData = _retrieveResults.GetUncompressedPixelData();

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
				_retrieveResults.Unload();
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

		private class FramePixelData
		{
			private readonly StreamingSopDataSource _parent;
			private readonly int _frameNumber;

			private readonly object _syncLock = new object();
			private volatile bool _alreadyRetrieved;
			private RetrievePixelDataResult _retrieveResult;

			public FramePixelData(StreamingSopDataSource parent, int frameNumber)
			{
				_parent = parent;
				_frameNumber = frameNumber;
			}

			private void RetrievePixelData(bool force, out RetrievePixelDataResult result)
			{
				//get this information before locking so there's no chance of deadlocking
				//with the parent data source (because we are accessing it's tags at the 
				//same time as it's trying to get the pixel data).
				string host = _parent._host;
				string aeTitle = _parent._aeTitle;
				string wadoPrefix = _parent._wadoUriPrefix;
				int wadoPort = _parent._wadoServicePort;

				Uri baseUri = new Uri(String.Format(wadoPrefix, host, wadoPort));
				StreamingClient client = new StreamingClient(baseUri);

				string studyInstanceUid = _parent.StudyInstanceUid;
				string seriesInstanceUid = _parent.SeriesInstanceUid;
				string sopInstanceUid = _parent.SopInstanceUid;
				string transferSyntaxUid = _parent.TransferSyntaxUid;

				lock (_syncLock)
				{
					if (!_alreadyRetrieved || force)
					{
						Stopwatch clock = new Stopwatch();
						clock.Start();

						_retrieveResult =
							client.RetrievePixelData(aeTitle, studyInstanceUid, seriesInstanceUid, sopInstanceUid, _frameNumber - 1);
						_alreadyRetrieved = true;

						clock.Stop();

						string message = String.Format("[Retrieve Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Bytes transferred: {3}, Elapsed (ms): {4}",
						                               sopInstanceUid, _frameNumber, transferSyntaxUid,
						                               _retrieveResult.MetaData.ContentLength, clock.Elapsed.TotalSeconds);

						Platform.Log(LogLevel.Debug, message);
					}

					result = _retrieveResult;
				}
			}

			public bool AlreadyRetrieved
			{
				get { return _alreadyRetrieved; }
			}

			public void RetrievePixelData(bool force)
			{
				RetrievePixelDataResult result;
				RetrievePixelData(force, out result);
			}

			public byte[] GetUncompressedPixelData()
			{
				RetrievePixelDataResult result;
				//important: do not put a lock around this method; it will deadlock with the parent synclock
				RetrievePixelData(false, out result);

				string sopInstanceUid = _parent.SopInstanceUid;
				string transferSyntaxUid = _parent.TransferSyntaxUid;

				lock (_syncLock)
				{
					//free this memory up - the parent already has the uncompressed data.
					_retrieveResult = null;

					Stopwatch clock = new Stopwatch();
					clock.Start();

					//synchronize the call to decompress; it's really already synchronized by
					//the parent b/c it's only called from CreateFrameNormalizedPixelData, but it doesn't hurt.
					byte[] pixelData = result.GetPixelData();

					clock.Stop();

					string message = String.Format("[Decompress Info] Sop/Frame: {0}/{1}, Transfer Syntax: {2}, Uncompressed bytes: {3}, Elapsed (ms): {4}",
					                               sopInstanceUid, _frameNumber, transferSyntaxUid,
					                               pixelData.Length, clock.Elapsed.TotalSeconds);

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