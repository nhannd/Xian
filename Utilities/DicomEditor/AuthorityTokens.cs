#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Utilities.DicomEditor
{
	public static class AuthorityTokens
	{
		public class Study
		{
			[AuthorityToken(Description = "Permission to anonymize a study in the viewer.")]
			public const string Anonymize = "Viewer/Study/Anonymize";
		}

		[AuthorityToken(Description = "Grant access to the DICOM Editor.")]
		public const string DicomEditor = "Viewer/DICOM Editor";
	}
}
