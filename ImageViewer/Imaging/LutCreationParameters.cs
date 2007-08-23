using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public abstract class LutCreationParameters : IMemento, IEquatable<LutCreationParameters>, IEquatable<ILut>
	{
		public readonly string FactoryName;

		private int _minInputValue;
		private int _maxInputValue;

		private Dictionary<string, object> _creationParameters;

		protected LutCreationParameters(string factoryName)
		{
			this.FactoryName = factoryName;
			_creationParameters = new Dictionary<string, object>();
		}
		
		private LutCreationParameters()
		{ 
		}
		
		internal int MinInputValue
		{
			get { return _minInputValue; }
			set { _minInputValue = value; }
		}

		internal int MaxInputValue
		{
			get { return _maxInputValue; }
			set { _maxInputValue = value; }
		}

		protected object this[string key]
		{
			get { return _creationParameters[key]; }
			set { _creationParameters[key] = value; }
		}

		public override int GetHashCode()
		{
			return this.GetKey().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			if (obj is LutCreationParameters)
				return this.Equals(obj as LutCreationParameters);
			else if (obj is ILut)
				return this.Equals(obj as ILut);

			return false;
		}

		public abstract string GetKey();

		#region IEquatable<LutCreationParameters> Members

		public bool Equals(LutCreationParameters other)
		{
			return other.GetKey() == this.GetKey();
		}

		#endregion

		#region IEquatable<ILut> Members

		public bool Equals(ILut other)
		{
			return other.GetKey() == this.GetKey();
		}

		#endregion

		public override string ToString()
		{
			return GetKey();
		}
	}
}
