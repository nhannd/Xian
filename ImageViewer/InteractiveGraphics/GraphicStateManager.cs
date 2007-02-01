using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Diagnostics;

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
