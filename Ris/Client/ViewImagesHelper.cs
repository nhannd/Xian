using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class ViewerIntegrationExtensionPoint : ExtensionPoint<IViewerIntegration>
    {
    }

    public interface IViewerIntegration
    {
        void OpenStudy(string accessionNumber);
    }

	public static class ViewImagesHelper
	{
		private static readonly IViewerIntegration _viewer;

		static ViewImagesHelper()
		{
			try
			{
				_viewer = (IViewerIntegration)(new ViewerIntegrationExtensionPoint()).CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No viewer integration extension found.");
			}
		}

		public static bool IsSupported
		{
			get { return _viewer != null; }
		}

		public static void OpenStudy(string accession)
		{
			if(_viewer == null)
				throw new NotSupportedException("No viewer integration extension found.");

			_viewer.OpenStudy(accession);
		}
	}
}
