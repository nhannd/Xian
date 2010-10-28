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
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Client
{
	public static class AuthorityTokens
	{
        public static class Desktop
        {
            [AuthorityToken(Description = "Allow administration of User-Interface validation rules.")]
            public const string UIValidationRules = "Desktop/UI Validation Rules";
		
			[AuthorityToken(Description = "Allow access to the RIS Folder Organization functionality.")]
			public const string FolderOrganization = "Desktop/RIS/Folder Organization";
		}
	}
}
