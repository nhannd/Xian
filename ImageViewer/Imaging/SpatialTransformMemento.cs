using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class SpatialTransformMemento : IMemento
	{
		private bool _scaleToFit;
		private float _scale;
		private float _translationX;
		private float _translationY;
		private int _rotation;
		private bool _flipHorizontal;
		private bool _flipVertical;

		public SpatialTransformMemento()
		{
		}

		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set { _scaleToFit = value; }
		}

		public float Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		public float TranslationX
		{
			get { return _translationX; }
			set { _translationX = value; }
		}

		public float TranslationY
		{
			get { return _translationY; }
			set { _translationY = value; }
		}

		public bool FlipHorizontal
		{
			get { return _flipHorizontal; }
			set { _flipHorizontal = value; }
		}

		public bool FlipVertical
		{
			get { return _flipVertical; }
			set { _flipVertical = value; }
		}

		public int Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}


		public override bool Equals(object obj)
		{
			Platform.CheckForNullReference(obj, "obj");
			SpatialTransformMemento memento = obj as SpatialTransformMemento;
			Platform.CheckForInvalidCast(memento, "obj", "SpatialTransformMemento");

			return (this.Scale == memento.Scale &&
				    this.TranslationX == memento.TranslationX &&
				    this.TranslationY == memento.TranslationY &&
				    this.FlipHorizontal == memento.FlipHorizontal &&
				    this.FlipVertical == memento.FlipVertical &&
				    this.Rotation == memento.Rotation &&
				    this.ScaleToFit == memento.ScaleToFit);
		}

		public override int GetHashCode()
		{
			// Normally, you would calculate a hash code dependent on immutable
			// member fields since we've given this object value type semantics
			// because of how we've overridden Equals().  However, this is a memento
			// and thus the actualy contents of the memento are irrelevant if they 
			// were ever put in a hashtable.  We are in fact interested in the instance.
			return base.GetHashCode();
		}
	}
}
