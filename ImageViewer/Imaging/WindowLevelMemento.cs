using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelMemento : IMemento
	{
		private double _windowWidth = 1.0d;
		private double _windowCenter = 0.0d;

		public WindowLevelMemento()
		{

		}

		public double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		public double WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}

		public override bool Equals(object obj)
		{
			Platform.CheckForNullReference(obj, "obj");
			WindowLevelMemento memento = obj as WindowLevelMemento;
			Platform.CheckForInvalidCast(memento, "obj", "WindowLevelMemento");

			return (this.WindowCenter == memento.WindowCenter &&
					this.WindowWidth == memento.WindowWidth);
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
