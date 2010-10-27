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

namespace ClearCanvas.Dicom.Iod.Modules
{
	/// <summary>
	/// IccProfile Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.11.15 (Table C.11.15-1)</remarks>
	public class IccProfileModuleIod : IodBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IccProfileModuleIod"/> class.
		/// </summary>	
		public IccProfileModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="IccProfileModuleIod"/> class.
		/// </summary>
		/// <param name="IDicomAttributeProvider">The dicom attribute provider.</param>
		public IccProfileModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) {}

		/// <summary>
		/// NOT IMPLEMENTED. Gets or sets the value of IccProfile in the underlying collection. Type 1.
		/// </summary> 		
		public object IccProfile
		{
			// TODO - Implement this.
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets an enumeration of <see cref="DicomTag"/>s used by this module.
		/// </summary>
		public static IEnumerable<uint> DefinedTags
		{
			get { yield return DicomTags.IccProfile; }
		}
	}
}