using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Summary description for SpatialTransformMemento.
	/// </summary>
	public class SpatialTransformMemento : IMemento
	{
		private bool _ScaleToFit;
		private float _Scale;
		private float _TranslationX;
		private float _TranslationY;
		private int _Rotation;
		private bool _FlipHorizontal;
		private bool _FlipVertical;

		public SpatialTransformMemento()
		{
		}

		public bool ScaleToFit
		{
			get
			{
				return _ScaleToFit;
			}
			set
			{
				_ScaleToFit = value;
			}
		}

		public float Scale
		{
			get
			{
				return _Scale;
			}
			set
			{
				_Scale = value;
			}
		}

		public float TranslationX
		{
			get
			{
				return _TranslationX;
			}
			set
			{
				_TranslationX = value;
			}
		}

		public float TranslationY
		{
			get
			{
				return _TranslationY;
			}
			set
			{
				_TranslationY = value;
			}
		}

		public bool FlipHorizontal
		{
			get
			{
				return _FlipHorizontal;
			}
			set
			{
				_FlipHorizontal = value;
			}
		}

		public bool FlipVertical
		{
			get
			{
				return _FlipVertical;
			}
			set
			{
				_FlipVertical = value;
			}
		}

		public int Rotation
		{
			get
			{
				return _Rotation;
			}
			set
			{
				_Rotation = value;
			}
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
