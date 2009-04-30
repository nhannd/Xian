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
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for the <see cref="StatefulCompositeGraphic.StateChanged"/>
	/// event.
	/// </summary>
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private IStatefulGraphic _statefulGraphic;
		private GraphicState _oldState;
		private GraphicState _newState;
		private IMouseInformation _mouseInformation;

		internal GraphicStateChangedEventArgs()
		{

		}

		/// <summary>
		/// Gets the <see cref="IStatefulGraphic"/>.
		/// </summary>
		public IStatefulGraphic StatefulGraphic
		{
			get { return _statefulGraphic; }
			internal set { _statefulGraphic = value; }
		}

		/// <summary>
		/// Gets the old <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState OldState
		{
			get { return _oldState; }
			internal set { _oldState = value; }
		}

		/// <summary>
		/// Gets the new <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState NewState
		{
			get { return _newState; }
			internal set 
			{
				Platform.CheckForNullReference(value, "NewState");
				_newState = value; 
			}
		}

		/// <summary>
		/// Gets the <see cref="IMouseInformation"/>.
		/// </summary>
		public IMouseInformation MouseInformation
		{
			get { return _mouseInformation; }
			internal set { _mouseInformation = value; }
		}
	}
}
