using System;
using System.Collections.Generic;
using System.Text;

using Gtk;
using ClearCanvas.Common;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop.Actions;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class ActiveToolbarButton : ToggleToolButton
    {
        private IClickAction _action;
		private bool _clickPending;
		private bool _ignore;
		
        private EventHandler _actionEnabledChangedHandler;
        private EventHandler _actionCheckedChangedHandler;

        public ActiveToolbarButton(IClickAction action, Tooltips tooltips)
        {
            _action = action;

            _actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
            _actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);

            _action.EnabledChanged += _actionEnabledChangedHandler;
            _action.CheckedChanged += _actionCheckedChangedHandler;

            this.Sensitive = _action.Enabled;
            this.Active = _action.IsCheckAction && _action.Checked;
			this.SetTooltip(tooltips, _action.Tooltip, "");

            this.Clicked += OnActivated;
			
            if (_action.IconSet != null)
            {
                try
                {
					// load the tool's icon from it's assembly
					System.Reflection.Assembly asm = _action.Target.GetType().Assembly;
					Image icon = new Image(asm, asm.GetName().Name + "." + _action.IconSet.LargeIcon);
//					icon.Pixbuf = icon.Pixbuf.AddAlpha(true, 255, 0, 0);	// make "red" transparent
					this.IconWidget = icon;
                }
                catch (Exception e)
                {
                    // TODO the icon was either null or not found - log some helpful message
                }
            }

        }
		
		private void OnActivated(object sender, EventArgs e)
		{
			// should the item have toggle behaviour or not?
			if(_action.IsCheckAction)
			{
				// it should behave as a toggle
				// however, because gtk fires the activated event even when the item
				// is programmatically de-activated, these need to be filtered out
				if(!_ignore)
					_action.Click();
				_ignore = false;
			}
			else
			{
				// it should not behave as a toggle
				// this is a hack to workaround the automatic toggle behaviour
				if(_clickPending) {
					_clickPending = false;
					_action.Click();
				} else {
					_clickPending = true;
					this.Active = false;
				}
			}
		}
		
        private void OnActionCheckedChanged(object sender, EventArgs e)
        {
			// however, because gtk fires the activated event even when the item
			// is programmatically de-activated, these need to be filtered out
			if(!_action.IsCheckAction)
				return;
			
			if(_action.Checked != this.Active)
			{
				_ignore = true;
				this.Active = _action.IsCheckAction && _action.Checked;
			}
        }

        private void OnActionEnabledChanged(object sender, EventArgs e)
        {
            this.Sensitive = _action.Enabled;
        }

        public override void Dispose()
        {
			if (_action != null)
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
            base.Dispose();
        }
    }
}
