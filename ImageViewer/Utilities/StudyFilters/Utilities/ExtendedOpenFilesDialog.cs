#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	[ExtensionPoint]
	public sealed class ExtendedOpenFilesDialog : ExtensionPoint<IExtendedOpenFilesDialogProvider>
	{
		private ExtendedOpenFilesDialog() : base() {}

		public static IEnumerable<string> GetFiles(FileDialogCreationArgs args)
		{
			ExtendedOpenFilesDialog xp = new ExtendedOpenFilesDialog();
			IExtendedOpenFilesDialogProvider provider = xp.CreateExtension() as IExtendedOpenFilesDialogProvider;
			if (provider == null)
				throw new NotSupportedException();
			return provider.GetFiles(args);
		}
	}

	public interface IExtendedOpenFilesDialogProvider
	{
		IEnumerable<string> GetFiles(FileDialogCreationArgs args);
	}
}