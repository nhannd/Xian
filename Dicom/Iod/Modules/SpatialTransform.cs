#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// SpatialTransform Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.6 (Table C.10-6)</remarks>
	public class SpatialTransformModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialTransformModuleIod"/> class.
		/// </summary>	
		public SpatialTransformModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SpatialTransformModuleIod"/> class.
		/// </summary>
		public SpatialTransformModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Gets or sets the value of ImageRotation in the underlying collection. Type 1.
		/// </summary>
		public int ImageRotation
		{
			get { return base.DicomAttributeProvider[DicomTags.ImageRotation].GetInt32(0, 0); }
			set
			{
				if (value % 90 != 0)
					throw new ArgumentOutOfRangeException("value", "ImageRotation must be one of 0, 90, 180 or 270.");
				base.DicomAttributeProvider[DicomTags.ImageRotation].SetInt32(0, ((value % 360) + 360) % 360); // this ensures that the value stored is positive and < 360
			}
		}

		/// <summary>
		/// Gets or sets the value of ImageHorizontalFlip in the underlying collection. Type 1.
		/// </summary>
		public ImageHorizontalFlip ImageHorizontalFlip
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.ImageHorizontalFlip].GetString(0, string.Empty), ImageHorizontalFlip.None); }
			set
			{
				if (value == ImageHorizontalFlip.None)
					throw new ArgumentOutOfRangeException("value", "ImageHorizontalFlip is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ImageHorizontalFlip], value);
			}
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield return DicomTags.ImageHorizontalFlip;
				yield return DicomTags.ImageRotation;
			}
		}
	}

	/// <summary>
	/// Enumerated values for the <see cref="DicomTags.ImageHorizontalFlip"/> attribute describing whether or not to flip the image horizontally.
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.10.6 (Table C.10-6)</remarks>
	public enum ImageHorizontalFlip {
		Y,
		N,

		/// <summary>
		/// Represents the null value, which is equivalent to the unknown status.
		/// </summary>
		None
	}
}
