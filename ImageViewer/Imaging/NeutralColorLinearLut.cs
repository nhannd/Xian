#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A 1-to-1 pass-through composable LUT (i.e. a null transform).
	/// </summary>
	[Cloneable(true)]
	public sealed class NeutralColorLinearLut : ComposableLut, IVoiLutLinear
	{
		private int _minInputValue;
		private int _maxInputValue;

		/// <summary>
		/// Constructs a 1-to-1 pass-through composable LUT for 8-bit unsigned values.
		/// </summary>
		public NeutralColorLinearLut()
		{
			_minInputValue = 0;
			_maxInputValue = 255;
		}

		/// <summary>
		/// Constructs a 1-to-1 pass-through composable LUT for unsigned values.
		/// </summary>
		/// <param name="channelBitDepth">The bit-depth of the unsigned values.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the bit-depth is not between 1 and 31, inclusive.</exception>
		public NeutralColorLinearLut(int channelBitDepth)
		{
			Platform.CheckArgumentRange(channelBitDepth, 1, 31, "channelBitDepth");
			_minInputValue = 0;
			_maxInputValue = (1 << channelBitDepth) - 1;
		}

		/// <summary>
		/// Gets an abbreviated description of the LUT.
		/// </summary>
		public override string GetDescription()
		{
			return string.Format(SR.FormatDescriptionNeutralColorLinearLut, WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		public override int MinInputValue
		{
			get { return _minInputValue; }
			set { _minInputValue = value; }
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		public override int MaxInputValue
		{
			get { return _maxInputValue; }
			set { _maxInputValue = value; }
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <remarks>
		/// Due to the nature of a <see cref="NeutralColorLinearLut"/>, this value is always exactly <see cref="MinInputValue"/>.
		/// </remarks>
		public override int MinOutputValue
		{
			get { return _minInputValue; }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <remarks>
		/// Due to the nature of a <see cref="NeutralColorLinearLut"/>, this value is always exactly <see cref="MaxInputValue"/>.
		/// </remarks>
		public override int MaxOutputValue
		{
			get { return _maxInputValue; }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		/// <remarks>
		/// Due to the nature of a <see cref="NeutralColorLinearLut"/>, the value is always exactly <paramref name="index"/>.
		/// </remarks>
		public override int this[int index]
		{
			get { return index; }
			protected set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the window width.
		/// </summary>
		/// <remarks>
		/// Due to the nature of a <see cref="NeutralColorLinearLut"/>, this value is always exactly <see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1.
		/// </remarks>
		public double WindowWidth
		{
			get { return _maxInputValue - _minInputValue + 1; }
		}

		/// <summary>
		/// Gets the window centre.
		/// </summary>
		/// <remarks>
		/// Due to the nature of a <see cref="NeutralColorLinearLut"/>, this value is always exactly (<see cref="MaxInputValue"/>-<see cref="MinInputValue"/>+1)/2.
		/// </remarks>
		public double WindowCenter
		{
			get { return this.WindowWidth/2; }
		}

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		public override string GetKey()
		{
			return string.Format("NEUTRAL_{0}_to_{1}", _minInputValue, _maxInputValue);
		}
	}
}