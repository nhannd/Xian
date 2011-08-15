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
	/// PatientBreed Code Sequence
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.7.1.1 (Table C.7-1)</remarks>
	[Obsolete("Use ContextGroups.Breed instead.")]
	public class PatientBreedCodeSequence : CodeSequenceMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PatientBreedCodeSequence"/> class.
		/// </summary>
		public PatientBreedCodeSequence() : base()
		{
			base.ContextIdentifier = "7480";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PatientBreedCodeSequence"/> class.
		/// </summary>
		/// <param name="dicomSequenceItem">The dicom sequence item.</param>
		public PatientBreedCodeSequence(DicomSequenceItem dicomSequenceItem) : base(dicomSequenceItem)
		{
			base.ContextIdentifier = "7480";
		}

		/// <summary>
		/// Converts a <see cref="PatientBreedCodeSequence"/> to a <see cref="Breed"/>.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static implicit operator Breed(PatientBreedCodeSequence code)
		{
			return new Breed(code.CodingSchemeDesignator, code.CodingSchemeVersion, code.CodeValue, code.CodeMeaning);
		}

		/// <summary>
		/// Converts a <see cref="Breed"/> to a <see cref="PatientBreedCodeSequence"/>.
		/// </summary>
		/// <param name="breed"></param>
		/// <returns></returns>
		public static implicit operator PatientBreedCodeSequence(Breed breed)
		{
			var codeSequence = new PatientBreedCodeSequence();
			breed.WriteToCodeSequence(codeSequence);
			return codeSequence;
		}
	}
}