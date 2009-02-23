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

		public bool IsMprImage(IPresentationImage image)
		{
			return image.ParentDisplaySet is MprDisplaySet;
		}

		public bool IsIdentityImage(IPresentationImage image)
		{
			return IsMprImage(image, DisplaySetIdentifier.Identity);
		}

		public bool IsOrthoXImage(IPresentationImage image)
		{
			return IsMprImage(image, DisplaySetIdentifier.OrthoX);
		}

		public bool IsOrthoYImage(IPresentationImage image)
		{
			return IsMprImage(image, DisplaySetIdentifier.OrthoY);
		}

		public bool IsObliqueImage(IPresentationImage image)
		{
			return IsMprImage(image, DisplaySetIdentifier.Oblique);
		}
		
		public MprDisplaySet GetIdentityDisplaySet()
		{
			return FindMprDisplaySet(DisplaySetIdentifier.Identity);
		}

		public MprDisplaySet GetOrthoYDisplaySet()
		{
			return FindMprDisplaySet(DisplaySetIdentifier.OrthoY);
		}

		public MprDisplaySet GetOrthoXDisplaySet()
		{
			return FindMprDisplaySet(DisplaySetIdentifier.OrthoX);
		}

		public MprDisplaySet GetObliqueDisplaySet()
		{
			return FindMprDisplaySet(DisplaySetIdentifier.Oblique);
		}

		private static bool IsMprImage(IPresentationImage image, DisplaySetIdentifier identifier)
		{
			if (image == null || image.ParentDisplaySet == null)
				return false;

			return image.ParentDisplaySet is MprDisplaySet && ((MprDisplaySet)image.ParentDisplaySet).Identifier == identifier;
		}

		private MprDisplaySet FindMprDisplaySet(DisplaySetIdentifier identifier)
		{
			IPhysicalWorkspace workspace = _context.Viewer.PhysicalWorkspace;
			foreach (IImageBox imageBox in workspace.ImageBoxes)
			{
				MprDisplaySet displaySet = imageBox.DisplaySet as MprDisplaySet;
				if (displaySet != null && displaySet.Identifier == identifier)
					return displaySet;
			}

			return null;
		}
	}
}
