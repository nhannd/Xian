#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A <see cref="StatefulCompositeGraphic"/> that has factory methods
	/// that create standard graphic states.
	/// </summary>
	/// <remarks>
	/// Factory methods can be overridden so that customized graphic states
	/// can be created.
	/// </remarks>
	[Cloneable(true)]
	public abstract class StandardStatefulCompositeGraphic 
		: StatefulCompositeGraphic, IStandardStatefulGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StandardStatefulCompositeGraphic"/>.
		/// </summary>
		protected StandardStatefulCompositeGraphic()
		{
			
		}

		#region IStandardStatefulGraphic Members

		/// <summary>
		/// Creates a new instance of <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="SelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedGraphicState(this);
		}

		#endregion
	}
}
