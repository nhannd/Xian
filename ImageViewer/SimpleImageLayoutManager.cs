using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	internal class SimpleImageLayoutManager : ILayoutManager
	{
		public SimpleImageLayoutManager()
		{

		}

		#region ILayoutManager Members

		public void Layout(IImageViewer imageViewer)
		{
			SimpleLogicalWorkspaceBuilder.Build(imageViewer);

			int numDisplaySets = GetNumDisplaySets(imageViewer);

			if (numDisplaySets == 1)
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(1, 1);
			else if (numDisplaySets == 2)
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(1, 2);
			else if (numDisplaySets <= 4)
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 2);
			else if (numDisplaySets <= 8)
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 4);
			else if (numDisplaySets <= 12)
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(3, 4);
			else
				imageViewer.PhysicalWorkspace.SetImageBoxGrid(4, 4);

			foreach (IImageBox imageBox in imageViewer.PhysicalWorkspace.ImageBoxes)
				imageBox.SetTileGrid(1,1);

			SimplePhysicalWorkspaceFiller.Fill(imageViewer);
			imageViewer.PhysicalWorkspace.Draw();
		}

		#endregion

		private int GetNumDisplaySets(IImageViewer imageViewer)
		{
			int count = 0;

			foreach (IImageSet imageSet in imageViewer.LogicalWorkspace.ImageSets)
				count += imageSet.DisplaySets.Count;

			return count;
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
