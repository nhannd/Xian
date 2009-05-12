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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class DicomMessageSopDataSource : StandardSopDataSource, IDicomMessageSopDataSource
	{
		private readonly DicomAttributeCollection _dummy;
		private DicomMessageBase _sourceMessage;
		private bool _loaded = false;
		private bool _loading = false;

		protected DicomMessageSopDataSource(DicomMessageBase sourceMessage)
		{
			_dummy = new DicomAttributeCollection();
			_sourceMessage = sourceMessage;
		}

		// TODO: not actually thread-safe because client code can use indexers on "SourceMessage",
		// triggering empty attributes to be inserted.
		public DicomMessageBase SourceMessage
		{
			get
			{
				lock (SyncLock)
				{
					Load();
					return _sourceMessage;
				}
			}
			protected set
			{
				lock (SyncLock)
				{
					_sourceMessage = value;
				}
			}
		}

		//TODO: push up?
		protected virtual void EnsureLoaded()
		{
		}

		//TODO: is there a better way to do this?
		private void Load()
		{
			lock(SyncLock)
			{
				if (_loaded || _loading)
					return;

				try
				{
					_loading = true;
					EnsureLoaded();
					_loaded = true;
				}
				finally
				{
					_loading = false;
				}
			}
		}

		public override DicomAttribute GetDicomAttribute(DicomTag tag)
		{
			lock (SyncLock)
			{
				Load();

				DicomAttribute attribute;
				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return attribute;

				if (_sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _dummy[tag];
			}
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			lock (SyncLock)
			{
				Load();

				DicomAttribute attribute;
				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return attribute;

				if (_sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute))
					return attribute;

				return _dummy[tag];
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				Load();

				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return true;

				return _sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				Load();

				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return true;

				return _sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute);
			}
		}

		#region Frame Data Handling

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new DicomMessageSopFrameData(frameNumber, this);
		}

		private class OverlayDataCache
		{
			private readonly Dictionary<int, byte[]> _data = new Dictionary<int, byte[]>();
			
			public byte[] this[int overlayIndex, int overlayFrame]
			{
				get
				{
					int key = (overlayFrame << 8) | (overlayIndex & 0x000000ff);
					if (!_data.ContainsKey(key))
						return null;
					return _data[key];
				}
				set
				{
					int key = (overlayFrame << 8) | (overlayIndex & 0x000000ff);
					if (!_data.ContainsKey(key))
						_data.Add(key, null);
					_data[key] = value;
				}
			}

			public void Clear()
			{
				_data.Clear();
			}
		}

		protected class DicomMessageSopFrameData : StandardSopFrameData
		{
			private readonly OverlayDataCache _overlayCache = new OverlayDataCache();
			private readonly int _frameIndex;

			public DicomMessageSopFrameData(int frameNumber, DicomMessageSopDataSource parent)
				: base(frameNumber, parent)
			{
				_frameIndex = frameNumber - 1;
			}

			public new DicomMessageSopDataSource Parent
			{
				get { return (DicomMessageSopDataSource) base.Parent; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				DicomMessageBase _message = this.Parent.SourceMessage;

				CodeClock clock = new CodeClock();
				clock.Start();

				PhotometricInterpretation photometricInterpretation;
				byte[] rawPixelData;

				if (!_message.TransferSyntax.Encapsulated)
				{
					DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(_message);
					// DICOM library uses zero-based frame numbers
					rawPixelData = pixelData.GetFrame(_frameIndex);

					ExtractOverlayFrames(rawPixelData, pixelData.BitsAllocated);

					photometricInterpretation = PhotometricInterpretation.FromCodeString(_message.DataSet[DicomTags.PhotometricInterpretation]);
				}
				else if (DicomCodecRegistry.GetCodec(_message.TransferSyntax) != null)
				{
					DicomCompressedPixelData pixelData = new DicomCompressedPixelData(_message);
					string pi;
					rawPixelData = pixelData.GetFrame(_frameIndex, out pi);
					photometricInterpretation = PhotometricInterpretation.FromCodeString(pi);
				}
				else
					throw new DicomCodecException("Unsupported transfer syntax");

				if (photometricInterpretation.IsColor)
					rawPixelData = ToArgb(_message.DataSet, rawPixelData, photometricInterpretation);
				else
					NormalizeGrayscalePixels(_message.DataSet, rawPixelData, _message.TransferSyntax);

				clock.Stop();
				PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "CreateFrameNormalizedPixelData", clock.Seconds);

				return rawPixelData;
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				int overlayIndex = overlayGroupNumber - 1;
				int overlayFrame = overlayFrameNumber - 1;

				// get the overlay data from cache if it has already been extracted/unpacked
				byte[] overlayData = _overlayCache[overlayIndex, overlayFrame];
				if (overlayData == null)
				{
					LoadOverlayPlane(overlayIndex); // force a load of the overlay plane
					overlayData = _overlayCache[overlayIndex, overlayFrame];
				}

				// release our strong reference and stick a placeholder in the cache so that we don't re-extract/re-unpack
				_overlayCache[overlayIndex, overlayFrame] = new byte[0];
				return overlayData;
			}

			private void ExtractOverlayFrames(byte[] rawPixelData, int bitsAllocated)
			{
				// if any overlays have embedded pixel data, extract them now or forever hold your peace
				DicomMessageBase message = this.Parent.SourceMessage;
				OverlayPlaneModuleIod overlayPlaneModule = new OverlayPlaneModuleIod(message.DataSet);
				foreach (OverlayPlane overlay in overlayPlaneModule)
				{
					if (overlay.IsEmbedded && _overlayCache[overlay.Index, _frameIndex] == null)
						_overlayCache[overlay.Index, _frameIndex] = OverlayData.Extract(overlay.OverlayBitPosition, bitsAllocated, false, rawPixelData);
				}
			}

			/// <summary>
			/// Loads all frames from the given overlay plane.
			/// </summary>
			/// <param name="index"></param>
			private void LoadOverlayPlane(int index)
			{
				DicomMessageBase message = this.Parent.SourceMessage;

				CodeClock clock = new CodeClock();
				clock.Start();

				OverlayPlaneModuleIod overlayPlanes = new OverlayPlaneModuleIod(message.DataSet);
				if (overlayPlanes.HasOverlayPlane(index))
				{
					OverlayPlane overlayPlane = overlayPlanes[index];
					if (overlayPlane.IsEmbedded)
					{
						this.GetNormalizedPixelData();
					}
					else
					{
						OverlayData odpd = new OverlayData(0, overlayPlane.OverlayRows, overlayPlane.OverlayColumns, overlayPlane.IsBigEndianOW, overlayPlane.OverlayData);
						int numberOfFrames = this.Parent.NumberOfFrames;
						byte[] overlayData;
						if (overlayPlane.IsMultiFrame)
						{
							// multiframe overlay: fill each frame of the overlay into associated image frame(s)
							for (int n = 1; n <= numberOfFrames; n++)
							{
								foreach (int frameIndex in overlayPlane.GetRelevantOverlayFrames(n - 1, numberOfFrames))
								{
									// TODO: handle unpacking individual frames
									Platform.Log(LogLevel.Warn, new NotSupportedException("Multiframe overlays are not supported."));
									overlayData = odpd.Unpack();
									DicomMessageSopFrameData fd = (DicomMessageSopFrameData)this.Parent.GetFrameData(n);
									fd._overlayCache[index, frameIndex] = overlayData;
								}
							}
						}
						else
						{
							overlayData = odpd.Unpack();
							for (int n = 1; n <= numberOfFrames; n++)
							{
								DicomMessageSopFrameData fd = (DicomMessageSopFrameData)this.Parent.GetFrameData(n);
								fd._overlayCache[index, 0] = overlayData;
							}
						}
					}
				}

				clock.Stop();
				PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "LoadOverlayPlane", clock.Seconds);
			}

			protected override void OnUnloaded()
			{
				_overlayCache.Clear();
				base.OnUnloaded();
			}
		}

		#endregion

		/// <summary>
		/// Normalizes grayscale pixel data by masking out non-data bits and shifting the data so that the high bit is always 1 less than bits stored.
		/// </summary>
		/// <param name="dicomAttributeProvider"></param>
		/// <param name="pixelData"></param>
		/// <param name="transferSyntax"></param>
		protected internal static void NormalizeGrayscalePixels(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData, TransferSyntax transferSyntax)
		{
			if (dicomAttributeProvider[DicomTags.BitsAllocated].IsEmpty)
				throw new ArgumentNullException("dicomAttributeProvider", "BitsAllocated must not be empty.");
			if (dicomAttributeProvider[DicomTags.BitsStored].IsEmpty)
				throw new ArgumentNullException("dicomAttributeProvider", "BitsStored must not be empty.");

			int bitsAllocated = dicomAttributeProvider[DicomTags.BitsAllocated].GetInt32(0, -1);
			int bitsStored = dicomAttributeProvider[DicomTags.BitsStored].GetInt32(0, -1);
			int highBit = dicomAttributeProvider[DicomTags.HighBit].GetInt32(0, bitsStored - 1);

			if (bitsAllocated != 8 && bitsAllocated != 16)
				throw new ArgumentException("BitsAllocated must be either 8 or 16.", "dicomAttributeProvider");

			unsafe
			{
				int shift = highBit + 1 - bitsStored;
				if (bitsAllocated == 16)
				{
					if (pixelData.Length%2 != 0)
						throw new ArgumentException("Pixel data length must be even.", "pixelData");

					ushort mask = (ushort) (((1 << bitsStored) - 1) << shift);
					int length = pixelData.Length;
					fixed (byte* data = pixelData)
					{
						ushort window;

						if (transferSyntax.LittleEndian)
						{
							for (int n = 0; n < length; n += 2)
							{
								window = (ushort) ((data[n + 1] << 8) + data[n]);
								window = (ushort) ((window & mask) >> shift);
								data[n] = (byte) (window & 0x00ff);
								data[n + 1] = (byte) ((window & 0xff00) >> 8);
							}
						}
						else
						{
							for (int n = 0; n < length; n += 2)
							{
								window = (ushort) ((data[n] << 8) + data[n + 1]);
								window = (ushort) ((window & mask) >> shift);
								data[n + 1] = (byte) (window & 0x00ff);
								data[n] = (byte) ((window & 0xff00) >> 8);
							}
						}
					}
				}
				else
				{
					byte mask = (byte) (((1 << bitsStored) - 1) << shift);
					int length = pixelData.Length;
					fixed (byte* data = pixelData)
					{
						for (int n = 0; n < length; n++)
						{
							data[n] = (byte) ((data[n] & mask) >> shift);
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts colour pixel data to ARGB.
		/// </summary>
		protected static byte[] ToArgb(IDicomAttributeProvider dicomAttributeProvider, byte[] pixelData, PhotometricInterpretation photometricInterpretation)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			int rows = dicomAttributeProvider[DicomTags.Rows].GetInt32(0, 0);
			int columns = dicomAttributeProvider[DicomTags.Columns].GetInt32(0, 0);
			int sizeInBytes = rows * columns * 4;
			byte[] argbPixelData = new byte[sizeInBytes];

			// Convert palette colour images to ARGB so we don't get interpolation artifacts
			// when rendering.
			if (photometricInterpretation == PhotometricInterpretation.PaletteColor)
			{
				int bitsAllocated = dicomAttributeProvider[DicomTags.BitsAllocated].GetInt32(0, 0);
				int pixelRepresentation = dicomAttributeProvider[DicomTags.PixelRepresentation].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					bitsAllocated,
					pixelRepresentation != 0 ? true : false,
					pixelData,
					argbPixelData,
					PaletteColorMap.Create(dicomAttributeProvider));
			}
			// Convert RGB and YBR variants to ARGB
			else
			{
				int planarConfiguration = dicomAttributeProvider[DicomTags.PlanarConfiguration].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					photometricInterpretation,
					planarConfiguration,
					pixelData,
					argbPixelData);
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "ToArgb", clock.Seconds);

			return argbPixelData;
		}
	}
}
