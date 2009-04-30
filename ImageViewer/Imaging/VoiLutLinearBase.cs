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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing all the base implementation for Linear Voi Luts.
	/// </summary>
	/// <remarks>
	/// A simple Linear Voi Lut class (<see cref="BasicVoiLutLinear"/>) and other 
	/// abstract base classes (<see cref="CalculatedVoiLutLinear"/>, <see cref="AlgorithmCalculatedVoiLutLinear"/>)
	/// are provided that cover most, if not all, Linear Voi Lut use cases.  You should not need
	/// to derive directly from this class.
	/// </remarks>
	/// <seealso cref="ComposableLut"/>
	/// <seealso cref="IComposableLut"/>
	[Cloneable(true)]
	public abstract class VoiLutLinearBase : ComposableLut
	{
		#region Private Fields

		private int _minInputValue;
		private int _maxInputValue;
		private double _windowRegionStart;
		private double _windowRegionEnd;
		private bool _recalculate;

		#endregion

		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected VoiLutLinearBase()
		{
			_recalculate = true;
			_minInputValue = int.MinValue;
			_maxInputValue = int.MaxValue;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		protected abstract double GetWindowWidth();

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		protected abstract double GetWindowCenter();

		#endregion

		#region Overrides
		#region Public Properties

		/// <summary>
		/// Gets the output value of the Lut at a given input <paramref name="index"/>.
		/// </summary>
		public sealed override int this[int index]
		{
			get
			{
				if (_recalculate)
				{
					Calculate();
					_recalculate = false;
				}

				if (index < _windowRegionStart)
				{
					return this.MinOutputValue;
				}
				else if (index > _windowRegionEnd)
				{
					return this.MaxOutputValue;
				}
				else
				{
					double scale = ((index - (this.GetWindowCenter() - 0.5)) / (this.GetWindowWidth() - 1)) + 0.5;
					return (int)((scale * (this.MaxOutputValue - this.MinOutputValue)) + this.MinOutputValue);
				}
			}
			protected set
			{
				throw new InvalidOperationException(SR.ExceptionLinearLutDataCannotBeSet);
			}
		}

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public sealed override int MinInputValue
		{
			get { return _minInputValue; }
			set
			{
				if (_minInputValue == value)
					return;

				_minInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		public sealed override int MaxInputValue
		{
			get { return _maxInputValue; }
			set
			{
				if (_maxInputValue == value)
					return;

				_maxInputValue = value;
				OnLutChanged();
			}
		}

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		/// <exception cref="MemberAccessException">Thrown on any attempt to set the value.</exception>
		public sealed override int MinOutputValue
		{
			get { return _minInputValue; }
			protected set { throw new InvalidOperationException(SR.ExceptionMinimumOutputValueIsNotSettable); }
		}

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		/// <exception cref="MemberAccessException">Thrown on any attempt to set the value.</exception>
		public sealed override int MaxOutputValue
		{
			get { return _maxInputValue; }
			protected set { throw new InvalidOperationException(SR.ExceptionMaximumOutputValueIsNotSettable); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.  The method should simply 
		/// be used to determine if a lut in the <see cref="ComposedLutPool"/> is the same 
		/// as an existing one.
		/// </remarks>
		public sealed override string GetKey()
		{
			return String.Format("{0}_{1}_{2}_{3}",
				this.MinInputValue,
				this.MaxInputValue,
				this.GetWindowWidth(),
				this.GetWindowCenter());
		}

		/// <summary>
		/// Should be called by implementors when the Lut characteristics have changed.
		/// </summary>
		protected override void OnLutChanged()
		{
			_recalculate = true;
			base.OnLutChanged();
		}

		#endregion
		#endregion

		#region Private Methods

		private void Calculate()
		{
			double halfWindow = (this.GetWindowWidth() - 1) / 2;
			_windowRegionStart = this.GetWindowCenter() - 0.5 - halfWindow;
			_windowRegionEnd = this.GetWindowCenter() - 0.5 + halfWindow;
		}
		
		#endregion
	}
}
