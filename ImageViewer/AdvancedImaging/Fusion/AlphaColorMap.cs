#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class AlphaColorMap : ColorMap
	{
		private readonly IDataLut _baseColorMap;
		private byte _alpha = 255;
		private bool _thresholding;

		public AlphaColorMap(IDataLut baseColorMap)
			: this(baseColorMap, 255, false) {}

		public AlphaColorMap(IDataLut baseColorMap, byte alpha, bool thresholding)
		{
			_baseColorMap = baseColorMap;
			_alpha = alpha;
			_thresholding = thresholding;
		}

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