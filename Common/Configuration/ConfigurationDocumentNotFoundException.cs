#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Exception indicates that a requested configuration document does not exist.
	/// </summary>
	public class ConfigurationDocumentNotFoundException : Exception
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConfigurationDocumentNotFoundException(ConfigurationDocumentKey documentKey)
			: base(FormatMessage(documentKey))
		{
		}

		private static string FormatMessage(ConfigurationDocumentKey documentKey)
		{
			return string.Format("The document {0}, Version={1}, User={2}, Instance={3} does not exist.",
						  documentKey.DocumentName,
						  documentKey.Version,
						  documentKey.User,
						  documentKey.InstanceKey);
		}
	}
}
