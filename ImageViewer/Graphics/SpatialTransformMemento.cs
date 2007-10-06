using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class SpatialTransformMemento : IMemento
	{
		private float _scale;
		private float _translationX;
		private float _translationY;
		private int _rotationXY;
		private bool _flipX;
		private bool _flipY;

		public SpatialTransformMemento()
		{
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

		public bool FlipX
		{
			get { return _flipX; }
			set { _flipX = value; }
		}

		public bool FlipY
		{
			get { return _flipY; }
			set { _flipY = value; }
		}

		public int RotationXY
		{
			get { return _rotationXY; }
			set { _rotationXY = value; }
		}

		public override bool Equals(object obj)
		{
			Platform.CheckForNullReference(obj, "obj");
			SpatialTransformMemento memento = obj as SpatialTransformMemento;
			Platform.CheckForInvalidCast(memento, "obj", "SpatialTransformMemento");

			return (this.Scale == memento.Scale &&
					this.TranslationX == memento.TranslationX &&
					this.TranslationY == memento.TranslationY &&
					this.FlipY == memento.FlipY &&
					this.FlipX == memento.FlipX &&
					this.RotationXY == memento.RotationXY);
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
