#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Specifies the line style to use when drawing the vector.
	/// </summary>
	public enum LineStyle
	{
		/// <summary>
		/// A solid line.
		/// </summary>
		Solid = 0,
		/// <summary>
		/// A dashed line.
		/// </summary>
		Dash = 1,
		/// <summary>
		/// A dotted line.
		/// </summary>
		Dot = 2
	}

	/// <summary>
	/// An vector <see cref="Graphic"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class VectorGraphic : Graphic, IVectorGraphic
	{
		/// <summary>
		/// The hit test distance in destination pixels.
		/// </summary>
		public static readonly int HitTestDistance = 10;
		private Color _color = Color.Yellow;
		private LineStyle _lineStyle = LineStyle.Solid;

		/// <summary>
		/// Initializes a new instance of <see cref="VectorGraphic"/>.
		/// </summary>
		protected VectorGraphic()
		{
		}

		/// <summary>
		/// Gets or sets the colour.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					base.NotifyVisualStateChanged("Color");
				}
			}
		}

		/// <summary>
		/// Gets or sets the line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return _lineStyle; }
			set
			{
				if (_lineStyle != value)
				{
					_lineStyle = value;
					base.NotifyVisualStateChanged("LineStyle");
				}
			}
		}
	}
}
