#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Macros;

namespace ClearCanvas.Dicom.Iod.Sequences
{
	/// <summary>
	/// BreedRegistry Code Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	[Obsolete("Use ContextGroups.BreedRegistry instead.")]
	public class BreedRegistryCodeSequence : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BreedRegistryCodeSequence"/> class.
		/// </summary>
		public BreedRegistryCodeSequence() : base()
		{
			base.ContextIdentifier = "7481";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BreedRegistryCodeSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public BreedRegistryCodeSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem)
		{
			base.ContextIdentifier = "7481";
		}

		/// <summary>
		/// Converts a <see cref="BreedRegistryCodeSequence"/> to a <see cref="BreedRegistry"/>.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static implicit operator BreedRegistry(BreedRegistryCodeSequence code)
		{
			return new BreedRegistry(code.CodingSchemeDesignator, code.CodingSchemeVersion, code.CodeValue, code.CodeMeaning);
		}

		/// <summary>
		/// Converts a <see cref="BreedRegistry"/> to a <see cref="BreedRegistryCodeSequence"/>.
		/// </summary>
		/// <param name="breedRegistry"></param>
		/// <returns></returns>
		public static implicit operator BreedRegistryCodeSequence(BreedRegistry breedRegistry)
		{
			var codeSequence = new BreedRegistryCodeSequence();
			breedRegistry.WriteToCodeSequence(codeSequence);
			return codeSequence;
		}
	}
}