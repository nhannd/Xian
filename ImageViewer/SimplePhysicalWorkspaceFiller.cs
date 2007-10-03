namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Fills all available <see cref="IImageBox"/>es in the
	/// <see cref="IPhysicalWorkspace"/> with all available
	/// <see cref="IDisplaySet"/>s in the <see cref="ILogicalWorkspace"/>.
	/// </summary>
	/// <remarks>
	/// Filling is done from left to right, top to bottom. If there are
	/// fewer <see cref="IDisplaySet"/>s than <see cref="IImageBox"/>es,
	/// the remaining <see cref="IImageBox"/>es will be left empty.
	/// Conversely, if there are more <see cref="IDisplaySet"/>s than
	/// <see cref="IImageBox"/>es, the remaining <see cref="IDisplaySet"/>s
	/// will not be shown.
	/// </remarks>
	public static class SimplePhysicalWorkspaceFiller
	{
		/// <summary>
		/// Performs the <see cref="IPhysicalWorkspace"/> filling operation.
		/// </summary>
		/// <param name="imageViewer"></param>
		public static void Fill(IImageViewer imageViewer)
		{
			IPhysicalWorkspace physicalWorkspace = imageViewer.PhysicalWorkspace;
			ILogicalWorkspace logicalWorkspace = imageViewer.LogicalWorkspace;

			int imageSetIndex = 0;
			int displaySetIndex = 0;

			foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
			{
				if (displaySetIndex == logicalWorkspace.ImageSets[imageSetIndex].DisplaySets.Count)
				{
					imageSetIndex++;
					displaySetIndex = 0;

					if (imageSetIndex == logicalWorkspace.ImageSets.Count)
						break;
				}

				imageBox.DisplaySet = logicalWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex];
				displaySetIndex++;
			}
		}
	}
}
