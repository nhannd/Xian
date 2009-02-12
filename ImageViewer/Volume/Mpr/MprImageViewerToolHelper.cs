using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal class MprImageViewerToolHelper
	{
		public readonly IImageViewerToolContext _context;

		public MprImageViewerToolHelper(IImageViewerToolContext context)
		{
			_context = context;
		}

		public MprLayoutManager GetMprLayoutManager()
		{
			if (_context.Viewer is MprImageViewerComponent)
				return ((MprImageViewerComponent)_context.Viewer).LayoutManager as MprLayoutManager;

			return null;
		}

		public void GetObliqueRotationAngles(out int rotationX, out int rotationY, out int rotationZ)
		{
			rotationX = 0;
			rotationY = 0;
			rotationZ = 0;

			IPresentationImage obliqueImage = GetObliqueImage();
			MprLayoutManager layoutManager = GetMprLayoutManager();

			if (obliqueImage != null && layoutManager != null)
			{
				rotationX = layoutManager.GetObliqueImageRotationX(obliqueImage);
				rotationY = layoutManager.GetObliqueImageRotationY(obliqueImage);
				rotationZ = layoutManager.GetObliqueImageRotationZ(obliqueImage);
			}
		}

		public IPresentationImage GetObliqueImage()
		{
			return _context.Viewer.PhysicalWorkspace.ImageBoxes[3].TopLeftPresentationImage;
		}

		public bool IsAxialImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetAxialImageBox();
		}

		public bool IsCoronalImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetCoronalImageBox();
		}

		public bool IsSaggittalImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetSagittalImageBox();
		}

		public IImageBox GetSagittalImageBox()
		{
			IPhysicalWorkspace workspace = _context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[0];
		}

		public IImageBox GetCoronalImageBox()
		{
			IPhysicalWorkspace workspace = _context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[1];
		}

		public IImageBox GetAxialImageBox()
		{
			IPhysicalWorkspace workspace = _context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[2];
		}


	}
}
