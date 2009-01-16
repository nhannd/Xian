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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// GraphicAnnotation Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
	public class GraphicAnnotationModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicAnnotationModuleIod"/> class.
		/// </summary>	
		public GraphicAnnotationModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphicAnnotationModuleIod"/> class.
		/// </summary>
		/// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
		public GraphicAnnotationModuleIod(DicomAttributeCollection dicomAttributeCollection) : base(dicomAttributeCollection) {}

		/// <summary>
		/// Gets or sets the value of GraphicAnnotationSequence in the underlying collection. Type 1.
		/// </summary>
		public GraphicAnnotationSequenceItem[] GraphicAnnotationSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.GraphicAnnotationSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					return null;

				GraphicAnnotationSequenceItem[] result = new GraphicAnnotationSequenceItem[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new GraphicAnnotationSequenceItem(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value", "GraphicAnnotationSequence is Type 1 Required.");

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeCollection[DicomTags.GraphicAnnotationSequence].Values = result;
			}
		}

		/// <summary>
		/// GraphicAnnotation Sequence
		/// </summary>
		/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
		public class GraphicAnnotationSequenceItem : SequenceIodBase {
			/// <summary>
			/// Initializes a new instance of the <see cref="GraphicAnnotationSequenceItem"/> class.
			/// </summary>
			public GraphicAnnotationSequenceItem() : base() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="GraphicAnnotationSequenceItem"/> class.
			/// </summary>
			/// <param name="dicomSequenceItem">The dicom sequence item.</param>
			public GraphicAnnotationSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

			/// <summary>
			/// Gets or sets the value of ReferencedImageSequence in the underlying collection. Type 1C.
			/// </summary>
			public ImageSopInstanceReferenceMacro[] ReferencedImageSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.ReferencedImageSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					ImageSopInstanceReferenceMacro[] result = new ImageSopInstanceReferenceMacro[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new ImageSopInstanceReferenceMacro(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						base.DicomAttributeCollection[DicomTags.ReferencedImageSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					base.DicomAttributeCollection[DicomTags.ReferencedImageSequence].Values = result;
				}
			}

			/// <summary>
			/// Gets or sets the value of GraphicLayer in the underlying collection. Type 1.
			/// </summary>
			public string GraphicLayer
			{
				get { return base.DicomAttributeCollection[DicomTags.GraphicLayer].GetString(0, string.Empty); }
				set
				{
					if (string.IsNullOrEmpty(value))
						throw new ArgumentNullException("value", "GraphicLayer is Type 1 Required.");
					base.DicomAttributeCollection[DicomTags.GraphicLayer].SetString(0, value);
				}
			}

			/// <summary>
			/// Gets or sets the value of TextObjectSequence in the underlying collection. Type 1C.
			/// </summary>
			public TextObjectSequenceItem[] TextObjectSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.TextObjectSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					TextObjectSequenceItem[] result = new TextObjectSequenceItem[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new TextObjectSequenceItem(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						base.DicomAttributeCollection[DicomTags.TextObjectSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					base.DicomAttributeCollection[DicomTags.TextObjectSequence].Values = result;
				}
			}

			/// <summary>
			/// Gets or sets the value of GraphicObjectSequence in the underlying collection. Type 1C.
			/// </summary>
			public GraphicObjectSequenceItem[] GraphicObjectSequence
			{
				get
				{
					DicomAttribute dicomAttribute = base.DicomAttributeCollection[DicomTags.GraphicObjectSequence];
					if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
					{
						return null;
					}

					GraphicObjectSequenceItem[] result = new GraphicObjectSequenceItem[dicomAttribute.Count];
					DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
					for (int n = 0; n < items.Length; n++)
						result[n] = new GraphicObjectSequenceItem(items[n]);

					return result;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						base.DicomAttributeCollection[DicomTags.GraphicObjectSequence] = null;
						return;
					}

					DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
					for (int n = 0; n < value.Length; n++)
						result[n] = value[n].DicomSequenceItem;

					base.DicomAttributeCollection[DicomTags.GraphicObjectSequence].Values = result;
				}
			}

			/// <summary>
			/// TextObject Sequence
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public class TextObjectSequenceItem : SequenceIodBase {
				/// <summary>
				/// Initializes a new instance of the <see cref="TextObjectSequenceItem"/> class.
				/// </summary>
				public TextObjectSequenceItem() : base() { }

				/// <summary>
				/// Initializes a new instance of the <see cref="TextObjectSequenceItem"/> class.
				/// </summary>
				/// <param name="dicomSequenceItem">The dicom sequence item.</param>
				public TextObjectSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

				/// <summary>
				/// Gets or sets the value of BoundingBoxAnnotationUnits in the underlying collection. Type 1C.
				/// </summary>
				public BoundingBoxAnnotationUnits BoundingBoxAnnotationUnits
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.BoundingBoxAnnotationUnits].GetString(0, string.Empty), BoundingBoxAnnotationUnits.None); }
					set
					{
						if (value == BoundingBoxAnnotationUnits.None)
						{
							base.DicomAttributeCollection[DicomTags.BoundingBoxAnnotationUnits] = null;
							return;
						}
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.BoundingBoxAnnotationUnits], value);
					}
				}

				/// <summary>
				/// Gets or sets the value of AnchorPointAnnotationUnits in the underlying collection. Type 1C.
				/// </summary>
				public AnchorPointAnnotationUnits AnchorPointAnnotationUnits
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.AnchorPointAnnotationUnits].GetString(0, string.Empty), AnchorPointAnnotationUnits.None); }
					set
					{
						if (value == AnchorPointAnnotationUnits.None)
						{
							base.DicomAttributeCollection[DicomTags.AnchorPointAnnotationUnits] = null;
							return;
						}
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.AnchorPointAnnotationUnits], value);
					}
				}

				/// <summary>
				/// Gets or sets the value of UnformattedTextValue in the underlying collection. Type 1.
				/// </summary>
				public string UnformattedTextValue
				{
					get { return base.DicomAttributeCollection[DicomTags.UnformattedTextValue].GetString(0, string.Empty); }
					set
					{
						if (string.IsNullOrEmpty(value))
							throw new ArgumentNullException("value", "UnformattedTextValue is Type 1 Required.");
						base.DicomAttributeCollection[DicomTags.UnformattedTextValue].SetString(0, value);
					}
				}

				/// <summary>
				/// Gets or sets the value of BoundingBoxTopLeftHandCorner in the underlying collection. Type 1C.
				/// </summary>
				public double[] BoundingBoxTopLeftHandCorner
				{
					get
					{
						double[] result = new double[2];
						if (base.DicomAttributeCollection[DicomTags.BoundingBoxTopLeftHandCorner].TryGetFloat64(0, out result[0]))
							if (base.DicomAttributeCollection[DicomTags.BoundingBoxTopLeftHandCorner].TryGetFloat64(0, out result[1]))
								return result;
						return null;
					}
					set
					{
						if (value == null || value.Length != 2)
						{
							base.DicomAttributeCollection[DicomTags.BoundingBoxTopLeftHandCorner] = null;
							return;
						}
						base.DicomAttributeCollection[DicomTags.BoundingBoxTopLeftHandCorner].SetFloat64(0, value[0]);
						base.DicomAttributeCollection[DicomTags.BoundingBoxTopLeftHandCorner].SetFloat64(1, value[1]);
					}
				}

				/// <summary>
				/// Gets or sets the value of BoundingBoxBottomRightHandCorner in the underlying collection. Type 1C.
				/// </summary>
				public double[] BoundingBoxBottomRightHandCorner
				{
					get
					{
						double[] result = new double[2];
						if (base.DicomAttributeCollection[DicomTags.BoundingBoxBottomRightHandCorner].TryGetFloat64(0, out result[0]))
							if (base.DicomAttributeCollection[DicomTags.BoundingBoxBottomRightHandCorner].TryGetFloat64(0, out result[1]))
								return result;
						return null;
					}
					set
					{
						if (value == null || value.Length != 2)
						{
							base.DicomAttributeCollection[DicomTags.BoundingBoxBottomRightHandCorner] = null;
							return;
						}
						base.DicomAttributeCollection[DicomTags.BoundingBoxBottomRightHandCorner].SetFloat64(0, value[0]);
						base.DicomAttributeCollection[DicomTags.BoundingBoxBottomRightHandCorner].SetFloat64(1, value[1]);
					}
				}

				/// <summary>
				/// Gets or sets the value of BoundingBoxTextHorizontalJustification in the underlying collection. Type 1C.
				/// </summary>
				public BoundingBoxTextHorizontalJustification BoundingBoxTextHorizontalJustification
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.BoundingBoxTextHorizontalJustification].GetString(0, string.Empty), BoundingBoxTextHorizontalJustification.None); }
					set
					{
						if (value == BoundingBoxTextHorizontalJustification.None)
						{
							base.DicomAttributeCollection[DicomTags.BoundingBoxTextHorizontalJustification] = null;
							return;
						}
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.BoundingBoxTextHorizontalJustification], value);
					}
				}

				/// <summary>
				/// Gets or sets the value of AnchorPoint in the underlying collection. Type 1C.
				/// </summary>
				public double[] AnchorPoint
				{
					get
					{
						double[] result = new double[2];
						if (base.DicomAttributeCollection[DicomTags.AnchorPoint].TryGetFloat64(0, out result[0]))
							if (base.DicomAttributeCollection[DicomTags.AnchorPoint].TryGetFloat64(0, out result[1]))
								return result;
						return null;
					}
					set
					{
						if (value == null || value.Length != 2)
						{
							base.DicomAttributeCollection[DicomTags.AnchorPoint] = null;
							return;
						}
						base.DicomAttributeCollection[DicomTags.AnchorPoint].SetFloat64(0, value[0]);
						base.DicomAttributeCollection[DicomTags.AnchorPoint].SetFloat64(1, value[1]);
					}
				}

				/// <summary>
				/// Gets or sets the value of AnchorPointVisibility in the underlying collection. Type 1C.
				/// </summary>
				public AnchorPointVisibility AnchorPointVisibility
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.AnchorPointVisibility].GetString(0, string.Empty), AnchorPointVisibility.None); }
					set
					{
						if (value == AnchorPointVisibility.None)
						{
							base.DicomAttributeCollection[DicomTags.AnchorPointVisibility] = null;
							return;
						}
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.AnchorPointVisibility], value);
					}
				}
			}

			/// <summary>
			/// GraphicObject Sequence
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public class GraphicObjectSequenceItem : SequenceIodBase {
				/// <summary>
				/// Initializes a new instance of the <see cref="GraphicObjectSequenceItem"/> class.
				/// </summary>
				public GraphicObjectSequenceItem() : base() { }

				/// <summary>
				/// Initializes a new instance of the <see cref="GraphicObjectSequenceItem"/> class.
				/// </summary>
				/// <param name="dicomSequenceItem">The dicom sequence item.</param>
				public GraphicObjectSequenceItem(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem) { }

				/// <summary>
				/// Gets or sets the value of GraphicAnnotationUnits in the underlying collection. Type 1.
				/// </summary>
				public GraphicAnnotationUnits GraphicAnnotationUnits
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.GraphicAnnotationUnits].GetString(0, string.Empty), GraphicAnnotationUnits.None); }
					set
					{
						if (value == GraphicAnnotationUnits.None)
							throw new ArgumentOutOfRangeException("value", "GraphicAnnotationUnits is Type 1 Required.");
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.GraphicAnnotationUnits], value);
					}
				}

				/// <summary>
				/// Gets or sets the value of GraphicDimensions in the underlying collection. Type 1.
				/// </summary>
				public int GraphicDimensions
				{
					get { return base.DicomAttributeCollection[DicomTags.GraphicDimensions].GetInt32(0, 2); }
					set
					{
						if (value != 2)
							throw new ArgumentOutOfRangeException("value", "GraphicDimensions must be 2.");
						base.DicomAttributeCollection[DicomTags.GraphicDimensions].SetInt32(0, value);
					}
				}

				/// <summary>
				/// Gets or sets the value of NumberOfGraphicPoints in the underlying collection. Type 1.
				/// </summary>
				public int NumberOfGraphicPoints
				{
					get { return base.DicomAttributeCollection[DicomTags.NumberOfGraphicPoints].GetInt32(0, 0); }
					set { base.DicomAttributeCollection[DicomTags.NumberOfGraphicPoints].SetInt32(0, value); }
				}

				/// <summary>
				/// Gets or sets the value of GraphicData in the underlying collection. Type 1.
				/// </summary>
				public double[] GraphicData
				{
					get
					{
						DicomAttribute attribute = base.DicomAttributeCollection[DicomTags.GraphicData];
						if (attribute.IsEmpty || attribute.IsNull || attribute.Count == 0)
							return null;
						return (double[])attribute.Values;
					}
					set
					{
						if (value == null || value.Length != base.DicomAttributeCollection[DicomTags.GraphicData].Count)
							throw new ArgumentNullException("value", "GraphicData is Type 1 Required.");
						base.DicomAttributeCollection[DicomTags.GraphicData].Values = value;
					}
				}

				/// <summary>
				/// Gets or sets the value of GraphicType in the underlying collection. Type 1.
				/// </summary>
				public GraphicType GraphicType
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.GraphicType].GetString(0, string.Empty), GraphicType.None); }
					set
					{
						if (value == GraphicType.None)
							throw new ArgumentOutOfRangeException("value", "GraphicType is Type 1 Required.");
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.GraphicType], value);
					}
				}

				/// <summary>
				/// Gets or sets the value of GraphicFilled in the underlying collection. Type 1C.
				/// </summary>
				public GraphicFilled GraphicFilled
				{
					get { return ParseEnum(base.DicomAttributeCollection[DicomTags.GraphicFilled].GetString(0, string.Empty), GraphicFilled.None); }
					set
					{
						if (value == GraphicFilled.None)
						{
							base.DicomAttributeCollection[DicomTags.GraphicFilled] = null;
							return;
						}
						SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.GraphicFilled], value);
					}
				}
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.BoundingBoxAnnotationUnits"/> attribute defining whether or not the annotation is Image or Displayed Area relative.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum BoundingBoxAnnotationUnits {
				Pixel,
				Display,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.AnchorPointAnnotationUnits"/> attribute defining whether or not the annotation is Image or Displayed Area relative.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum AnchorPointAnnotationUnits {
				Pixel,
				Display,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.GraphicAnnotationUnits"/> attribute defining whether or not the annotation is Image or Displayed Area relative.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum GraphicAnnotationUnits {
				Pixel,
				Display,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.BoundingBoxTextHorizontalJustification"/> attribute describing the location of the text relative to the vertical edges of the bounding box.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum BoundingBoxTextHorizontalJustification {
				Left,
				Right,
				Center,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.AnchorPointVisibility"/> attribute.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum AnchorPointVisibility {
				Yes,
				No,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.GraphicType"/> attribute describing the shape of the graphic that is to be drawn.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum GraphicType {
				Point,
				Polyline,
				Interpolated,
				Circle,
				Ellipse,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}

			/// <summary>
			/// Enumerated values for the <see cref="DicomTags.GraphicFilled"/> attribute.
			/// </summary>
			/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.5 (Table C.10-5)</remarks>
			public enum GraphicFilled {
				Yes,
				No,

				/// <summary>
				/// Represents the null value, which is equivalent to the unknown status.
				/// </summary>
				None
			}
		}
	}
}
