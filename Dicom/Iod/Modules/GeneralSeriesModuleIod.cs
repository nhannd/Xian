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
using ClearCanvas.Dicom.Iod.Macros.PerformedProcedureStepSummary;
using ClearCanvas.Dicom.Iod.Macros.PerformedProcedureStepSummary.PerformedProtocolCodeSequence;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GeneralSeries Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.3.1 (Table C.7-5a)</remarks>
	public class GeneralSeriesModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralSeriesModuleIod"/> class.
		/// </summary>	
		public GeneralSeriesModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeneralSeriesModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
		public GeneralSeriesModuleIod(DicomAttributeCollection dicomAttributeCollection) : base(dicomAttributeCollection) {}

		/// <summary>
		/// Gets or sets the value of Modality in the underlying collection. Type 1.
		/// </summary>
		public string Modality
		{
			get { return base.DicomAttributeCollection[DicomTags.Modality].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "Modality is Type 1 Required.");
				base.DicomAttributeCollection[DicomTags.Modality].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesInstanceUid in the underlying collection. Type 1.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return base.DicomAttributeCollection[DicomTags.SeriesInstanceUid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("value", "SeriesInstanceUid is Type 1 Required.");
				base.DicomAttributeCollection[DicomTags.SeriesInstanceUid].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesNumber in the underlying collection. Type 2.
		/// </summary>
		public int? SeriesNumber
		{
			get
			{
				int result;
				if (base.DicomAttributeCollection[DicomTags.SeriesNumber].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeCollection[DicomTags.SeriesNumber].SetNullValue();
					return;
				}
				base.DicomAttributeCollection[DicomTags.SeriesNumber].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Laterality in the underlying collection. Type 2C.
		/// </summary>
		public string Laterality
		{
			get { return base.DicomAttributeCollection[DicomTags.Laterality].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.Laterality] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.Laterality].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDate and SeriesTime in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? SeriesDateTime
		{
			get
			{
				string date = base.DicomAttributeCollection[DicomTags.SeriesDate].GetString(0, string.Empty);
				string time = base.DicomAttributeCollection[DicomTags.SeriesTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeCollection[DicomTags.SeriesDate] = null;
					base.DicomAttributeCollection[DicomTags.SeriesTime] = null;
					return;
				}
				DicomAttribute date = base.DicomAttributeCollection[DicomTags.SeriesDate];
				DicomAttribute time = base.DicomAttributeCollection[DicomTags.SeriesTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformingPhysiciansName in the underlying collection. Type 3.
		/// </summary>
		public string PerformingPhysiciansName
		{
			get { return base.DicomAttributeCollection[DicomTags.PerformingPhysiciansName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.PerformingPhysiciansName] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PerformingPhysiciansName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformingPhysicianIdentificationSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro[] PerformingPhysicianIdentificationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.PerformingPhysicianIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				PersonIdentificationMacro[] result = new PersonIdentificationMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PersonIdentificationMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.PerformingPhysicianIdentificationSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.PerformingPhysicianIdentificationSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of ProtocolName in the underlying collection. Type 3.
		/// </summary>
		public string ProtocolName
		{
			get { return base.DicomAttributeCollection[DicomTags.ProtocolName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.ProtocolName] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.ProtocolName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SeriesDescription in the underlying collection. Type 3.
		/// </summary>
		public string SeriesDescription
		{
			get { return base.DicomAttributeCollection[DicomTags.SeriesDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.SeriesDescription] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.SeriesDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OperatorsName in the underlying collection. Type 3.
		/// </summary>
		public string OperatorsName
		{
			get { return base.DicomAttributeCollection[DicomTags.OperatorsName].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.OperatorsName] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.OperatorsName].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of OperatorIdentificationSequence in the underlying collection. Type 3.
		/// </summary>
		public PersonIdentificationMacro[] OperatorIdentificationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.OperatorIdentificationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				PersonIdentificationMacro[] result = new PersonIdentificationMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PersonIdentificationMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.OperatorIdentificationSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.OperatorIdentificationSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro ReferencedPerformedProcedureStepSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ReferencedPerformedProcedureStepSequence];
				if (value == null)
				{
					base.DicomAttributeCollection[DicomTags.ReferencedPerformedProcedureStepSequence] = null;
					return;
				}
				dicomAttribute.Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Creates the ReferencedPerformedProcedureStepSequence in the underlying collection. Type 3.
		/// </summary>
		public ISopInstanceReferenceMacro CreateReferencedPerformedProcedureStepSequence()
		{
			DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ReferencedPerformedProcedureStepSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
			{
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] {dicomSequenceItem};
				ISopInstanceReferenceMacro sequenceType = new SopInstanceReferenceMacro(dicomSequenceItem);
				sequenceType.InitializeAttributes();
				return sequenceType;
			}
			return new SopInstanceReferenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
		}

		/// <summary>
		/// Gets or sets the value of RelatedSeriesSequence in the underlying collection. Type 3.
		/// </summary>
		public RelatedSeriesSequence[] RelatedSeriesSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.RelatedSeriesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				RelatedSeriesSequence[] result = new RelatedSeriesSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RelatedSeriesSequence(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.RelatedSeriesSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.RelatedSeriesSequence].Values = result;
			}
		}

		/// <summary>
		/// Gets or sets the value of BodyPartExamined in the underlying collection. Type 3.
		/// </summary>
		public string BodyPartExamined
		{
			get { return base.DicomAttributeCollection[DicomTags.BodyPartExamined].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.BodyPartExamined] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.BodyPartExamined].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PatientPosition in the underlying collection. Type 2C.
		/// </summary>
		public string PatientPosition
		{
			get { return base.DicomAttributeCollection[DicomTags.PatientPosition].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.PatientPosition] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PatientPosition].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of SmallestPixelValueInSeries in the underlying collection. Type 3.
		/// </summary>
		public int? SmallestPixelValueInSeries
		{
			get
			{
				int result;
				if (base.DicomAttributeCollection[DicomTags.SmallestPixelValueInSeries].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeCollection[DicomTags.SmallestPixelValueInSeries] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.SmallestPixelValueInSeries].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of LargestPixelValueInSeries in the underlying collection. Type 3.
		/// </summary>
		public int? LargestPixelValueInSeries
		{
			get
			{
				int result;
				if (base.DicomAttributeCollection[DicomTags.LargestPixelValueInSeries].TryGetInt32(0, out result))
					return result;
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeCollection[DicomTags.LargestPixelValueInSeries] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.LargestPixelValueInSeries].SetInt32(0, value.Value);
			}
		}

		/// <summary>
		/// Gets or sets the value of RequestAttributesSequence in the underlying collection. Type 3.
		/// </summary>
		public IRequestAttributesMacro[] RequestAttributesSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.RequestAttributesSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				IRequestAttributesMacro[] result = new IRequestAttributesMacro[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new RequestAttributesMacro(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.RequestAttributesSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.RequestAttributesSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a RequestAttributesSequence item. Does not modify the RequestAttributesSequence in the underlying collection.
		/// </summary>
		public IRequestAttributesMacro CreateRequestAttributesSequence()
		{
			IRequestAttributesMacro iodBase = new RequestAttributesMacro(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepId in the underlying collection. Type 3.
		/// </summary>
		public string PerformedProcedureStepId
		{
			get { return base.DicomAttributeCollection[DicomTags.PerformedProcedureStepId].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.PerformedProcedureStepId] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PerformedProcedureStepId].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepStartDate and PerformedProcedureStepStartTime in the underlying collection.  Type 3.
		/// </summary>
		public DateTime? PerformedProcedureStepStartDateTime
		{
			get
			{
				string date = base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate].GetString(0, string.Empty);
				string time = base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime].GetString(0, string.Empty);
				return DateTimeParser.ParseDateAndTime(string.Empty, date, time);
			}
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate] = null;
					base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime] = null;
					return;
				}
				DicomAttribute date = base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate];
				DicomAttribute time = base.DicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime];
				DateTimeParser.SetDateTimeAttributeValues(value, date, time);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProcedureStepDescription in the underlying collection. Type 3.
		/// </summary>
		public string PerformedProcedureStepDescription
		{
			get { return base.DicomAttributeCollection[DicomTags.PerformedProcedureStepDescription].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.PerformedProcedureStepDescription] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.PerformedProcedureStepDescription].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PerformedProtocolCodeSequence in the underlying collection. Type 3.
		/// </summary>
		public IPerformedProtocolCodeSequence[] PerformedProtocolCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.PerformedProtocolCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				IPerformedProtocolCodeSequence[] result = new IPerformedProtocolCodeSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new PerformedProtocolCodeSequenceClass(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeCollection[DicomTags.PerformedProtocolCodeSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.PerformedProtocolCodeSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a PerformedProtocolCodeSequence item. Does not modify the PerformedProtocolCodeSequence in the underlying collection.
		/// </summary>
		public IPerformedProtocolCodeSequence CreatePerformedProtocolCodeSequence()
		{
			IPerformedProtocolCodeSequence iodBase = new PerformedProtocolCodeSequenceClass(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		/// <summary>
		/// Gets or sets the value of CommentsOnThePerformedProcedureStep in the underlying collection. Type 3.
		/// </summary>
		public string CommentsOnThePerformedProcedureStep
		{
			get { return base.DicomAttributeCollection[DicomTags.CommentsOnThePerformedProcedureStep].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeCollection[DicomTags.CommentsOnThePerformedProcedureStep] = null;
					return;
				}
				base.DicomAttributeCollection[DicomTags.CommentsOnThePerformedProcedureStep].SetString(0, value);
			}
		}

		/// <summary>
		/// PerformedProtocol Code Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.13 (Table 10-16)</remarks>
		internal class PerformedProtocolCodeSequenceClass : CodeSequenceMacro, IPerformedProtocolCodeSequence
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PerformedProtocolCodeSequenceClass"/> class.
			/// </summary>
			public PerformedProtocolCodeSequenceClass() : base() {}

			/// <summary>
			/// Initializes a new instance of the <see cref="PerformedProtocolCodeSequenceClass"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public PerformedProtocolCodeSequenceClass(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

			/// <summary>
			/// Initializes the underlying collection to implement the module or sequence using default values.
			/// </summary>
			public virtual void InitializeAttributes() {}

			/// <summary>
			/// Gets or sets the value of ProtocolContextSequence in the underlying collection. Type 3.
			/// </summary>
			public IProtocolContextSequence[] ProtocolContextSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ProtocolContextSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					ProtocolContextSequenceClass[] result = new ProtocolContextSequenceClass[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new ProtocolContextSequenceClass(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						base.DicomAttributeCollection[DicomTags.ProtocolContextSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					base.DicomAttributeCollection[DicomTags.ProtocolContextSequence].Values = result;
				}
			}

			/// <summary>
			/// Creates a single instance of a ProtocolContextSequence item. Does not modify the tag in the underlying collection.
			/// </summary>
			public IProtocolContextSequence CreateProtocolContextSequence()
			{
				IProtocolContextSequence iodBase = new ProtocolContextSequenceClass(new DicomSequenceItem());
				iodBase.InitializeAttributes();
				return iodBase;
			}

			/// <summary>
			/// ProtocolContext Sequence
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section 10.13 (Table 10-16)</remarks>
			internal class ProtocolContextSequenceClass : ContentItemMacro, IProtocolContextSequence
			{
				/// <summary>
				/// Initializes a new instance of the <see cref="ProtocolContextSequenceClass"/> class.
				/// </summary>
				public ProtocolContextSequenceClass() : base() {}

				/// <summary>
				/// Initializes a new instance of the <see cref="ProtocolContextSequenceClass"/> class.
				/// </summary>
				/// <param name="dicomSequenceItem">The dicom sequence item.</param>
				public ProtocolContextSequenceClass(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) {}

				/// <summary>
				/// Initializes the underlying collection to implement the module or sequence using default values.
				/// </summary>
				public virtual void InitializeAttributes() {}

				/// <summary>
				/// Gets or sets the value of ContentItemModifierSequence in the underlying collection. Type 3.
				/// </summary>
				public ContentItemMacro[] ContentItemModifierSequence
				{
					get
					{
						DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ContentItemModifierSequence];
						if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
						{
							return null;
						}

						ContentItemMacro[] result = new ContentItemMacro[dicomAttribute.Count];
						DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
						for (int n = 0; n < items.Length; n++)
							result[n] = new ContentItemMacro(items[n]);

						return result;
					}
					set
					{
						if (value == null || value.Length == 0)
						{
							base.DicomAttributeCollection[DicomTags.ContentItemModifierSequence] = null;
							return;
						}

						DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
						for (int n = 0; n < value.Length; n++)
							result[n] = value[n].DicomSequenceItem;

						base.DicomAttributeCollection[DicomTags.ContentItemModifierSequence].Values = result;
					}
				}
			}
		}
	}
}