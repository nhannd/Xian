#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// Mask Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.5.10 (Table ?)</remarks>
	public class MaskModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MaskModuleIod"/> class.
		/// </summary>	
		public MaskModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="MaskModuleIod"/> class.
		/// </summary>
		public MaskModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		//TODO: Implement Mask Module when we support masks (This is a conditional module for GSPS IOD)

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags {
			get {
				yield break;
			}
		}
	}
}