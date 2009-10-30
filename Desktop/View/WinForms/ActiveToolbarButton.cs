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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using System.Text;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ActiveToolbarButton : ToolStripButton
    {
        private IClickAction _action;
        private EventHandler _actionEnabledChangedHandler;
        private EventHandler _actionCheckedChangedHandler;
		private EventHandler _actionVisibleChangedHandler;
		private EventHandler _actionLabelChangedHandler;
		private EventHandler _actionTooltipChangedHandler;
		private EventHandler _actionIconSetChangedHandler;

		private IconSize _iconSize;

		public ActiveToolbarButton(IClickAction action)
			: this(action, IconSize.Medium)
		{
		}

		public ActiveToolbarButton(IClickAction action, IconSize iconSize)
        {
            _action = action;

            _actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
            _actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);
			_actionVisibleChangedHandler = new EventHandler(OnActionVisibleChanged);
			_actionLabelChangedHandler = new EventHandler(OnActionLabelChanged);
			_actionTooltipChangedHandler = new EventHandler(OnActionTooltipChanged);
			_actionIconSetChangedHandler = new EventHandler(OnActionIconSetChanged);

            _action.EnabledChanged += _actionEnabledChangedHandler;
            _action.CheckedChanged += _actionCheckedChangedHandler;
			_action.VisibleChanged += _actionVisibleChangedHandler;
			_action.LabelChanged += _actionLabelChangedHandler;
			_action.TooltipChanged += _actionTooltipChangedHandler;
			_action.IconSetChanged += _actionIconSetChangedHandler;

			_iconSize = iconSize;

            this.Text = _action.Label;
            this.Enabled = _action.Enabled;
			SetTooltipText();
			this.Checked = _action.Checked;

            UpdateVisibility();
            UpdateEnablement();
			UpdateIcon();

            this.Click += delegate(object sender, EventArgs e)
            {
                _action.Click();
            };
        }

		public IconSize IconSize
		{
			get { return _iconSize; }
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					UpdateIcon();
				}
			}
		}

        private void OnActionCheckedChanged(object sender, EventArgs e)
        {
            this.Checked = _action.Checked;
        }

        private void OnActionEnabledChanged(object sender, EventArgs e)
        {
			UpdateEnablement();
		}

		private void OnActionVisibleChanged(object sender, EventArgs e)
		{
			UpdateVisibility();
		}

		private void OnActionLabelChanged(object sender, EventArgs e)
		{
			this.Text = _action.Label;
		}

		private void OnActionTooltipChanged(object sender, EventArgs e)
		{
			SetTooltipText();
		}

		private void OnActionIconSetChanged(object sender, EventArgs e)
		{
			UpdateIcon();
		}

		protected override void Dispose(bool disposing)
        {
            if (disposing && _action != null)
            {
                // VERY IMPORTANT: instances of this class will be created and discarded frequently
                // throughout the lifetime of the application
                // therefore is it extremely important that the event handlers are disconnected
                // from the underlying _action events
                // otherwise, this object will hang around for the entire lifetime of the _action object,
                // even though this object is no longer needed
                _action.EnabledChanged -= _actionEnabledChangedHandler;
                _action.CheckedChanged -= _actionCheckedChangedHandler;
				_action.VisibleChanged -= _actionVisibleChangedHandler;
				_action.LabelChanged -= _actionLabelChangedHandler;
				_action.TooltipChanged -= _actionTooltipChangedHandler;
				_action.IconSetChanged -= _actionIconSetChangedHandler;

                _action = null;
            }
            base.Dispose(disposing);
        }

        private void UpdateVisibility()
        {
            this.Visible = _action.Visible && (_action.Permissible || DesktopViewSettings.Default.ShowNonPermissibleActions);
        }

        private void UpdateEnablement()
        {
            this.Enabled = _action.Enabled && (_action.Permissible || DesktopViewSettings.Default.EnableNonPermissibleActions);
        }
	
		private void UpdateIcon()
		{
			if (_action.IconSet != null && _action.ResourceResolver != null)
			{
				try
				{
					Image oldImage = this.Image;

					this.Image = IconFactory.CreateIcon(_action.IconSet[_iconSize], _action.ResourceResolver);
					if (oldImage != null)
						oldImage.Dispose();

					this.Invalidate();
				}
				catch (Exception e)
				{
					// the icon was either null or not found - log some helpful message
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		private void SetTooltipText()
		{
			ToolTipText = GetTooltipText(_action);
		}

		internal static string GetTooltipText(IClickAction action)
		{
			string actionTooltip = action.Tooltip;
			if (string.IsNullOrEmpty(actionTooltip))
				actionTooltip = (action.Label ?? string.Empty).Replace("&", "");

			if (action.KeyStroke == XKeys.None)
				return actionTooltip;

			bool ctrl = (action.KeyStroke & XKeys.Control) == XKeys.Control;
			bool alt = (action.KeyStroke & XKeys.Alt) == XKeys.Alt;
			bool shift = (action.KeyStroke & XKeys.Shift) == XKeys.Shift;
			XKeys keyCode = action.KeyStroke & XKeys.KeyCode;
			
			StringBuilder builder = new StringBuilder();
			builder.Append(actionTooltip);

			if (keyCode != XKeys.None)
			{
				if (builder.Length > 0)
					builder.AppendLine();
				builder.AppendFormat("{0}: ", SR.LabelKeyboardShortcut);
				if (ctrl)
					builder.Append("Ctrl");

				if (alt)
				{
					if (ctrl)
						builder.Append("+");

					builder.Append("Alt");
				}

				if (shift)
				{
					if (ctrl || alt)
						builder.Append("+");
					
					builder.Append("Shift");
				}

				if (ctrl || alt || shift)
					builder.Append("+");

				builder.Append(keyCode);
			}

			return builder.ToString();
		}
	}
}
