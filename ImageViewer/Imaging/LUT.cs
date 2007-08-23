using System;
using ClearCanvas.Common;
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

		protected void OnLutChanged()
		{
			EventsHelper.Fire(_lutChanged, this, EventArgs.Empty);
		}

		public abstract LutCreationParameters GetCreationParametersMemento();
		public abstract bool TrySetCreationParametersMemento(LutCreationParameters creationParameters);
		
		#region ILUT Members

		public abstract int this[int index] { get; protected set; }

		public event EventHandler LutChanged
		{
			add { _lutChanged += value; }
			remove { _lutChanged -= value; }
		}

		public abstract int MinInputValue { get; }

		public abstract int MaxInputValue { get; }
		
		public abstract int MinOutputValue { get; }

		public abstract int MaxOutputValue { get; }

		public abstract string GetKey();
		
		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is ILut)
				return this.Equals(obj as ILut);
			if (obj is LutCreationParameters)
				return this.Equals(obj as LutCreationParameters);
			
			return false;
		}

		public override int GetHashCode()
		{
			return this.GetKey().GetHashCode();
		}

		#region IEquatable<LutCreationParameters> Members

		public bool Equals(LutCreationParameters other)
		{
			return this.GetKey() == other.GetKey();
		}

		#endregion

		#region IEquatable<ILut> Members

		public bool Equals(ILut other)
		{
			return this.GetKey() == other.GetKey();
		}

		#endregion
	}
}
