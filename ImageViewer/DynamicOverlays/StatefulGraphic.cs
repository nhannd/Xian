using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public abstract class StatefulGraphic : Graphic
	{
		private GraphicState _state;
		private MouseInformation _currentPointerInformation;
		private event EventHandler<GraphicStateChangedEventArgs> _stateChangedEvent;

		public StatefulGraphic()
		{
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

				// Perform any cleanup necessary in the old state
				if (_state != null)
					_state.OnExitState(_currentPointerInformation);

				GraphicStateChangedEventArgs args = new GraphicStateChangedEventArgs();

				// Old state *can* be null, i.e., we're assigning state for the first time,
				// so there isn't an old state.
				args.OldState = _state;

				_state = value;

				args.NewState = _state;

				if (args.OldState != null)
				{
					// Perform any intialization necessary in the new state
					_state.OnEnterState(_currentPointerInformation);

					OnStateChanged(args);
				}
				
				Trace.Write(_state.ToString());
			}
		}

		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				if (base.Selected != value)
				{
					base.Selected = value;

					if (this.Focused)
					{
						if (value)
							this.State = CreateFocusSelectedState();
						else
							this.State = CreateFocusState();
					}
					else
					{
						if (value)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		public override bool Focused
		{
			get
			{
				return base.Focused;
			}
			set
			{
				if (base.Focused != value)
				{
					base.Focused = value;

					if (value)
					{
						if (this.Selected)
							this.State = CreateFocusSelectedState();
						else
							this.State = CreateFocusState();
					}
					else
					{
						if (this.Selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { _stateChangedEvent += value; }
			remove { _stateChangedEvent -= value; }
		}

		public virtual bool SetCursorToken(MouseInformation pointerInformation)
		{
			return false;
		}

		public virtual GraphicState CreateCreateState()
		{
			throw new InvalidOperationException();
		}

		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		public virtual GraphicState CreateFocusState()
		{
			return new FocusGraphicState(this);
		}

		public virtual GraphicState CreateFocusSelectedState()
		{
			return new FocusSelectedGraphicState(this);
		}
		
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);	
		}

		public virtual void OnEnterInactiveState(MouseInformation pointerInformation)
		{
			//this.Color = Color.Yellow;
			//Draw();
		}

		public virtual void OnEnterFocusState(MouseInformation pointerInformation)
		{
			//this.Color = Color.Orange;
			//Draw();
		}

		public virtual void OnEnterFocusSelectedState(MouseInformation pointerInformation)
		{
			//this.Color = Color.Red;
			//Draw();
		}

		public virtual void OnEnterSelectedState(MouseInformation pointerInformation)
		{ 
		}

		public virtual void OnExitInactiveState(MouseInformation pointerInformation)
		{
		}

		public virtual void OnExitFocusState(MouseInformation pointerInformation)
		{
		}

		public virtual void OnExitSelectedState(MouseInformation pointerInformation)
		{
		}

		public virtual void OnExitFocusSelectedState(MouseInformation pointerInformation)
		{ 
		}

		public virtual bool Start(MouseInformation pointerInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_currentPointerInformation = pointerInformation;
			return this.State.Start(pointerInformation);
		}

		public virtual bool Track(MouseInformation pointerInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_currentPointerInformation = pointerInformation;
			return this.State.Track(pointerInformation);
		}

		public virtual bool Stop(MouseInformation pointerInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_currentPointerInformation = pointerInformation;
			return this.State.Stop(pointerInformation);
		}

		public virtual void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			EventsHelper.Fire(_stateChangedEvent, this, e);
		}
	}
}
