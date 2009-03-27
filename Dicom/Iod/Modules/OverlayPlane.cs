#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// OverlayPlane Module
	/// </summary>
	/// <seealso cref="OverlayPlaneModuleIod.this"/>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public class OverlayPlaneModuleIod : IodBase, IEnumerable<OverlayPlane>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>	
		public OverlayPlaneModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>
		public OverlayPlaneModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// Gets the Overlays in the underlying collection. The index must be between 0 and 15 inclusive.
		/// </summary>
		/// <remarks>
		/// The implementation of the Overlay Plane module involving repeating groups is a holdover
		/// from previous versions of the DICOM Standard. For each of the 16 allowed overlays, there
		/// exists a separate set of tags bearing the same element numbers but with a group number
		/// of the form 60xx, where xx is an even number from 00 to 1E inclusive. In order to make
		/// these IOD classes easier to use, each of these 16 sets of tags are represented as
		/// separate items of a collection, and may be addressed by an index between 0 and 15
		/// inclusive (mapping to the even groups between 6000 and 601E).
		/// </remarks>
		public OverlayPlane this[int index]
		{
			get
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				return new OverlayPlane(ComputeTagOffset(index), this.DicomAttributeProvider);
			}
		}

		public bool HasOverlayPlane(int index)
		{
			if (index < 0 || index >= 16)
				return false;
			DicomAttribute attrib;
			if (!DicomAttributeProvider.TryGetAttribute(ComputeTagOffset(index) + DicomTags.OverlayBitPosition, out attrib))
				return false;
			return !attrib.IsEmpty;
		}

		private static uint ComputeTagOffset(int index)
		{
			return (uint) index*2*0x10000;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get
			{
				for (int n = 0; n < 16; n++)
				{
					uint tagOffset = ComputeTagOffset(n);
					yield return tagOffset + DicomTags.OverlayBitPosition;
					yield return tagOffset + DicomTags.OverlayBitsAllocated;
					yield return tagOffset + DicomTags.OverlayColumns;
					yield return tagOffset + DicomTags.OverlayData;
					yield return tagOffset + DicomTags.OverlayDescription;
					yield return tagOffset + DicomTags.OverlayLabel;
					yield return tagOffset + DicomTags.OverlayOrigin;
					yield return tagOffset + DicomTags.OverlayRows;
					yield return tagOffset + DicomTags.OverlaySubtype;
					yield return tagOffset + DicomTags.OverlayType;
					yield return tagOffset + DicomTags.RoiArea;
					yield return tagOffset + DicomTags.RoiMean;
					yield return tagOffset + DicomTags.RoiStandardDeviation;
				}
			}
		}

		#region IEnumerable<OverlayPlane> Members

		public IEnumerator<OverlayPlane> GetEnumerator()
		{
			for (int n = 0; n < 16; n++)
			{
				if (this.HasOverlayPlane(n))
					yield return this[n];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.OverlayType"/> attribute .
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public enum OverlayType
	{
		/// <summary>
		/// Graphics
		/// </summary>
		G,

		/// <summary>
		/// ROI
		/// </summary>
		R,

		/// <summary>
		/// Represents the null value, which is equivalent to the unknown status.
		/// </summary>
		None
	}

	/// <summary>
	/// Overlay Plane Item
	/// </summary>
	/// <seealso cref="OverlayPlaneModuleIod.this"/>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public class OverlayPlane : IodBase
	{
		private readonly uint _tagOffset;

		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlane"/> class.
		/// </summary>
		/// <param name="tagOffset">The tag offset for this overlay.</param>
		/// <param name="dicomAttributeProvider">The underlying collection.</param>
		internal OverlayPlane(uint tagOffset, IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
		{
			_tagOffset = tagOffset;
		}

		public uint TagOffset
		{
			get { return _tagOffset; }
		}

		public bool IsBigEndianOW
		{
			get { return ByteBuffer.LocalMachineEndian == Endian.Big && base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData] is DicomAttributeOW; }
		}

		/// <summary>
		/// Gets or sets the value of OverlayRows in the underlying collection. Type 1.
		/// </summary>
		public int OverlayRows
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayRows].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayRows].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayColumns in the underlying collection. Type 1.
		/// </summary>
		public int OverlayColumns
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayColumns].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayColumns].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayType in the underlying collection. Type 1.
		/// </summary>
		public OverlayType OverlayType
		{
			get { return ParseEnum(base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayType].GetString(0, string.Empty), OverlayType.None); }
			set
			{
				if (value == OverlayType.None)
					throw new ArgumentOutOfRangeException("value", "OverlayType is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayType], value, true);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayOrigin in the underlying collection. Type 1.
		/// </summary>
		public Point? OverlayOrigin
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayOrigin];
				int[] result = new int[2];
				if (attribute.TryGetInt32(0, out result[0]))
					if (attribute.TryGetInt32(1, out result[1]))
						return new Point(result[0], result[1]);
				return null;
			}
			set
			{
				if (!value.HasValue)
					throw new ArgumentNullException("value", "OverlayOrigin is Type 1 Required.");
				DicomAttribute attribute = base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayOrigin];
				attribute.SetInt32(0, value.Value.X);
				attribute.SetInt32(1, value.Value.Y);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitsAllocated in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitsAllocated
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitsAllocated].GetInt32(0, 0); }
			set
			{
				if (value != 1)
					throw new ArgumentOutOfRangeException("value", "OverlayBitsAllocated must be 1. Encoding overlay data in the unused bits of PixelData is not supported.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitsAllocated].SetInt32(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitPosition in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitPosition
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitPosition].GetInt32(0, 0); }
			set {
				if (value != 0)
					throw new ArgumentOutOfRangeException("value", "OverlayBitPosition must be 0. Encoding overlay data in the unused bits of PixelData is not supported.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayBitPosition].SetInt32(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayData in the underlying collection. Type 1.
		/// </summary>
		public byte[] OverlayData
		{
			get { return (byte[]) base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "OverlayData is Type 1 Required.");
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayData].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayDescription in the underlying collection. Type 3.
		/// </summary>
		public string OverlayDescription
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlaySubtype in the underlying collection. Type 3.
		/// </summary>
		public string OverlaySubtype
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlaySubtype].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayLabel in the underlying collection. Type 3.
		/// </summary>
		public string OverlayLabel
		{
			get { return base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.OverlayLabel].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiArea in the underlying collection. Type 3.
		/// </summary>
		public int? RoiArea
		{
			get
			{
				int result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiArea].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiMean in the underlying collection. Type 3.
		/// </summary>
		public double? RoiMean
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiMean].SetFloat64(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RoiStandardDeviation in the underlying collection. Type 3.
		/// </summary>
		public double? RoiStandardDeviation
		{
			get
			{
				double result;
				if (base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation] = null;
					return;
				}
				base.DicomAttributeProvider[_tagOffset + DicomTags.RoiStandardDeviation].SetFloat64(0, value.Value);
			}
		}

		public unsafe void ConvertEmbeddedOverlay(DicomUncompressedPixelData pd)
		{
			// General sanity checks
			if (pd.SamplesPerPixel > 1)
				throw new DicomException("Unable to convert embedded overlays when Samples Per Pixe > 1");
			if (pd.BitsStored == 8 && pd.BitsAllocated == 8)
				throw new DicomException("Unable to remove overlay with 8 Bits Stored and 8 Bits Allocated");
			if (pd.BitsStored == 16 && pd.BitsAllocated == 16)
				throw new DicomException("Unable to remove overlay with 16 Bits Stored and 16 Bits Allocated");

			int frameSize = pd.UncompressedFrameSize;
			int overlaySize = frameSize/pd.BitsAllocated;
			if (frameSize%pd.BitsAllocated > 0)
				overlaySize++;

			int numValues = frameSize/pd.BytesAllocated;

			byte[] overlay = new byte[overlaySize];
			int overlayOffset = 0;
			// Embededded overlay smust exist for all frames, they can't be for selected
			for (int i = 0; i < pd.NumberOfFrames; i++)
			{
				byte[] frameData = pd.GetFrame(i);

				if (pd.BitsAllocated <= 8)
				{
					byte pixelMask = ((byte)(0x1 << this.OverlayBitPosition ));
					byte overlayMask = 0x01;

					fixed (byte* pFrameData = frameData)
					{
						byte* pixelData = pFrameData;
						for (int p = 0; p < numValues; p++, pixelData++)
						{
							if ((*pixelData & pixelMask) != 0)
							{
								overlay[overlayOffset] |= overlayMask;
								*pixelData &= (byte)~pixelMask;
							}

							if (overlayMask == 0x80)
							{
								overlayMask = 0x01;
								overlayOffset++;
							}
							else
								overlayMask <<= 1;
						}
					}
				}
				else
				{
					fixed (byte* pFrameData = frameData)
					{
						ushort pixelMask = ((ushort)(0x1 << OverlayBitPosition));
						byte overlayMask = 0x01;

						ushort* pixelData = (ushort*) pFrameData;
						for (int p = 0; p < numValues; p++, pixelData++)
						{
							if ((*pixelData & pixelMask) != 0)
							{
								overlay[overlayOffset] |= overlayMask;
								*pixelData &= (ushort)~pixelMask;
							}

							if (overlayMask == 0x80)
							{
								overlayMask = 0x01;
								overlayOffset++;
							}
							else
								overlayMask <<= 1;
						}
					}
				}
			}

			// Assign the new overlay tags
			OverlayBitPosition = 0;
			OverlayBitsAllocated = 1;
			if (IsBigEndianOW)
			{
				// Just do a bulk swap, performance isn't much of an issue.
				ByteBuffer buffer = new ByteBuffer(overlay, Endian.Little);
				buffer.Swap2();
				OverlayData = buffer.ToBytes();
			}
			else
				OverlayData = overlay;

			// Cleanup Rows/Columns if necessary
			if (OverlayColumns == 0)
				OverlayColumns = pd.ImageWidth;
			if (OverlayRows == 0)
				OverlayRows = pd.ImageHeight;
		}
	}
}