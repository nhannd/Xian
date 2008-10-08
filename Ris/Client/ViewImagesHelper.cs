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
        void Open(string accessionNumber);
    	void Close(string accessionNumber);
    	void Activate(string accessionNumber);
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

		private static void CheckSupported()
		{
			if (_viewer == null)
				throw new NotSupportedException("No viewer integration extension found.");
		}

		public static bool IsSupported
		{
			get { return _viewer != null; }
		}

		public static void Open(string accession)
		{
			CheckSupported();
			_viewer.Open(accession);
		}

		public static bool Close(string accessionNumber)
		{
			CheckSupported();

			try
			{
				_viewer.Close(accessionNumber);
				return true;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to close the viewer for Accession# {0}.", accessionNumber));
			}

			return false;
		}

		public static bool Activate(string accessionNumber)
		{
			CheckSupported();

			try
			{
				_viewer.Activate(accessionNumber);
				return true;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, String.Format("Failed to activate the viewer for Accession# {0}.", accessionNumber));
			}

			return false;
		}
	}
}
