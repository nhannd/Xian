using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public abstract class StatefulGraphic : Graphic
	{
		private GraphicState _state;
		private XMouseEventArgs _mouseArgs;
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
					_state.OnExitState(_mouseArgs);

				GraphicStateChangedEventArgs args = new GraphicStateChangedEventArgs();

				// Old state *can* be null, i.e., we're assigning state for the first time,
				// so there isn't an old state.
				args.OldState = _state;

				_state = value;

				args.NewState = _state;

				if (args.OldState != null)
				{
					// Perform any intialization necessary in the new state
					_state.OnEnterState(_mouseArgs);

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

					if (value == true)
						this.State = CreateSelectedState();
					else
						this.State = CreateInactiveState();
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
						this.State = CreateFocusState();
					}
					else
					{
						//transition back to selected if the graphic is selected.
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

		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		public virtual void OnEnterInactiveState(XMouseEventArgs e)
		{
			//this.Color = Color.Yellow;
			//Draw();
		}

		public virtual void OnEnterFocusState(XMouseEventArgs e)
		{
			//this.Color = Color.Orange;
			//Draw();
		}

		public virtual void OnEnterSelectedState(XMouseEventArgs e)
		{
			//this.Color = Color.Red;
			//Draw();
		}

		public virtual void OnExitInactiveState(XMouseEventArgs e)
		{
		}

		public virtual void OnExitFocusState(XMouseEventArgs e)
		{
		}

		public virtual void OnExitSelectedState(XMouseEventArgs e)
		{
		}

		#region IUIEventHandler Members

		public override bool OnMouseDown(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			_mouseArgs = e;
			return this.State.OnMouseDown(e);;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			_mouseArgs = e;
			return this.State.OnMouseMove(e);
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			_mouseArgs = e;
			return this.State.OnMouseUp(e);
		}

		public override bool OnMouseWheel(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			return this.State.OnMouseWheel(e);
		}

		public override bool OnKeyDown(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			return this.State.OnKeyDown(e);
		}

		public override bool OnKeyUp(XKeyEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			return this.State.OnKeyUp(e);
		}

		#endregion

		public virtual void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			EventsHelper.Fire(_stateChangedEvent, this, e);
		}
	}
}
