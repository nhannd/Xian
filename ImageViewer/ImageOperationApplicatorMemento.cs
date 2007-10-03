using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageOperationApplicatorMemento : IMemento
	{
		private IList<ImageOriginatorMemento> _imageOriginatorMementos;

		public ImageOperationApplicatorMemento(
			IList<ImageOriginatorMemento> imageOriginatorMementos)
		{
			Platform.CheckForNullReference(imageOriginatorMementos, "imageOriginatorMementos");

			_imageOriginatorMementos = imageOriginatorMementos;
		}

		public IList<ImageOriginatorMemento> ImageOriginatorMementos
		{
			get { return _imageOriginatorMementos; }
		}

		public override bool Equals(object obj)
		{
			Platform.CheckForNullReference(obj, "obj");
			ImageOperationApplicatorMemento imageOperationApplicatorMemento = obj as ImageOperationApplicatorMemento;
			Platform.CheckForInvalidCast(imageOperationApplicatorMemento, "obj", "ImageOperationApplicatorMemento");

			if (this.ImageOriginatorMementos.Count !=
				imageOperationApplicatorMemento.ImageOriginatorMementos.Count)
				return false;

			for (int i = 0; i < this.ImageOriginatorMementos.Count; i++)
			{
				if (!this.ImageOriginatorMementos[i].Memento.Equals(
					imageOperationApplicatorMemento.ImageOriginatorMementos[i].Memento))
					return false;
			}

			return true;
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
