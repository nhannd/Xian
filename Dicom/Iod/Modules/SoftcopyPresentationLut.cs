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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// SoftcopyPresentationLut Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.6 (Table C.11.6-1)</remarks>
	public class SoftcopyPresentationLutModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SoftcopyPresentationLutModuleIod"/> class.
		/// </summary>	
		public SoftcopyPresentationLutModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SoftcopyPresentationLutModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
		public SoftcopyPresentationLutModuleIod(DicomAttributeCollection dicomAttributeCollection) : base(dicomAttributeCollection) {}

		/// <summary>
		/// Gets or sets the value of PresentationLutSequence in the underlying collection. Type 1C.
		/// </summary>
		public PresentationLutSequenceItem PresentationLutSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.PresentationLutSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new PresentationLutSequenceItem(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.PresentationLutSequence];
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.PresentationLutSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of PresentationLutShape in the underlying collection. Type 1C.
		/// </summary>
		public PresentationLutShape PresentationLutShape
		{
			get { return ParseEnum(base.DicomAttributeCollection[DicomTags.PresentationLutShape].GetString(0, string.Empty), PresentationLutShape.None); }
			set
			{
				if (value == PresentationLutShape.None)
				{
					base.DicomAttributeCollection[DicomTags.PresentationLutShape] = null;
					return;
				}
				SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.PresentationLutShape], value);
			}
		}

		/// <summary>
		/// PresentationLut Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.6 (Table C.11.6-1)</remarks>
		public class PresentationLutSequenceItem : SequenceIodBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PresentationLutSequenceItem"/> class.
			/// </summary>
			public PresentationLutSequenceItem() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="PresentationLutSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public PresentationLutSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

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
			/// Gets or sets the value of LutData in the underlying collection. Type 1.
			/// </summary>
			public byte[] LutData
			{
				get
				{
					DicomAttribute attribute = base.DicomAttributeCollection[DicomTags.LutData];
					if (attribute.IsNull || attribute.IsEmpty)
						return null;
					return (byte[]) attribute.Values;
				}
				set
				{
					if (value == null)
						throw new ArgumentOutOfRangeException("value", "LutData is Type 1 Required.");
					base.DicomAttributeCollection[DicomTags.LutData].Values = value;
				}
			}
		}
	}
}