using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.DicomGraphics
{
	public class AddGeometricShutterUndoableCommand : UndoableCommand
	{
		private readonly GeometricShutter _shutter;
		private readonly GeometricShuttersGraphic _parent;

		public AddGeometricShutterUndoableCommand(GeometricShuttersGraphic parent, GeometricShutter shutter)
		{
			_parent = parent;
			_shutter = shutter;
		}

		public override void Unexecute()
		{
			if (_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Remove(_shutter);
		}

		public override void Execute()
		{
			if (!_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Add(_shutter);
		}
	}

	public class RemoveGeometricShutterUndoableCommand : UndoableCommand
	{
		private readonly GeometricShutter _shutter;
		private readonly GeometricShuttersGraphic _parent;

		public RemoveGeometricShutterUndoableCommand(GeometricShuttersGraphic parent, GeometricShutter shutter)
		{
			_parent = parent;
			_shutter = shutter;
		}

		public override void Unexecute()
		{
			if (!_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Add(_shutter);
		}

		public override void Execute()
		{
			if (_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Remove(_shutter);
		}
	}

	[Cloneable(true)]
	public abstract class GeometricShutter
	{
		internal GeometricShutter()
		{
		}

		internal abstract void AddToGraphicsPath(GraphicsPath path);

		internal GeometricShutter Clone()
		{
			return CloneBuilder.Clone(this) as GeometricShutter;
		}
	}

	[Cloneable(true)]
	public class CircularShutter : GeometricShutter
	{
		public CircularShutter(Point center, int radius)
		{
			this.Center = center;
			this.Radius = radius;
		}

		private CircularShutter()
		{
		}

		public readonly Point Center;
		public readonly int Radius;

		public Rectangle BoundingRectangle
		{
			get
			{
				int x = Center.X - Radius;
				int y = Center.Y - Radius;
				int widthHeight = 2*Radius;
				return new Rectangle(x, y, widthHeight, widthHeight);
			}
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddEllipse(BoundingRectangle);
		}
	}

	[Cloneable(true)]
	public class RectangularShutter : GeometricShutter
	{
		public RectangularShutter(int left, int right, int top, int bottom)
			: this(new Rectangle(left, top, right - left, bottom - top))
		{
		}

		public RectangularShutter(Rectangle rectangle)
		{
			this.Rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
		}

		private RectangularShutter()
		{
		}

		public readonly Rectangle Rectangle;

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddRectangle(Rectangle);
		}
	}

	[Cloneable]
	public class PolygonalShutter : GeometricShutter
	{
		private readonly List<Point> _vertices;
		public readonly ReadOnlyCollection<Point> _readOnlyVertices;

		public PolygonalShutter(IEnumerable<Point> vertices)
		{
			_vertices = new List<Point>(vertices);
			_readOnlyVertices = new ReadOnlyCollection<Point>(_vertices);
		}

		private PolygonalShutter(PolygonalShutter source, ICloningContext context)
			: this(source._vertices)
		{
		}

		public ReadOnlyCollection<Point> Vertices
		{
			get { return _readOnlyVertices; }
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddPolygon(_vertices.ToArray());
		}
	}
	
	[Cloneable]
	public class GeometricShuttersGraphic : CompositeGraphic
	{
		public const string Name = "Geometric Shutters";

		private readonly Rectangle _imageRectangle;
		[CloneIgnore]
		private readonly List<GeometricShutter> _dicomShutters;
		[CloneIgnore]
		private readonly ReadOnlyCollection<GeometricShutter> _readOnlyDicomShutters;
		[CloneIgnore]
		private readonly ObservableList<GeometricShutter> _customShutters;
		[CloneIgnore]
		private ColorImageGraphic _imageGraphic;
		private Color _fillColor = Color.Black;

		public GeometricShuttersGraphic(int rows, int columns)
		{
			_imageRectangle = new Rectangle(0, 0, columns, rows);
			
			_customShutters = new ObservableList<GeometricShutter>();
			_customShutters.ItemAdded += OnCustomShuttersChanged;
			_customShutters.ItemRemoved += OnCustomShuttersChanged;
			_customShutters.ItemChanging += OnCustomShuttersChanged;
			_customShutters.ItemChanged += OnCustomShuttersChanged;

			_dicomShutters = new List<GeometricShutter>();
			_readOnlyDicomShutters = new ReadOnlyCollection<GeometricShutter>(_dicomShutters);
			
			base.Name = Name;
		}

		//for cloning; all the underlying graphics are also cloneable.
		protected GeometricShuttersGraphic(GeometricShuttersGraphic source, ICloningContext context)
			: this(source._imageRectangle.Height, source._imageRectangle.Width)
		{
			context.CloneFields(source, this);
			
			foreach (GeometricShutter shutter in source._customShutters)
				_customShutters.Add(shutter.Clone());

			foreach (GeometricShutter shutter in source._dicomShutters)
				_dicomShutters.Add(shutter.Clone());
		}

		internal void AddDicomShutter(GeometricShutter dicomShutter)
		{
			_dicomShutters.Add(dicomShutter);
			Invalidate();
		}

		private bool HasShutters
		{
			get { return _customShutters.Count > 0 || _dicomShutters.Count > 0; }	
		}

		public ReadOnlyCollection<GeometricShutter> DicomShutters
		{
			get { return _readOnlyDicomShutters; }
		}

		public ObservableList<GeometricShutter> CustomShutters
		{
			get { return _customShutters; }
		}

		public Color FillColor
		{
			get { return _fillColor; }
			set
			{
				if (_fillColor == value)
					return;

				_fillColor = value;
				Invalidate();
			}
		}

		public override void OnDrawing()
		{
			base.OnDrawing();

			RenderImageGraphic();
		}

		private void RenderImageGraphic()
		{
			if (_imageGraphic != null || !HasShutters)
				return;

			int stride = _imageRectangle.Width*4;
			int size = _imageRectangle.Height * stride;
			byte[] buffer = new byte[size];

			GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			try
			{
				using (Bitmap bitmap = new Bitmap(_imageRectangle.Width, _imageRectangle.Height, stride, PixelFormat.Format32bppPArgb, bufferHandle.AddrOfPinnedObject()))
				{
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
					{
						graphics.Clear(Color.FromArgb(0, Color.Black));
						using (Brush brush = new SolidBrush(_fillColor))
						{
							foreach (GeometricShutter shutter in GetAllShutters())
							{
								using (GraphicsPath path = new GraphicsPath())
								{
									path.FillMode = FillMode.Alternate;
									path.AddRectangle(_imageRectangle);
									shutter.AddToGraphicsPath(path);
									path.CloseFigure();
									graphics.FillPath(brush, path);
								}
							}
						}
					}

					//NOTE: we are not doing this properly according to Dicom.  We should be rendering
					//to a 16-bit image so we can set the 16-bit p-value.
					_imageGraphic = new ColorImageGraphic(_imageRectangle.Height, _imageRectangle.Width, buffer);
					base.Graphics.Add(_imageGraphic);
				}
			}
			finally
			{
				bufferHandle.Free();
			}
		}

		private IEnumerable<GeometricShutter> GetAllShutters()
		{
			foreach (GeometricShutter shutter in _dicomShutters)
				yield return shutter;

			foreach (GeometricShutter shutter in _customShutters)
				yield return shutter;
		}

		private void Invalidate()
		{
			if (_imageGraphic != null)
			{
				base.Graphics.Remove(_imageGraphic);
				_imageGraphic.Dispose();
				_imageGraphic = null;
			}
		}

		private void OnCustomShuttersChanged(object sender, ListEventArgs<GeometricShutter> e)
		{
			Invalidate();
		}
	}
}
