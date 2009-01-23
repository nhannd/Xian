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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// OverlayPlane Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public class OverlayPlaneModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>	
		public OverlayPlaneModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="OverlayPlaneModuleIod"/> class.
		/// </summary>
		public OverlayPlaneModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		//TODO: Overlay tags are actually 60xx,eeee where xx is anything from 00 to 1E inclusive, each representing a single layer. This still needs to be implemented.

		/// <summary>
		/// Gets or sets the value of OverlayRows in the underlying collection. Type 1.
		/// </summary>
		public int OverlayRows
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayRows].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.OverlayRows].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayColumns in the underlying collection. Type 1.
		/// </summary>
		public int OverlayColumns
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayColumns].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.OverlayColumns].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayType in the underlying collection. Type 1.
		/// </summary>
		public OverlayType OverlayType
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.OverlayType].GetString(0, string.Empty), OverlayType.None); }
			set
			{
				if (value == OverlayType.None)
					throw new ArgumentOutOfRangeException("value", "OverlayType is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.OverlayType], value, true);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayOrigin in the underlying collection. Type 1.
		/// </summary>
		public string OverlayOrigin
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayOrigin].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "OverlayOrigin is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.OverlayOrigin].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitsAllocated in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitsAllocated
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayBitsAllocated].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.OverlayBitsAllocated].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayBitPosition in the underlying collection. Type 1.
		/// </summary>
		public int OverlayBitPosition
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayBitPosition].GetInt32(0, 0); }
			set { base.DicomAttributeProvider[DicomTags.OverlayBitPosition].SetInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the value of OverlayData in the underlying collection. Type 1.
		/// </summary>
		public byte[] OverlayData
		{
			get { return (byte[])base.DicomAttributeProvider[DicomTags.OverlayData].Values; }
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "OverlayData is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.OverlayData].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayDescription in the underlying collection. Type 3.
		/// </summary>
		public string OverlayDescription
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OverlayDescription] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OverlayDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlaySubtype in the underlying collection. Type 3.
		/// </summary>
		public string OverlaySubtype
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlaySubtype].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OverlaySubtype] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OverlaySubtype].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OverlayLabel in the underlying collection. Type 3.
		/// </summary>
		public string OverlayLabel
		{
			get { return base.DicomAttributeProvider[DicomTags.OverlayLabel].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.OverlayLabel] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.OverlayLabel].SetString(0, value);
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
				if (base.DicomAttributeProvider[DicomTags.RoiArea].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.RoiArea] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RoiArea].SetInt32(0, value.Value);
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
				if (base.DicomAttributeProvider[DicomTags.RoiMean].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.RoiMean] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RoiMean].SetFloat64(0, value.Value);
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
				if (base.DicomAttributeProvider[DicomTags.RoiStandardDeviation].TryGetFloat64(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.RoiStandardDeviation] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.RoiStandardDeviation].SetFloat64(0, value.Value);
			}
		}
	}

	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.OverlayType"/> attribute .
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.9.2 (Table C.9-2)</remarks>
	public enum OverlayType {
		G,
		R,

		/// <summary>
		/// Represents the null value, which is equivalent to the unknown status.
		/// </summary>
		None
	}
}
