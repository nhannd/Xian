using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{

    public abstract class AENavigatorTool : Tool<IAENavigatorToolContext>
    {
        private bool _enabled;
        private event EventHandler _enabledChangedEvent;

        public AENavigatorTool()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
                }
            }
        }

        protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

        public event EventHandler EnabledChanged
        {
            add { _enabledChangedEvent += value; }
            remove { _enabledChangedEvent -= value; }
        }
    }
}
