#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class AuthorityTokens
	{
		[AuthorityToken(Description = "Allow publishing of locally created data to remote servers.")]
		public const string Publishing = "Viewer/Publishing";

		public static class Configuration
		{
		    [AuthorityToken(Description = "Allow configuration of data publishing options.", Formerly = "Viewer/Administration/Key Images")]
			public const string Publishing = "Viewer/Configuration/Publishing";

            [AuthorityToken(Description = "Allow administration/configuration of the local DICOM Server (e.g. set AE Title, Port).", Formerly = "Viewer/Administration/DICOM Server")]
            public const string DicomServer = "Viewer/Configuration/DICOM Server";

            [AuthorityToken(Description = "Allow configuration of local DICOM storage.", Formerly = "Viewer/Administration/Storage")]
            public const string Storage = "Viewer/Configuration/Storage";
        }
	}
}
