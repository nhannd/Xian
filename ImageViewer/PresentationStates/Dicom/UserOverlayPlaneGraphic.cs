using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable(true)]
	public class UserOverlayPlaneGraphic : OverlayPlaneGraphic
	{
		private readonly int _rows;
		private readonly int _columns;

		public UserOverlayPlaneGraphic(int rows, int columns) : base(rows, columns)
		{
			_rows = rows;
			_columns = columns;
		}

		public new string Label
		{
			get { return base.Label; }
			set { base.Label = value; }
		}

		public new string Description
		{
			get { return base.Description; }
			set { base.Description = value; }
		}

		public new OverlayType Type
		{
			get { return base.Type; }
			set { base.Type = value; }
		}

		public new OverlaySubtype Subtype
		{
			get { return base.Subtype; }
			set { base.Subtype = value; }
		}

		public new PointF Origin
		{
			get { return base.Origin; }
			set { base.Origin = value; }
		}

		public bool this[Point point]
		{
			get { return this[point.X, point.Y]; }
			set { this[point.X, point.Y] = value; }
		}

		public bool this[int x, int y]
		{
			get { return base.OverlayPixelData[y*_columns + x] > 0; }
			set { base.OverlayPixelData[y*_columns + x] = value ? (byte) 0xFF : (byte) 0x00; }
		}
	}
}