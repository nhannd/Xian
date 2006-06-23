using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model.Imaging
{
	public class ImageOperationApplicatorMemento : IMemento
	{
		private IList<ImageAndOriginator> _linkedImagesAndOriginators;
		private IMemento _memento;

		public ImageOperationApplicatorMemento(
			IList<ImageAndOriginator> linkedImagesAndOriginators,
			IMemento memento)
		{
			Platform.CheckForNullReference(linkedImagesAndOriginators, "linkedImagesAndOriginators");
			Platform.CheckForNullReference(memento, "memento");

			_linkedImagesAndOriginators = linkedImagesAndOriginators;
			_memento = memento;
		}

		public IList<ImageAndOriginator> LinkedImagesAndOriginators
		{
			get { return _linkedImagesAndOriginators; }
		}

		public IMemento Memento
		{
			get { return _memento; }
		}

		public override bool Equals(object obj)
		{
			Platform.CheckForNullReference(obj, "obj");
			ImageOperationApplicatorMemento imageOperationApplicatorMemento = obj as ImageOperationApplicatorMemento;
			Platform.CheckForInvalidCast(imageOperationApplicatorMemento, "obj", "ImageOperationApplicatorMemento");

			return _memento.Equals(imageOperationApplicatorMemento.Memento);
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
