using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System.Configuration;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Configuration/Settings")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class SettingsManagementTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    new SettingsManagementComponent(),
                    "Settings Management",
                    delegate(IApplicationComponent c) { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="SettingsManagementComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class SettingsManagementComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// SettingsManagementComponent class
    /// </summary>
    [AssociateView(typeof(SettingsManagementComponentViewExtensionPoint))]
    public class SettingsManagementComponent : ApplicationComponent
    {
        #region SettingsGroupNode class

        class SettingsGroupNode
        {
            private string _nodeName;
            private List<SettingsGroupNode> _childNodes;
            private Type _settingsClass;

            public SettingsGroupNode(string name)
            {
                _nodeName = name;
                _childNodes = new List<SettingsGroupNode>();
            }

            public string Name { get { return _nodeName; } }
            public Type SettingsClass { get { return _settingsClass; } }
            public IList<SettingsGroupNode> ChildNodes { get { return _childNodes; } }

            public void InsertGroup(Type settingsClass)
            {
                InsertGroup(settingsClass, settingsClass.FullName.Split('.'), 0);
            }

            private void InsertGroup(Type settingsClass, string[] pathParts, int depth)
            {
                if (depth == pathParts.Length)
                {
                    _settingsClass = settingsClass;
                }
                else
                {
                    SettingsGroupNode child = CollectionUtils.SelectFirst<SettingsGroupNode>(_childNodes,
                        delegate(SettingsGroupNode c) { return c._nodeName == pathParts[depth]; });
                    if (child == null)
                    {
                        child = new SettingsGroupNode(pathParts[depth]);
                        _childNodes.Add(child);
                    }
                    child.InsertGroup(settingsClass, pathParts, depth + 1);
                }
            }
        }


        #endregion


        #region SettingsProperty class

        public class SettingsProperty
        {
            private PropertyInfo _propertyInfo;
            private string _value;
            private string _startingValue;
            private string _defaultValue;

            public SettingsProperty(PropertyInfo propertyInfo, string value)
            {
                _propertyInfo = propertyInfo;
                _startingValue = _value = value;
                _defaultValue = SettingsClassMetaDataReader.GetDefaultValue(_propertyInfo);
            }

            public string Name
            {
                get { return _propertyInfo.Name; }
            }

            public string TypeName
            {
                get { return _propertyInfo.PropertyType.FullName; }
            }

            public string Description
            {
                get { return SettingsClassMetaDataReader.GetDescription(_propertyInfo); }
            }

            public string Scope
            {
                get { return SettingsClassMetaDataReader.IsAppScoped(_propertyInfo) ? "Application" : "User"; }
            }

            public string DefaultValue
            {
                get { return _defaultValue; }
            }

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public bool UsingDefaultValue
            {
                get { return _value == _defaultValue; }
            }

            public bool Dirty
            {
                get { return _value != _startingValue; }
            }
        }

        #endregion

        private SettingsGroupNode _rootGroupNode;
        private Tree<SettingsGroupNode> _settingsGroupTree;

        private IConfigurationService _service;
        private Table<SettingsProperty> _settingsPropertiesTable;

        private SettingsGroupNode _selectedSettingsGroup;
        private event EventHandler _selectedSettingsGroupChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsManagementComponent()
        {
            // define the structure of the settings group tree
            TreeItemBinding<SettingsGroupNode> settingsGroupTreeBinding = new TreeItemBinding<SettingsGroupNode>();
            settingsGroupTreeBinding.NodeTextProvider = delegate(SettingsGroupNode node) { return node.Name; };
            settingsGroupTreeBinding.SubTreeProvider = delegate(SettingsGroupNode node)
                    {
                        return new Tree<SettingsGroupNode>(settingsGroupTreeBinding, node.ChildNodes);
                    };
            settingsGroupTreeBinding.CanHaveSubTreeHandler = 
                delegate(SettingsGroupNode node) { return node.ChildNodes.Count > 0; };

            _settingsGroupTree = new Tree<SettingsGroupNode>(settingsGroupTreeBinding);
            _settingsGroupTree.Items.Add(_rootGroupNode = new SettingsGroupNode("Groups"));

            // define the settings properties table
            _settingsPropertiesTable = new Table<SettingsProperty>();
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Name",
                delegate(SettingsProperty p) { return p.Name; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Scope",
                delegate(SettingsProperty p) { return p.Scope; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Value",
               delegate(SettingsProperty p) { return p.Value; },
               delegate(SettingsProperty p, string text) { p.Value = text; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Default Value",
                delegate(SettingsProperty p) { return p.DefaultValue; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Type",
                delegate(SettingsProperty p) { return p.TypeName; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Description",
                delegate(SettingsProperty p) { return p.Description; }));



        }

        public override void Start()
        {
            _service = ApplicationContext.GetService<IConfigurationService>();

            List<Type> settingsClasses = new List<Type>();
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                foreach (Type t in plugin.Assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(SettingsBase)))
                    {
                        settingsClasses.Add(t);
                    }
                }
            }

            settingsClasses.Sort(delegate(Type t1, Type t2) { return t1.FullName.CompareTo(t2.FullName); });
            foreach (Type t in settingsClasses)
            {
                _rootGroupNode.InsertGroup(t);
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public override bool CanExit()
        {
            SaveModifiedSettings(true);
            return true;
        }

        #region Presentation Model

        public ITree SettingsGroupTree
        {
            get { return _settingsGroupTree; }
        }

        public ISelection SelectedSettingsGroup
        {
            get { return new Selection(_selectedSettingsGroup); }
            set
            {
                SettingsGroupNode node = (SettingsGroupNode)value.Item;
                if (node != _selectedSettingsGroup)
                {
                    // save any changes before changing _selectedSettingsGroup
                    SaveModifiedSettings(true);

                    _selectedSettingsGroup = node;
                    LoadSettingsProperties();
                    EventsHelper.Fire(_selectedSettingsGroupChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedSettingsGroupChanged
        {
            add { _selectedSettingsGroupChanged += value; }
            remove { _selectedSettingsGroupChanged -= value; }
        }

        public ITable SettingsPropertiesTable
        {
            get { return _settingsPropertiesTable; }
        }

        public void SaveChanges()
        {
            SaveModifiedSettings(false);
        }

        #endregion

        private void LoadSettingsProperties()
        {
            _settingsPropertiesTable.Items.Clear();

            if (_selectedSettingsGroup != null && _selectedSettingsGroup.SettingsClass != null)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                _service.LoadSettingsValues(
                    SettingsClassMetaDataReader.GetGroupName(_selectedSettingsGroup.SettingsClass),
                    SettingsClassMetaDataReader.GetVersion(_selectedSettingsGroup.SettingsClass),
                    null, null, // load the default profile
                    values);    

                FillSettingsPropertiesTable(values);
            }
        }

        private void SaveModifiedSettings(bool confirmationRequired)
        {
            // if no dirty properties, nothing to save
            if (_selectedSettingsGroup != null && _selectedSettingsGroup.SettingsClass != null && IsAnyPropertyDirty())
            {
                if (confirmationRequired && !ConfirmSave())
                    return;

                // fill a dictionary with all values that differ from the defaults
                Dictionary<string, string> values = new Dictionary<string, string>();
                foreach (SettingsProperty p in _settingsPropertiesTable.Items)
                {
                    if (!p.UsingDefaultValue)
                    {
                        values[p.Name] = p.Value;
                    }
                }

                _service.SaveSettingsValues(
                        SettingsClassMetaDataReader.GetGroupName(_selectedSettingsGroup.SettingsClass),
                        SettingsClassMetaDataReader.GetVersion(_selectedSettingsGroup.SettingsClass),
                        null, null,    // save to the default profile
                        values);

                // refresh the table so that properties are no longer marked dirty
                FillSettingsPropertiesTable(values);
            }
        }

        private void FillSettingsPropertiesTable(IDictionary<string, string> storedValues)
        {
            _settingsPropertiesTable.Items.Clear();
            foreach (PropertyInfo pi in SettingsClassMetaDataReader.GetSettingsProperties(_selectedSettingsGroup.SettingsClass))
            {
                string value = storedValues.ContainsKey(pi.Name) ? storedValues[pi.Name] : SettingsClassMetaDataReader.GetDefaultValue(pi);
                _settingsPropertiesTable.Items.Add(new SettingsProperty(pi, value));
            }
        }

        private bool IsAnyPropertyDirty()
        {
            return CollectionUtils.Contains<SettingsProperty>(_settingsPropertiesTable.Items,
                delegate(SettingsProperty p) { return p.Dirty; });
        }

        private bool ConfirmSave()
        {
            DialogBoxAction action = this.Host.ShowMessageBox("Settings have been modified.  Save changes?", MessageBoxActions.YesNo);
            return action == DialogBoxAction.Yes;
        }

    }
}
