using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ILayoutManager
	{
		private IImageViewer _imageViewer;
		private bool _physicalWorkspaceLayoutSet = false;

		// Constructor
		public LayoutManager()
		{	
		}

		#region ILayoutManager Members

		public void Layout(IImageViewer imageViewer)
		{
			SimpleLogicalWorkspaceBuilder.Build(imageViewer);
			LayoutPhysicalWorkspace(imageViewer.PhysicalWorkspace);
			SimplePhysicalWorkspaceFiller.Fill(imageViewer);
			imageViewer.PhysicalWorkspace.Draw();
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		#endregion

		private void LayoutPhysicalWorkspace(IPhysicalWorkspace physicalWorkspace)
		{
			if (_physicalWorkspaceLayoutSet)
				return;

			_physicalWorkspaceLayoutSet = true;

			StoredLayoutConfiguration configuration = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities in the study).
			if (physicalWorkspace.LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = physicalWorkspace.LogicalWorkspace.ImageSets[0];
				foreach(IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (configuration == null)
						configuration = LayoutConfigurationSettings.GetMinimumConfiguration();

					StoredLayoutConfiguration storedConfiguration = LayoutConfigurationSettings.Default.GetLayoutConfiguration(displaySet.PresentationImages[0] as IImageSopProvider);
					configuration.ImageBoxRows = Math.Max(configuration.ImageBoxRows, storedConfiguration.ImageBoxRows);
					configuration.ImageBoxColumns = Math.Max(configuration.ImageBoxColumns, storedConfiguration.ImageBoxColumns);
					configuration.TileRows = Math.Max(configuration.TileRows, storedConfiguration.TileRows);
					configuration.TileColumns = Math.Max(configuration.TileColumns, storedConfiguration.TileColumns);
				}
			}

			if (configuration == null)
				configuration = LayoutConfigurationSettings.Default.DefaultConfiguration;

			physicalWorkspace.SetImageBoxGrid(configuration.ImageBoxRows, configuration.ImageBoxColumns);
			for (int i = 0; i < physicalWorkspace.ImageBoxes.Count; ++i)
				physicalWorkspace.ImageBoxes[i].SetTileGrid(configuration.TileRows, configuration.TileColumns);
		}
	}
}
