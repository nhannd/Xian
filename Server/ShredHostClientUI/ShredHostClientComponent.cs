using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Server.ShredHost;

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

            public ShredHostClient ShredHostClient
            {
                get { return _component.ShredHostProxy; }
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

            // connect to the ShredHost WCF Service
            _shredHostProxy = new ShredHostClient();

            // initialize the list of shreds to be displayed
            _shredCollection = new Table<Shred>();
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Shreds",
                delegate(Shred shred) { return shred.Name; }
                ));
            _shredCollection.Columns.Add(new TableColumn<Shred, string>("Status",
                delegate(Shred shred) { return (shred.IsRunning) ? "Running" : "Stopped"; }
                ));

            WcfDataShred[] shreds = _shredHostProxy.GetShreds();
            foreach (WcfDataShred shred in shreds)
            {
                _shredCollection.Items.Add(new Shred(shred._id, shred._name, shred._description, shred._isRunning));
            }

            // poll to see if ShredHost is running
            _isShredHostRunning = _shredHostProxy.IsShredHostRunning();

            _toolSet = new ToolSet(new ShredHostClientToolExtensionPoint(), new ShredHostClientToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "shredhostclient-contextmenu", _toolSet.Actions);
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            _shredHostProxy.Close();
            _toolSet.Dispose();
            _toolSet = null;

            base.Stop();
        }

        public void Toggle()
        {
            this.IsShredHostRunning = _shredHostProxy.IsShredHostRunning();
            if (this.IsShredHostRunning)
            {
                this.IsShredHostRunning = _shredHostProxy.Stop();
            }
            else
            {
                this.IsShredHostRunning = _shredHostProxy.Start();
            }
        }

        public void StartShred(Shred shred)
        {
            bool newRunningState = this.ShredHostProxy.StartShred(shred.GetWcfDataShred());
            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = newRunningState;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        public void StopShred(Shred shred)
        {
            bool newRunningState = this.ShredHostProxy.StopShred(shred.GetWcfDataShred());
            int indexCurrentShred = this.ShredCollection.Items.FindIndex(delegate(Shred otherShred)
            {
                return otherShred.Id == shred.Id;
            });

            this.ShredCollection.Items[indexCurrentShred].IsRunning = newRunningState;
            this.ShredCollection.Items.NotifyItemUpdated(indexCurrentShred);
        }

        #region Properties
        private bool _isShredHostRunning;
        private ISelection _tableSelection;
        private Table<Shred> _shredCollection;
        ShredHostClient _shredHostProxy;

        public ShredHostClient ShredHostProxy
        {
            get { return _shredHostProxy; }
        }

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
        #endregion
    }
}
