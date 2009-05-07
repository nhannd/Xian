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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using System.IO;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingSopDataSource : DicomMessageSopDataSource
	{
		private class FramePixelData
		{
			private readonly StreamingSopDataSource _parent;
			private readonly int _frameNumber;

			private readonly object _syncLock = new object();
			private volatile bool _alreadyRetrieved;
			private RetrievePixelDataResult _retrieveResult;
			private readonly byte[][] _overlayData;

			public FramePixelData(StreamingSopDataSource parent, int frameNumber)
			{
				_parent = parent;
				_frameNumber = frameNumber;
				_overlayData = new byte[16][];
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

			public byte[] GetOverlayData(int index)
			{
				return _overlayData[index];
			}

			public bool HasOverlayData(int index)
			{
				return _overlayData[index] != null;
			}

			public void SetOverlayData(int index, byte[] data)
			{
				_overlayData[index] = data;
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

		private readonly string _host;
		private readonly string _aeTitle;
		private readonly string _wadoUriPrefix;
		private readonly int _wadoServicePort;
		private volatile bool _fullHeaderRetrieved = false;

		private volatile FramePixelData[] _retrieveResults;

		public StreamingSopDataSource(InstanceXml instanceXml, string host, string aeTitle, string wadoUriPrefix, int wadoServicePort)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml.Collection))
		{
			//These don't get set properly for instance xml.
			DicomFile sourceFile = (DicomFile)SourceMessage;
			sourceFile.TransferSyntaxUid = instanceXml.TransferSyntax.UidString;
			sourceFile.MediaStorageSopInstanceUid = instanceXml.SopInstanceUid;
			sourceFile.MetaInfo[DicomTags.SopClassUid].SetString(0, instanceXml.SopClass.Uid);

			_host = host;
			_aeTitle = aeTitle;
			_wadoUriPrefix = wadoUriPrefix;
			_wadoServicePort = wadoServicePort;
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)((DicomFile)base.SourceMessage).DataSet; }
		}

		public override DicomAttribute GetDicomAttribute(DicomTag tag)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.GetDicomAttribute(tag);
			}
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.GetDicomAttribute(tag);
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag.TagValue))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
		}

		private FramePixelData[] RetrieveResults
		{
			get
			{
				if (_retrieveResults == null)
				{
					lock (SyncLock)
					{
						if (_retrieveResults == null)
						{
							_retrieveResults = new FramePixelData[NumberOfFrames];
							for (int i = 0; i < _retrieveResults.Length; ++i)
								_retrieveResults[i] = new FramePixelData(this, i + 1);
						}
					}
				}

				return _retrieveResults;
			}	
		}

		public void RetrievePixelData(int frameNumber)
		{
			RetrievePixelData(frameNumber, false);
		}

		public void RetrievePixelData(int frameNumber, bool force)
		{
			RetrieveResults[frameNumber - 1].RetrievePixelData(force);
		}

		internal bool IsFrameRetrieved(int frameNumber)
		{
			return RetrieveResults[frameNumber - 1].AlreadyRetrieved;
		}

		protected override byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			int frameIndex = frameNumber - 1;
			FramePixelData frameData = RetrieveResults[frameIndex];

			byte[] pixelData = frameData.GetUncompressedPixelData();
			
			string photometricInterpretationCode = this[DicomTags.PhotometricInterpretation].ToString();
			PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);

			TransferSyntax ts = TransferSyntax.GetTransferSyntax(this.TransferSyntaxUid);
			if (pi.IsColor)
			{

				if (ts == TransferSyntax.Jpeg2000ImageCompression ||
					ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
					ts == TransferSyntax.JpegExtendedProcess24 ||
					ts == TransferSyntax.JpegBaselineProcess1 ||
					ts == TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1)
					pi = PhotometricInterpretation.Rgb;

				pixelData = ToArgb(this, pixelData, pi);
			}
			else
			{
				OverlayPlaneModuleIod opmi = new OverlayPlaneModuleIod(this);
				foreach (OverlayPlane overlayPlane in opmi)
				{
					if(false && overlayPlane.IsEmbedded && !frameData.HasOverlayData(overlayPlane.Index))
					{
						byte[] overlayData = OverlayData.Extract(overlayPlane.OverlayBitPosition, this[DicomTags.BitsAllocated].GetInt32(0, 0), false, pixelData);
						frameData.SetOverlayData(overlayPlane.Index, overlayData);
					}
				}

				NormalizeGrayscalePixels(this, pixelData, ts);
			}

			return pixelData;
		}

		protected override byte[] CreateFrameNormalizedOverlayData(int overlayNumber, int frameNumber)
		{
			int frameIndex = frameNumber - 1;
			int overlayIndex = overlayNumber - 1;
			FramePixelData frameData = RetrieveResults[frameIndex];

			byte[] overlayData = null;

			OverlayPlaneModuleIod opmi = new OverlayPlaneModuleIod(this);
			if(opmi.HasOverlayPlane(overlayIndex))
			{
				OverlayPlane overlayPlane = opmi[overlayIndex];
				//DicomAttribute odx = this.GetDicomAttribute(overlayPlane.TagOffset + DicomTags.OverlayData);

				if(!frameData.HasOverlayData(overlayIndex))
				{
					if(overlayPlane.IsEmbedded)
					{
						//CreateFrameNormalizedPixelData(frameNumber);
					}
					else
					{
						OverlayData od = new OverlayData(overlayPlane.ComputeOverlayDataBitOffset(overlayIndex),
							overlayPlane.OverlayRows,
							overlayPlane.OverlayColumns,
							overlayPlane.IsBigEndianOW,
							overlayPlane.OverlayData);
						frameData.SetOverlayData(overlayIndex, od.Unpack());
					}
				}
				overlayData = frameData.GetOverlayData(overlayIndex);
			}

			return overlayData;
		}

		protected override void OnUnloadFrameData(int frameNumber)
		{
			this.RetrieveResults[frameNumber - 1].Unload();
		}

		private bool NeedFullHeader(uint tag)
		{
			if (_fullHeaderRetrieved)
				return false;

			if (CollectionUtils.Contains(AttributeCollection.ExcludedTags, 
				delegate(DicomTag dicomTag) { return dicomTag.TagValue == tag; }))
			{
				return true;
			}

			DicomAttribute attribute = base.GetDicomAttribute(tag);
			if (attribute is DicomAttributeSQ)
			{
				DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem) item).HasExcludedTags(true))
								return true;
						}
					}
				}
			}

			return false;
		}

		private void GetFullHeader()
		{
			if (!_fullHeaderRetrieved)
			{
				Uri uri = new Uri(String.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));
				StreamingClient client = new StreamingClient(uri);

				DicomFile imageHeader = new DicomFile();
				using (Stream imageHeaderStream = client.RetrieveImageHeader(_aeTitle, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid))
				{
					imageHeader.Load(imageHeaderStream);
					base.SourceMessage = imageHeader;
					_fullHeaderRetrieved = true;
				}
			}
		}
	}
}
