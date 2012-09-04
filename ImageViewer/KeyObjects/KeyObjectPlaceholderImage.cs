#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.KeyObjects
{
	[Cloneable]
	public class KeyObjectPlaceholderImage : GrayscalePresentationImage
	{
		private readonly string _reason;

		public KeyObjectPlaceholderImage(string reason) : base(5, 5)
		{
			InvariantTextPrimitive textGraphic = new InvariantTextPrimitive(_reason = reason);
			textGraphic.Color = Color.WhiteSmoke;
			base.ApplicationGraphics.Add(textGraphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected KeyObjectPlaceholderImage(KeyObjectPlaceholderImage source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override void OnDrawing()
		{
			// upon drawing, re-centre the text
			RectangleF bounds = base.ClientRectangle;
			PointF anchor = new PointF(bounds.Left + bounds.Width/2, bounds.Top + bounds.Height/2);
			InvariantTextPrimitive textGraphic = (InvariantTextPrimitive) base.ApplicationGraphics.FirstOrDefault(IsType<InvariantTextPrimitive>);
			textGraphic.CoordinateSystem = CoordinateSystem.Destination;
			textGraphic.Location = anchor;
			textGraphic.ResetCoordinateSystem();
			base.OnDrawing();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new KeyObjectPlaceholderImage(_reason);
		}

		private static bool IsType<T>(object test)
		{
			return test is T;
		}
	}
}