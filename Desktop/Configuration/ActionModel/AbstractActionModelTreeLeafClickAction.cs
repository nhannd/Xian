#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public class AbstractActionModelTreeLeafClickAction : AbstractActionModelTreeLeafAction
	{
		private XKeys _keyStroke = XKeys.None;

		public AbstractActionModelTreeLeafClickAction(IClickAction clickAction) : base(clickAction)
		{
			_keyStroke = clickAction.KeyStroke;
		}

		protected new IClickAction Action
		{
			get { return (IClickAction) base.Action; }
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set
			{
				if (!this.RequestValidation("KeyStroke", value))
					return;

				if (_keyStroke != value)
				{
					_keyStroke = value;
					this.NotifyValidated("KeyStroke", value);
					this.OnKeyStrokeChanged();
				}
			}
		}

		public bool IsValidKeyStroke(XKeys keyStroke)
		{
			return this.RequestValidation("KeyStrokePreview", keyStroke);
		}

		protected virtual void OnKeyStrokeChanged()
		{
			this.NotifyItemChanged();

			this.Action.KeyStroke = _keyStroke;
		}
	}
}