#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class AlphaColorMap : ColorMap
	{
		private readonly IColorMap _baseColorMap;
		private byte _alpha = 255;
		private bool _thresholding;

		public AlphaColorMap(IColorMap baseColorMap)
			: this(baseColorMap, 255, false) {}

		public AlphaColorMap(IColorMap baseColorMap, byte alpha, bool thresholding)
		{
			_baseColorMap = baseColorMap;
			_alpha = alpha;
			_thresholding = thresholding;
		}

		//TODO (CR Sept 2010): maybe we should just build this into the base ColorMap class.
		public byte Alpha
		{
			get { return _alpha; }
			set
			{
				if (_alpha != value)
				{
					_alpha = value;
					base.OnLutChanged();
				}
			}
		}

		//TODO (CR Sept 2010): unused?
		public bool Thresholding
		{
			get { return _thresholding; }
			set
			{
				if (_thresholding != value)
				{
					_thresholding = value;
					base.OnLutChanged();
				}
			}
		}

		public override int MinInputValue
		{
			get { return base.MinInputValue; }
			set { base.MinInputValue = _baseColorMap.MinInputValue = value; }
		}

		public override int MaxInputValue
		{
			get { return base.MaxInputValue; }
			set { base.MaxInputValue = _baseColorMap.MaxInputValue = value; }
		}

		/// <summary>
		/// Generates the Lut.
		/// </summary>
		protected override void Create()
		{
			int min = MinInputValue;
			int max = MaxInputValue;
			for (int i = min; i <= max; i++)
				this[i] = (_baseColorMap[i] & 0x00FFFFFF) + (_alpha << 24);
			if (_thresholding)
				this[min] = (_baseColorMap[min] & 0x00FFFFFF);
		}

		public override object CreateMemento()
		{
			return new Memento(base.CreateMemento(), _alpha, _thresholding);
		}

		public override void SetMemento(object memento)
		{
			Memento m = (Memento) memento;
			base.SetMemento(m.Object);
			this.Alpha = m.Alpha;
			this.Thresholding = m.Thresholding;
		}

		public override string GetKey()
		{
			return string.Format("{0}[alpha={1}]", _baseColorMap.GetKey(), _alpha);
		}

		/// <summary>
		/// Returns an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format("{0}[alpha={1}]", _baseColorMap.GetDescription(), _alpha);
		}

		private class Memento
		{
			public readonly object Object;
			public readonly byte Alpha;
			public readonly bool Thresholding;

			public Memento(object @object, byte alpha, bool thresholding)
			{
				this.Object = @object;
				this.Alpha = alpha;
				this.Thresholding = thresholding;
			}

			public override int GetHashCode()
			{
				int code = 0x04634367 ^ this.Alpha.GetHashCode() ^ this.Thresholding.GetHashCode();
				if (this.Object != null)
					code ^= this.Object.GetHashCode();
				return code;
			}

			public override bool Equals(object obj)
			{
				if (obj is Memento)
					return Equals(this.Alpha, ((Memento) obj).Alpha) && Equals(this.Thresholding, ((Memento) obj).Thresholding) && Equals(this.Object, ((Memento) obj).Object);
				return base.Equals(obj);
			}
		}
	}
}