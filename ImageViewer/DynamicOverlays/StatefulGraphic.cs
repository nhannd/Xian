using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model.Layers;

namespace ClearCanvas.Workstation.Model.DynamicOverlays
{
	public abstract class StatefulGraphic : Graphic
	{
		private GraphicState m_State;
		private XMouseEventArgs m_MouseArgs;
		private event EventHandler<GraphicStateChangedEventArgs> m_StateChangedEvent;

		public StatefulGraphic()
		{
		}

		public GraphicState State
		{
			get { return m_State; }
			set
			{
				Platform.CheckForNullReference(value, "State");

				// If it's the same state, then don't do anything
				if (m_State != null)
					if (m_State.GetType() == value.GetType())
						return;

				// Perform any cleanup necessary in the old state
				if (m_State != null)
					m_State.OnExitState(m_MouseArgs);

				GraphicStateChangedEventArgs args = new GraphicStateChangedEventArgs();

				// Old state *can* be null, i.e., we're assigning state for the first time,
				// so there isn't an old state.
				args.OldState = m_State;

				m_State = value;

				args.NewState = m_State;

				if (args.OldState != null)
				{
					// Perform any intialization necessary in the new state
					m_State.OnEnterState(m_MouseArgs);

					OnStateChanged(args);
				}
				
				Trace.Write(m_State.ToString());
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

		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { m_StateChangedEvent += value; }
			remove { m_StateChangedEvent -= value; }
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

			m_MouseArgs = e;
			return this.State.OnMouseDown(e);;
		}

		public override bool OnMouseMove(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			m_MouseArgs = e;
			return this.State.OnMouseMove(e);
		}

		public override bool OnMouseUp(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			Platform.CheckMemberIsSet(this.State, "State");

			m_MouseArgs = e;
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
			EventsHelper.Fire(m_StateChangedEvent, this, e);
		}
	}
}
