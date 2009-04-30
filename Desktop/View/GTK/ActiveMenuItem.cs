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
using System.Collections.Generic;
using System.Text;

using Gtk;
using ClearCanvas.Common;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop.Actions;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class ActiveMenuItem : CheckMenuItem
    {
        private IClickAction _action;
		private bool _clickPending;
		private bool _ignore;
		
        private EventHandler _actionEnabledChangedHandler;
        private EventHandler _actionCheckedChangedHandler;

        public ActiveMenuItem(IClickAction action)
		:base(action.Label.Replace('&', '_'))
        {
            _action = action;

            _actionEnabledChangedHandler = new EventHandler(OnActionEnabledChanged);
            _actionCheckedChangedHandler = new EventHandler(OnActionCheckedChanged);

            _action.EnabledChanged += _actionEnabledChangedHandler;
            _action.CheckedChanged += _actionCheckedChangedHandler;

            this.Sensitive = _action.Enabled;
            this.Active = _action.IsCheckAction && _action.Checked;

            this.Activated += OnActivated;
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
