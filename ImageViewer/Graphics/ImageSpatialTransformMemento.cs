#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class ImageSpatialTransformMemento : IEquatable<ImageSpatialTransformMemento>
	{
		private bool _scaleToFit;
		private object _spatialTransformMemento;

		public ImageSpatialTransformMemento(bool scaleToFit, object spatialTransformMemento)
		{
			_scaleToFit = scaleToFit;
			_spatialTransformMemento = spatialTransformMemento;
		}

		public object SpatialTransformMemento
		{
			get { return _spatialTransformMemento; }
		}

		public bool ScaleToFit
		{
			get { return _scaleToFit; }
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			return this.Equals(obj as ImageSpatialTransformMemento);
		}

		#region IEquatable<ImageSpatialTransformMemento>

		public bool Equals(ImageSpatialTransformMemento other)
		{
			if (other == null)
				return false;

			return other.ScaleToFit == ScaleToFit && this.SpatialTransformMemento.Equals(other.SpatialTransformMemento);
		}

		#endregion
	}
}
