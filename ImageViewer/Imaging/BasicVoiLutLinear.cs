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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The most basic of Linear Luts where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> can be directly set/manipulated.
	/// </summary>
	/// <seealso cref="IBasicVoiLutLinear"/>
	[Cloneable(true)]
	public sealed class BasicVoiLutLinear : VoiLutLinearBase, IBasicVoiLutLinear
	{
		#region Window/Level Memento class

		private class WindowLevelMemento : IEquatable<WindowLevelMemento>
		{
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public WindowLevelMemento(double windowWidth, double windowCenter)
			{
				WindowWidth = windowWidth;
				WindowCenter = windowCenter;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}	
		
			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is WindowLevelMemento)
					return this.Equals((WindowLevelMemento) obj);

				return false;
			}

			#region IEquatable<WindowLevelMemento> Members

			public bool Equals(WindowLevelMemento other)
			{
				if (other == null)
					return false;

				return this.WindowWidth == other.WindowWidth && this.WindowCenter == other.WindowCenter;
			}

			#endregion
		}

		#endregion

		#region Private Fields

		private double _windowWidth;
		private double _windowCenter;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructor.  
		/// </summary>
		/// <remarks>
		/// Allows the initial <see cref="WindowWidth"/> and <see cref="WindowCenter"/> to be set.
		/// </remarks>
		/// <param name="windowWidth">The initial Window Width.</param>
		/// <param name="windowCenter">The initial Window Center.</param>
		public BasicVoiLutLinear(double windowWidth, double windowCenter)
			: base()
		{
			this.WindowWidth = windowWidth;
			this.WindowCenter = windowCenter;
		}

		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <remarks>
		/// The initial <see cref="WindowWidth"/> and <see cref="WindowCenter"/> are 1 and 0, respectively.
		/// </remarks>
		public BasicVoiLutLinear()
			: this(1, 0)
		{
		}

		#endregion

		#region Protected Methods
		#region Overrides

		/// <summary>
		/// Gets the <see cref="WindowWidth"/>.
		/// </summary>
		protected override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the <see cref="WindowCenter"/>.
		/// </summary>
		protected override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		#endregion
		#endregion

		#region Public Members
		#region Properties

		/// <summary>
		/// Gets or sets the Window Width.
		/// </summary>
		public double WindowWidth
		{
			get { return _windowWidth; }
			set
			{
				if (value == _windowWidth)
					return;

				if (value < 1)
					value = 1;

				_windowWidth = value;
				base.OnLutChanged();
			}
		}

		/// <summary>
		/// Gets or sets the Window Center.
		/// </summary>
		public double WindowCenter
		{
			get { return _windowCenter; }
			set
			{
				if (value == _windowCenter)
					return;

				_windowCenter = value;
				base.OnLutChanged();
			}
		}

		#endregion

		#region Methods
		#region Overrides

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionBasicLinearLut, WindowWidth, WindowCenter);
		}

		/// <summary>
		/// Creates a memento, through which the Lut's state can be restored.
		/// </summary>
		public override object CreateMemento()
		{
			return new WindowLevelMemento(this.WindowWidth, this.WindowCenter);
		}

		/// <summary>
		/// Sets the Lut's state from the input memento object.
		/// </summary>
		/// <exception cref="InvalidCastException">Thrown when the memento is unrecognized, which should never happen.</exception>
		/// <param name="memento">The memento to use to restore a previous state.</param>
		public override void SetMemento(object memento)
		{
			WindowLevelMemento windowLevelMemento = (WindowLevelMemento) memento;

			this.WindowWidth = windowLevelMemento.WindowWidth;
			this.WindowCenter = windowLevelMemento.WindowCenter;
		}

		#endregion
		#endregion
		#endregion
	}
}
