using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A lookup table.
	/// </summary>
	public abstract class Lut : ILut
	{
		private event EventHandler _lutChanged;

		protected virtual void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		#region ILUT Members

		public abstract int MinInputValue { get; set; }

		public abstract int MaxInputValue { get; set; }

		public abstract int MinOutputValue { get; protected set;}

		public abstract int MaxOutputValue { get; protected set;}

		public abstract int this[int index] { get; protected set; }

		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		public abstract string GetKey();

		public abstract string GetDescription();

		#endregion

		#region IMemorable Members

		public virtual IMemento CreateMemento()
		{
			return null;
		}

		public virtual void SetMemento(IMemento memento)
		{
		}

		#endregion
	}
}
