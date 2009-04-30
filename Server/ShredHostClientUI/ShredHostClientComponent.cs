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
using System.ServiceProcess;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Server.ShredHost;
using System.Threading;

namespace ClearCanvas.Server.ShredHostClientUI
{
    [ExtensionPoint()]
    public class ShredHostClientToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Extension point for views onto <see cref="ShredHostClientComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ShredHostClientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IShredHostClientToolContext : IToolContext
    {
        Shred SelectedShred { get; }
        void StartShred(Shred shred);
        void StopShred(Shred shred);
    }
	
    /// <summary>
    /// ShredHostClientComponent class
    /// </summary>
    [AssociateView(typeof(ShredHostClientComponentViewExtensionPoint))]
    public class ShredHostClientComponent : ApplicationComponent
    {
        public class ShredHostClientToolContext : ToolContext, IShredHostClientToolContext
        {

            public ShredHostClientToolContext(ShredHostClientComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component;
            }

            #region IShredHostClientToolContext Members

            public Shred SelectedShred
            {
                get 
                {
                    Shred selectedShred = _component.TableSelection.Item as Shred;
                    return selectedShred;
                }
            }

            public void StartShred(Shred shred)
            {
                _component.StartShred(shred);
            }

            public void StopShred(Shred shred)
            {
                _component.StopShred(shred);
            }

            #endregion

            #region Private fields
            ShredHostClientComponent _component;
            #endregion
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ShredHostClientComponent()
        {

        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();

            // initialize the list object that will display the shreds
            _shredCollection = new Table<Shred>();
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Shreds",
                delegate(Shred shred) { return shred.Name; }
                ));
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Status",
                delegate(Shred shred) { return (shred.IsRunning) ? "Running" : "Stopped"; }
                ));

            // refresh the state of the Shred Host and shreds
            Refresh();

            _toolSet = new ToolSet(new ShredHostClientToolExtensionPoint(), new ShredHostClientToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-contextmenu", _toolSet.Actions);

            _refreshTask = new BackgroundTask(delegate(IBackgroundTaskContext context)
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(2000);
                    Refresh();
                }
            },true
            );

            _refreshTask.Run();

        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            _toolSet.Dispose();
            _toolSet = null;

            // signal the refresh thread to stop
            _refreshTask.RequestCancel();

            base.Stop();
        }

        public void Toggle()
        {
            ServiceController controller = GetShredHostServiceController();
            switch (controller.Status)
            {
                case ServiceControllerStatus.Running:
                    controller.Stop();
                    _shredCollection.Items.Clear();
                    return;
                case ServiceControllerStatus.Stopped:
                    controller.Start();
                    return;
                default:
                    return;
            }
        }

        private void Refresh()
        {
            // poll to see if ShredHost is running
            ServiceController controller = GetShredHostServiceController();
            bool newStatus = (controller.Status == ServiceControllerStatus.Running);
            if (this.IsShredHostRunning != newStatus)
                this.IsShredHostRunning = newStatus;

            // if the shred host is not running, the WCF service will not be reachable.
            if (!this.IsShredHostRunning)
                return;

            WcfDataShred[] shreds;
            using (ShredHostClient client = new ShredHostClient())
            {
                shreds = client.GetShreds();
            }

            foreach (WcfDataShred shred in shreds)
            {
                int matchIndex = _shredCollection.Items.FindIndex(delegate(Shred otherShred)
                {
                    return otherShred.Id == shred._id;
                });

                // this is a new shred being reported from Shred Host, add it to our list,
                // usually the case if we are refreshing for the first time
                if (-1 == matchIndex)
                {
                    _shredCollection.Items.Add(new Shred(shred._id, shred._name, shred._description, shred._isRunning));
                }
                else
                {
                    if (_shredCollection.Items[matchIndex].IsRunning != shred._isRunning)
                        _shredCollection.Items[matchIndex].IsRunning = shred._isRunning;
                }
            }
        }

        public void StartShred(Shred shred)
        {
            bool isRunning;
            using (ShredHostClient client = new ShredHostClient())
            {
                isRunning = client.StartShred(shred.GetWcfDataShred());
            }

            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = isRunning;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        public void StopShred(Shred shred)
        {
            bool isRunning;
            using (ShredHostClient client = new ShredHostClient())
            {
                isRunning = client.StopShred(shred.GetWcfDataShred());
            }

            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = isRunning;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        public ServiceController GetShredHostServiceController()
        {
            return new ServiceController("ClearCanvas Shred Host Service");
        }

        #region Properties
        private bool _isShredHostRunning;
        private ISelection _tableSelection;
        private Table<Shred> _shredCollection;

        public ISelection TableSelection
        {
            get { return _tableSelection; }
            set
            {
                if (_tableSelection != value)
                {
                    _tableSelection = value;
                    NotifyPropertyChanged("TableSelection");
                }
            }
        }

        public bool IsShredHostRunning
        {
            get { return _isShredHostRunning; }
            set
            {
                _isShredHostRunning = value;
                NotifyPropertyChanged("IsShredHostRunning");
            }
        }

        public Table<Shred> ShredCollection
        {
            get { return _shredCollection; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
        }

        public ActionModelRoot ContextMenuModel
        {
            get { return _contextMenuModel; }
        }
        #endregion

        #region Private fields
        ToolSet _toolSet;
        ActionModelRoot _toolbarModel;
        ActionModelRoot _contextMenuModel;
        BackgroundTask _refreshTask;
        #endregion
    }
}
