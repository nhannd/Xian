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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class GraphicStateManager
	{
		private GraphicState _state;
		private event EventHandler<GraphicStateChangedEventArgs> _stateChangedEvent;
		private IMouseInformation _mouseInformation;

		public GraphicStateManager()
		{

		}

		public IMouseInformation MouseInformation
		{
			get { return _mouseInformation; }
			set { _mouseInformation = value; }
		}

		public GraphicState State
		{
			get { return _state; }
			set
			{
				Platform.CheckForNullReference(value, "State");

				// If it's the same state, then don't do anything
				if (_state != null)
					if (_state.GetType() == value.GetType())
						return;

				GraphicStateChangedEventArgs args = new GraphicStateChangedEventArgs();

				// Old state *can* be null, i.e., we're assigning state for the first time,
				// so there isn't an old state.
				args.OldState = _state;

				_state = value;

				args.NewState = _state;
				args.MouseInformation = _mouseInformation;
				args.StatefulGraphic = _state.StatefulGraphic;

				// If the old state is null, we're really just initializing the state variable,
				// so don't tell anyone about it
				if (args.OldState != null)
					EventsHelper.Fire(_stateChangedEvent, this, args);

				Trace.Write(_state.ToString());
			}
		}

		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { _stateChangedEvent += value; }
			remove { _stateChangedEvent -= value; }
		}

	}
}
