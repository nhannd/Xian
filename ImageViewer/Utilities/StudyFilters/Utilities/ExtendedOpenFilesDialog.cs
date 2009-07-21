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