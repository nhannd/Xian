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
	[Cloneable]
	public sealed class NeutralColorLinearLut : ComposableLut, IVoiLutLinear
	{
		private int _channelBitDepth;
		private double _windowWidth;
		private double _windowCentre;
		private int _minInputValue;
		private int _maxInputValue;
		private int _minOutputValue;
		private int _maxOutputValue;

		public NeutralColorLinearLut()
		{
			_channelBitDepth = 8;
			_minInputValue = _minOutputValue = 0;
			_maxInputValue = _maxOutputValue = 255;
			_windowWidth = 256;
			_windowCentre = 128;
		}

		public NeutralColorLinearLut(int channelBitDepth)
		{
			Platform.CheckArgumentRange(channelBitDepth, 1, 256, "channelBitDepth");
			_channelBitDepth = channelBitDepth;
			_minInputValue = _minOutputValue = 0;
			_maxInputValue = _maxOutputValue = (1 << channelBitDepth) - 1;
			_windowWidth = 1 << channelBitDepth;
			_windowCentre = 1 << (channelBitDepth - 1);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		private NeutralColorLinearLut(NeutralColorLinearLut source, ICloningContext context) : base()
		{
			context.CloneFields(source, this);
		}

		public override string GetDescription()
		{
			return string.Format(SR.FormatDescriptionNeutralColorLinearLut, WindowWidth, WindowCenter);
		}

		public override int MinInputValue
		{
			get { return _minInputValue; }
			set { _minInputValue = value; }
		}

		public override int MaxInputValue
		{
			get { return _maxInputValue; }
			set { _maxInputValue = value; }
		}

		public override int MinOutputValue
		{
			get { return _minOutputValue; }
			protected set { _minOutputValue = value; }
		}

		public override int MaxOutputValue
		{
			get { return _maxOutputValue; }
			protected set { _maxOutputValue = value; }
		}

		public override int this[int index]
		{
			get { return index; }
			protected set { throw new NotSupportedException(); }
		}

		public override string GetKey()
		{
			return string.Format("NEUTRAL_{0}BIT", _channelBitDepth);
		}

		#region IVoiLutLinear Members

		public double WindowWidth
		{
			get { return _windowWidth; }
		}

		public double WindowCenter
		{
			get { return _windowCentre; }
		}

		#endregion
	}
}