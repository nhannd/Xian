using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	[Cloneable(true)]
	public class OverlayPlanesGraphic : CompositeGraphic
	{
		public const string Name = "Dicom Overlay Planes";

		internal OverlayPlanesGraphic()
		{
			base.Name = Name;
		}

		public IEnumerable<OverlayPlaneGraphic> GetOverlayPlanes()
		{
			foreach (IGraphic graphic in Graphics)
			{
				if (graphic is OverlayPlaneGraphic)
					yield return graphic as OverlayPlaneGraphic;
			}
		}
	}
}
