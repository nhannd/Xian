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
		private bool m_ScaleToFit;
		private float m_Scale;
		private float m_TranslationX;
		private float m_TranslationY;
		private float m_Rotation;
		private bool m_FlipHorizontal;
		private bool m_FlipVertical;

		public SpatialTransformMemento()
		{
		}

		public bool ScaleToFit
		{
			get
			{
				return m_ScaleToFit;
			}
			set
			{
				m_ScaleToFit = value;
			}
		}

		public float Scale
		{
			get
			{
				return m_Scale;
			}
			set
			{
				m_Scale = value;
			}
		}

		public float TranslationX
		{
			get
			{
				return m_TranslationX;
			}
			set
			{
				m_TranslationX = value;
			}
		}

		public float TranslationY
		{
			get
			{
				return m_TranslationY;
			}
			set
			{
				m_TranslationY = value;
			}
		}

		public bool FlipHorizontal
		{
			get
			{
				return m_FlipHorizontal;
			}
			set
			{
				m_FlipHorizontal = value;
			}
		}

		public bool FlipVertical
		{
			get
			{
				return m_FlipVertical;
			}
			set
			{
				m_FlipVertical = value;
			}
		}

		public float Rotation
		{
			get
			{
				return m_Rotation;
			}
			set
			{
				m_Rotation = value;
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
