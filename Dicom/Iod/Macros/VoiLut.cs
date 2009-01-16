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
using ClearCanvas.Dicom.Iod.Macros.VoiLut;

namespace ClearCanvas.Dicom.Iod.Macros
{
	/// <summary>
	/// VoiLut Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
	public interface IVoiLutMacro : IIodMacro
	{
		/// <summary>
		/// Gets or sets the value of VoiLutSequence in the underlying collection. Type 1C.
		/// </summary>
		VoiLutSequenceItem[] VoiLutSequence { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowCenter in the underlying collection. Type 1C.
		/// </summary>
		double[] WindowCenter { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowWidth in the underlying collection. Type 1C.
		/// </summary>
		byte[] WindowWidth { get; set; }

		/// <summary>
		/// Gets or sets the value of WindowCenterWidthExplanation in the underlying collection. Type 3.
		/// </summary>
		string WindowCenterWidthExplanation { get; set; }

		/// <summary>
		/// Gets or sets the value of VoiLutFunction in the underlying collection. Type 3.
		/// </summary>
		string VoiLutFunction { get; set; }
	}

	/// <summary>
	/// VoiLut Macro
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
	internal class VoiLutMacro : SequenceIodBase, IVoiLutMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutMacro"/> class.
		/// </summary>
		public VoiLutMacro() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="VoiLutMacro"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public VoiLutMacro(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

		public void InitializeAttributes() {}

		/// <summary>
		/// Gets or sets the value of VoiLutSequence in the underlying collection. Type 1C.
		/// </summary>
		public VoiLutSequenceItem[] VoiLutSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.VoiLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				VoiLutSequenceItem[] result = new VoiLutSequenceItem[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new VoiLutSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.VoiLutSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.VoiLutSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowCenter in the underlying collection. Type 1C.
		/// </summary>
		public double[] WindowCenter
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeCollection[DicomTags.WindowCenter];
				if (attribute.IsNull || attribute.IsEmpty)
					return null;
				return (double[]) attribute.Values;
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.WindowCenter] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.WindowCenter].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowWidth in the underlying collection. Type 1C.
		/// </summary>
		public byte[] WindowWidth
		{
			get
			{
				DicomAttribute attribute = base.DicomAttributeCollection[DicomTags.WindowWidth];
				if (attribute.IsNull || attribute.IsEmpty)
					return null;
				return (byte[]) attribute.Values;
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.WindowWidth] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.WindowWidth].Values = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of WindowCenterWidthExplanation in the underlying collection. Type 3.
		/// </summary>
		public string WindowCenterWidthExplanation
		{
			get { return base.DicomAttributeCollection[DicomTags.WindowCenterWidthExplanation].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.WindowCenterWidthExplanation] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.WindowCenterWidthExplanation].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of VoiLutFunction in the underlying collection. Type 3.
		/// </summary>
		public string VoiLutFunction
		{
			get { return base.DicomAttributeCollection[DicomTags.VoiLutFunction].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.VoiLutFunction] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.VoiLutFunction].SetString(0, value);
			}
		}
	}

	namespace VoiLut
	{
		/// <summary>
		/// VoiLut Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.2 (Table C.11-2b)</remarks>
		public class VoiLutSequenceItem : SequenceIodBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="VoiLutSequenceItem"/> class.
			/// </summary>
			public VoiLutSequenceItem() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="VoiLutSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public VoiLutSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Gets or sets the value of LutDescriptor in the underlying collection. Type 1.
			/// </summary>
			public int[] LutDescriptor
			{
				get
				{
					int[] result = new int[3];
					if (base.DicomAttributeCollection[DicomTags.LutDescriptor].TryGetInt32(0, out result[0]))
						if (base.DicomAttributeCollection[DicomTags.LutDescriptor].TryGetInt32(1, out result[1]))
							if (base.DicomAttributeCollection[DicomTags.LutDescriptor].TryGetInt32(2, out result[2]))
								return result;
					return null;
				}
				set
				{
					if (value == null || value.Length != 3)
						throw new ArgumentNullException("value", "LutDescriptor is Type 1 Required.");
					base.DicomAttributeCollection[DicomTags.LutDescriptor].SetInt32(0, value[0]);
					base.DicomAttributeCollection[DicomTags.LutDescriptor].SetInt32(1, value[1]);
					base.DicomAttributeCollection[DicomTags.LutDescriptor].SetInt32(2, value[2]);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutExplanation in the underlying collection. Type 3.
			/// </summary>
			public string LutExplanation
			{
				get { return base.DicomAttributeCollection[DicomTags.LutExplanation].GetString(0, string.Empty); }
				set
				{
					if (string.IsNullOrEmpty(value))
					{
						base.DicomAttributeCollection[DicomTags.LutExplanation] = null;
						return;
					}
					base.DicomAttributeCollection[DicomTags.LutExplanation].SetString(0, value);
				}
			}

			/// <summary>
			/// Gets or sets the value of LutData in the underlying collection. Type 1C.
			/// </summary>
			public byte[] LutData
			{
				get
				{
					DicomAttribute attribute = base.DicomAttributeCollection[DicomTags.LutData];
					if (attribute.IsNull || attribute.IsEmpty || attribute.Count == 0)
						return null;
					return (byte[]) attribute.Values;
				}
				set
				{
					if (value == null)
					{
						base.DicomAttributeCollection[DicomTags.LutData] = null;
						return;
					}
					base.DicomAttributeCollection[DicomTags.LutData].Values = value;
				}
			}
		}
	}
}