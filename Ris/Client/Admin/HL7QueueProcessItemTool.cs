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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("process", "hl7queue-contextmenu/Process Message", "Process")]
    [EnabledStateObserver("process", "Enabled", "EnabledChanged")]
    [Tooltip("process", "Process Message")]
	[IconSet("process", IconScheme.Colour, "ProcessMessageToolSmall.png", "ProcessMessageToolMedium.png", "ProcessMessageToolLarge.png")]

    [ExtensionOf(typeof(HL7QueueToolExtensionPoint))]
    public class HL7QueueProcessItemTool : Tool<IHL7QueueToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChanged;

        public override void Initialize()
        {
            base.Initialize();
            this.Context.DefaultAction = Process;
            this.Context.SelectedHL7QueueItemChanged += delegate(object sender, EventArgs args)
            {
                this.Enabled = (this.Context.SelectedHL7QueueItem != null);
            };
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        public void Process()
        {
            //TODO:  Wait cursor
            ProcessQueueItem(this.Context.SelectedHL7QueueItem);
            this.Context.Refresh();
        }

        private void ProcessQueueItem(HL7QueueItemDetail selectedQueueItem)
        {
            try
            {
                Platform.GetService<IHL7QueueService>(
                    delegate(IHL7QueueService service)
                    {
                        ProcessHL7QueueItemRequest processRequest = new ProcessHL7QueueItemRequest(selectedQueueItem.QueueItemRef);
                        service.ProcessHL7QueueItem(processRequest);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Context.DesktopWindow);
            }
        }
    }
}
