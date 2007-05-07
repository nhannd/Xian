using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class AutoVoiLutMemento : IMemento, IEquatable<AutoVoiLutMemento>
	{
		private int _lutIndex;

		public AutoVoiLutMemento(int lutIndex)
		{
			_lutIndex = lutIndex;
		}

		public int LutIndex
		{
			get { return _lutIndex; }
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			AutoVoiLutMemento other = obj as AutoVoiLutMemento;
			if (other == null)
				return false;

			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			return _lutIndex.GetHashCode();
		}

		#region IEquatable<AutoVoiLutMemento> Members

		public bool Equals(AutoVoiLutMemento other)
		{
			if (other == null)
				return false;

			return _lutIndex == other._lutIndex;
		}

		#endregion
	}
}
