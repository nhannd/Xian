using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	[Cloneable(true)]
	public class ShuttersGraphic : CompositeGraphic
	{
		public const string Name = "Dicom Shutters";

		internal ShuttersGraphic()
		{
			base.Name = Name;
		}

		public OverlayPlaneGraphic BitmapShutter
		{
			get
			{
				return CollectionUtils.SelectFirst(Graphics,
					delegate(IGraphic graphic)
						{
							return graphic is OverlayPlaneGraphic && ((OverlayPlaneGraphic) graphic).IsBitmapShutter;
						}) as OverlayPlaneGraphic;
			}
		}

		public GeometricShuttersGraphic GeometricShutters
		{
			get
			{
				return CollectionUtils.SelectFirst(Graphics,
					delegate(IGraphic graphic)
					{
						return graphic is GeometricShuttersGraphic;
					}) as GeometricShuttersGraphic;
			}
		}
	}
}
