using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ExtensionPoint()]
    public class AENavigatorToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint()]
    public class AENavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IAENavigatorToolContext : IToolContext
    {
        DicomServerTree DicomAEServerTree { get; set;}
        event EventHandler SelectedServerChanged;
        IDesktopWindow DesktopWindow { get; }
        int UpdateType { get; set; }
    }

    [AssociateView(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponent : ApplicationComponent
	{
        public class AENavigatorToolContext : ToolContext, IAENavigatorToolContext
        {
            AENavigatorComponent _component;

            public AENavigatorToolContext(AENavigatorComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component; 
            }

            #region IAENavigatorToolContext Members

            public DicomServerTree DicomAEServerTree
            {
                get { return _component._dicomServerTree; }
                set { _component._dicomServerTree = value; }
            }

            public event EventHandler SelectedServerChanged
            {
                add { _component.SelectedServerChanged += value; }
                remove { _component.SelectedServerChanged -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public int UpdateType
            {
                get { return _component.UpdateType; }
                set { _component.UpdateType = value; }
            }

            #endregion
        }

        #region Fields

        private DicomServerTree _dicomServerTree;
        private event EventHandler _selectedServerChanged;
        private AEServerGroup _selectedServers;
        private int _updateType;
        private ToolSet _toolSet;
        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;

        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My Studies";
        private static String _myServersRoot = "MyServersRoot";
        private static String _myServersXmlFile = "DicomAEServers.xml";

        public DicomServerTree DicomServerTree
        {
            get { return _dicomServerTree; }
            set { _dicomServerTree = value; }
        }

        public AEServerGroup SelectedServers
        {
            get { return _selectedServers; }
            set { _selectedServers = value; }
        }

        public int UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
            set { _toolbarModel = value; }
        }

        public ActionModelRoot ContextMenuModel
        {
            get { return _contextMenuModel; }
            set { _contextMenuModel = value; }
        }

        public static String MyServersTitle
        {
            get { return AENavigatorComponent._myServersTitle; }
        }

        public static String MyDatastoreTitle
        {
            get { return AENavigatorComponent._myDatastoreTitle; }
        }

        public static String MyServersRoot
        {
            get { return AENavigatorComponent._myServersRoot; }
        }

        public static String MyServersXmlFile
        {
            get { return AENavigatorComponent._myServersXmlFile; }
        }

        #endregion

        public AENavigatorComponent()
        {
            _selectedServers = new AEServerGroup();
            _dicomServerTree = new DicomServerTree();
            if (_dicomServerTree.CurrentServer != null && _dicomServerTree.CurrentServer.IsServer)
            {
                _selectedServers.Servers.Add((DicomServer)_dicomServerTree.CurrentServer);
                _selectedServers.Name = _dicomServerTree.CurrentServer.ServerName;
                _selectedServers.GroupID = _dicomServerTree.CurrentServer.ServerPath + "/" + _selectedServers.Name;
            }

        }

        public void SelectChanged(IDicomServer dataNode)
        {
            if (dataNode.IsServer)
            {
                SetSelectedServer((DicomServer)dataNode);
            }
            else
            {
                _selectedServers = new AEServerGroup();
                _selectedServers.Servers = _dicomServerTree.FindChildServers((DicomServerGroup)dataNode);
                _selectedServers.GroupID = dataNode.ServerPath + "/" + dataNode.ServerName;
                _selectedServers.Name = dataNode.ServerName;
                _dicomServerTree.CurrentServer = dataNode;
                EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            }

        }

        public bool NodeMoved(IDicomServer dataNodeParent, IDicomServer dataNodeNew)
        {
            if (dataNodeParent.IsServer || isNodeExists((DicomServerGroup)dataNodeParent, dataNodeNew))
                return false;
            if (dataNodeNew.IsServer)
            {
                _dicomServerTree.CurrentServer = dataNodeNew;
                _dicomServerTree.DeleteDicomServer(true);
                dataNodeNew.ServerPath = dataNodeParent.ServerPath + "/" + dataNodeParent.ServerName;
                ((DicomServerGroup)dataNodeParent).AddChild(dataNodeNew);
                SelectChanged(dataNodeNew);
            }
            else
            {
                DicomServerGroup dsg = (DicomServerGroup)dataNodeNew;
                _dicomServerTree.CurrentServer = dataNodeNew;
                _dicomServerTree.DeleteDicomServer(true);
                _dicomServerTree.RenameDicomServerGroup(dsg, "", dsg.ServerPath, dataNodeParent.ServerPath + "/" + dataNodeParent.ServerName, 1);
                dsg.ServerPath = dataNodeParent.ServerPath + "/" + dataNodeParent.ServerName;
                ((DicomServerGroup)dataNodeParent).AddChild(dsg);
                _dicomServerTree.CurrentServer = dsg;
                SelectChanged(dsg);
            }
            _dicomServerTree.SaveDicomServers();
            return true;
        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        private void SetSelectedServer(DicomServer server)
        {
            _selectedServers = new AEServerGroup();
            _selectedServers.Servers.Add(server);
            _selectedServers.Name = server.ServerName;
            _selectedServers.GroupID = server.ServerPath + "/" + server.ServerName;
            _dicomServerTree.CurrentServer = server;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        private bool isNodeExists(DicomServerGroup dataNodeParent, IDicomServer dataNodeNew)
        {
            if (dataNodeNew.ServerPath.Equals(dataNodeParent.ServerPath + "/" + dataNodeParent.ServerName))
                return true;
            foreach (IDicomServer ids in dataNodeParent.ChildServers)
            {
                if (ids.ServerName.Equals(dataNodeNew.ServerName))
                    return true;
            }
            return false;
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new AENavigatorToolExtensionPoint(), new AENavigatorToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomaenavigator-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomaenavigator-contextmenu", _toolSet.Actions);
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

    }

    public enum ServerUpdateType
    {
        Add,
        Edit,
        Delete
    }
}
