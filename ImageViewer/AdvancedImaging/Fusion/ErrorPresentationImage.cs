#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal sealed class ErrorPresentationImage : BasicPresentationImage
	{
		private readonly string _errorMessage;

		public ErrorPresentationImage(string errorMessage)
			: base(new GrayscaleImageGraphic(8, 8))
		{
			CompositeImageGraphic.Graphics.Add(new ErrorMessageGraphic {Text = _errorMessage = errorMessage, Color = Color.WhiteSmoke});
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private ErrorPresentationImage(ErrorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			return new AnnotationLayout();
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new ErrorPresentationImage(_errorMessage);
		}

		[Cloneable(true)]
		private class ErrorMessageGraphic : InvariantTextPrimitive
		{
			protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						var clientRectangle = ParentPresentationImage.ClientRectangle;
						Location = new PointF(clientRectangle.Width/2f, clientRectangle.Height/2f);
					}
					finally
					{
						ResetCoordinateSystem();
					}
				}
				base.OnDrawing();
			}
		}
	}
}