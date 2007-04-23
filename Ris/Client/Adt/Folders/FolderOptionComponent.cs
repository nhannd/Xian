using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt.Folders
{
    /// <summary>
    /// Extension point for views onto <see cref="FolderOptionComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderOptionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// FolderOptionComponent class
    /// </summary>
    [AssociateView(typeof(FolderOptionComponentViewExtensionPoint))]
    public class FolderOptionComponent : ApplicationComponent
    {
        private int _refreshTime;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderOptionComponent(int refreshTime)
        {
            _refreshTime = refreshTime;
        }

        public int RefreshTime
        {
            get { return _refreshTime; }
            set 
            { 
                _refreshTime = value;
                this.Modified = true;
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                if (_refreshTime == 0 || _refreshTime > 5000)
                {
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                else
                {
                    Platform.ShowMessageBox("Enter 0 or a number greater than 5000");
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
