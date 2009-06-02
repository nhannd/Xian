using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// A dynamically created 1-bit bitmap overlay plane <see cref="IGraphic"/>.
	/// </summary>
	[Cloneable]
	public class UserOverlayPlaneGraphic : OverlayPlaneGraphic
	{
		private readonly int _rows;
		private readonly int _columns;

		/// <summary>
		/// Constructs an empty <see cref="UserOverlayPlaneGraphic"/> with the specified image dimensions.
		/// </summary>
		/// <param name="rows">The number of rows to allocate to the overlay.</param>
		/// <param name="columns">The number of columns to allocate to the overlay.</param>
		public UserOverlayPlaneGraphic(int rows, int columns) : base(rows, columns)
		{
			_rows = rows;
			_columns = columns;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected UserOverlayPlaneGraphic(UserOverlayPlaneGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets a user-defined text string which may be used to label or name the overlay.
		/// </summary>
		public new string Label
		{
			get { return base.Label; }
			set { base.Label = value; }
		}

		/// <summary>
		/// Gets or sets user-defined comments about the overlay.
		/// </summary>
		public new string Description
		{
			get { return base.Description; }
			set { base.Description = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating the type of content represented by the overlay plane.
		/// </summary>
		public new OverlayType Type
		{
			get { return base.Type; }
			set { base.Type = value; }
		}

		/// <summary>
		/// Gets or sets a value identifying the intended purpose of the overlay.
		/// </summary>
		public new OverlaySubtype Subtype
		{
			get { return base.Subtype; }
			set { base.Subtype = value; }
		}

		/// <summary>
		/// Gets or sets the location of the top left corner of the overlay relative to the image.
		/// </summary>
		public new PointF Origin
		{
			get { return base.Origin; }
			set { base.Origin = value; }
		}

		/// <summary>
		/// Gets or sets the presence of the overlay pixel at the specified coordinate.
		/// </summary>
		/// <param name="point">The coordinate of the pixel.</param>
		public bool this[Point point]
		{
			get { return this[point.X, point.Y]; }
			set { this[point.X, point.Y] = value; }
		}

		/// <summary>
		/// Gets or sets the presence of the overlay pixel at the specified coordinate.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel.</param>
		/// <param name="y">The y-coordinate of the pixel.</param>
		public bool this[int x, int y]
		{
			get { return base.OverlayPixelData[y*_columns + x] > 0; }
			set { base.OverlayPixelData[y*_columns + x] = value ? (byte) 0xFF : (byte) 0x00; }
		}
	}
}