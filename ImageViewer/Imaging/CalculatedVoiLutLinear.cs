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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base functionality for Luts where the <see cref="WindowWidth"/>
	/// and <see cref="WindowCenter"/> are calculated and/or retrieved from an external source.
	/// </summary>
	/// <seealso cref="IVoiLutLinear"/>
	[Cloneable(true)]
	public abstract class CalculatedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected CalculatedVoiLutLinear()
		{
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Gets the <see cref="WindowWidth"/>.
		/// </summary>
		protected sealed override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the <see cref="WindowCenter"/>.
		/// </summary>
		protected sealed override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		#endregion

		#region IVoiLutLinear Members

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public abstract double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public abstract double WindowCenter { get; }

		#endregion
	}
}
