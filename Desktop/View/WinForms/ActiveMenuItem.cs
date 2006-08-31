using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ActiveMenuItem : ToolStripMenuItem
    {
        private IClickAction _action;
        private EventHandler _actionEnabledChangedHandler;
        private EventHandler _actionCheckedChangedHandler;

        public ActiveMenuItem(IClickAction action)
        {
            _action = action;

            _actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
            _actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);

            _action.EnabledChanged += _actionEnabledChangedHandler;
            _action.CheckedChanged += _actionCheckedChangedHandler;

            this.Text = _action.Label;
            this.Enabled = _action.Enabled;
            this.Checked = _action.Checked;
			//this.AutoSize = false;
			//this.ImageScaling = ToolStripItemImageScaling.None;

            this.Click += delegate(object sender, EventArgs e)
            {
                _action.Click();
            };

            if (_action.IconSet != null && _action.ResourceResolver != null)
            {
                try
                {
                    this.Image = IconFactory.CreateIcon(_action.IconSet.MediumIcon, _action.ResourceResolver);
                }
                catch (Exception e)
                {
                    // TODO the icon was either null or not found - log some helpful message 
                    throw e;
                }
            }
        }

        private void OnActionCheckedChanged(object sender, EventArgs e)
        {
            this.Checked = _action.Checked;
        }

        private void OnActionEnabledChanged(object sender, EventArgs e)
        {
            this.Enabled = _action.Enabled;
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

                _action = null;
            }
            base.Dispose(disposing);
        }
        
    }
}
