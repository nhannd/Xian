using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuConfigureSettings")]
	[ClickHandler("activate", "Activate")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class SettingsManagementLaunchTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Activate()
        {
            if (_workspace == null)
            {
				IConfigurationStore store = null;
				try
                {
                    // if this throws an exception, only the default LocalFileSettingsProvider can be used.
                    store = (IConfigurationStore)(new ConfigurationStoreExtensionPoint()).CreateExtension();
                }
                catch (NotSupportedException)
                {
					//allow editing of the app.config file via the LocalConfigurationStore.
					store = new LocalConfigurationStore();
                }
			
				_workspace = ApplicationComponent.LaunchAsWorkspace(
					this.Context.DesktopWindow,
					new SettingsManagementComponent(store),
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
        #region SettingsProperty class

        public class SettingsProperty
        {
            private PropertyInfo _propertyInfo;
            private string _value;
            private string _startingValue;
            private string _defaultValue;

            private event EventHandler _valueChanged;

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
                set
                {
                    if (value != _value)
                    {
                        _value = value;
                        EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
                    }
                }
            }

            public event EventHandler ValueChanged
            {
                add { _valueChanged += value; }
                remove { _valueChanged -= value; }
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

        private IConfigurationStore _configStore;

        private Table<Type> _settingsGroupTable;
        private Type _selectedSettingsGroup;
        private event EventHandler _selectedSettingsGroupChanged;

        private Table<SettingsProperty> _settingsPropertiesTable;
        private SettingsProperty _selectedSettingsProperty;
        private event EventHandler _selectedSettingsPropertyChanged;

        private SimpleActionModel _settingsPropertiesActionModel;
        private ClickAction _saveAllAction;
        private ClickAction _resetAllAction;
        private ClickAction _resetAction;
        private ClickAction _editAction;


        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsManagementComponent(IConfigurationStore configStore)
        {
            _configStore = configStore;

            // define the structure of the settings group table
            _settingsGroupTable = new Table<Type>();
            _settingsGroupTable.Columns.Add(new TableColumn<Type, string>("Group",
                delegate(Type t) { return SettingsClassMetaDataReader.GetGroupName(t); }));
            _settingsGroupTable.Columns.Add(new TableColumn<Type, string>("Version",
                delegate(Type t) { return SettingsClassMetaDataReader.GetVersion(t).ToString(); }));
            _settingsGroupTable.Columns.Add(new TableColumn<Type, string>("Description",
                delegate(Type t) { return SettingsClassMetaDataReader.GetGroupDescription(t); }));


            // define the settings properties table
            _settingsPropertiesTable = new Table<SettingsProperty>();
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Property",
                delegate(SettingsProperty p) { return p.Name; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Description",
               delegate(SettingsProperty p) { return p.Description; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Scope",
                delegate(SettingsProperty p) { return p.Scope; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Type",
                delegate(SettingsProperty p) { return p.TypeName; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Default Value",
                delegate(SettingsProperty p) { return p.DefaultValue; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Value",
               delegate(SettingsProperty p) { return p.Value; },
               delegate(SettingsProperty p, string text) { p.Value = text; }));

            _settingsPropertiesActionModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));

            _saveAllAction = _settingsPropertiesActionModel.AddAction("saveall", SR.LabelSaveAll, "SaveToolSmall.png",
                delegate() { SaveModifiedSettings(false); });

            _editAction = _settingsPropertiesActionModel.AddAction("edit", SR.LabelEdit, "EditToolSmall.png",
               delegate() { EditProperty(_selectedSettingsProperty); });

            _resetAction = _settingsPropertiesActionModel.AddAction("reset", SR.LabelReset, "ResetToolSmall.png",
               delegate() { ResetPropertyValue(_selectedSettingsProperty); });

            _resetAllAction = _settingsPropertiesActionModel.AddAction("resetall", SR.LabelResetAll, "ResetAllToolSmall.png",
                delegate() { ResetAllPropertyValues(); });

        }

        public override void Start()
        {
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                foreach (Type t in plugin.Assembly.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(ApplicationSettingsBase)) && !t.IsAbstract)
                    {
                        _settingsGroupTable.Items.Add(t);
                    }
                }
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

        public ITable SettingsGroupTable
        {
            get { return _settingsGroupTable; }
        }

        public ISelection SelectedSettingsGroup
        {
            get { return new Selection(_selectedSettingsGroup); }
            set
            {
                Type settingsClass = (Type)value.Item;
                if (settingsClass != _selectedSettingsGroup)
                {
                    // save any changes before changing _selectedSettingsGroup
                    SaveModifiedSettings(true);

                    _selectedSettingsGroup = settingsClass;
                    LoadSettingsProperties();
                    UpdateActionEnablement();
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

        public ActionModelRoot SettingsPropertiesActionModel
        {
            get { return _settingsPropertiesActionModel; }
        }

        public ISelection SelectedSettingsProperty
        {
            get { return new Selection(_selectedSettingsProperty); }
            set
            {
                SettingsProperty p = (SettingsProperty)value.Item;
                if (p != _selectedSettingsProperty)
                {
                    _selectedSettingsProperty = p;
                    UpdateActionEnablement();
                    EventsHelper.Fire(_selectedSettingsPropertyChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedSettingsPropertyChanged
        {
            add { _selectedSettingsPropertyChanged += value; }
            remove { _selectedSettingsPropertyChanged -= value; }
        }

        public void SettingsPropertyDoubleClicked()
        {
            if (_selectedSettingsProperty != null)
            {
                EditProperty(_selectedSettingsProperty);
            }
        }

        #endregion

        private void LoadSettingsProperties()
        {
            _settingsPropertiesTable.Items.Clear();

            if (_selectedSettingsGroup != null)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                _configStore.LoadSettingsValues(_selectedSettingsGroup,
                    null, null, // load the default profile
                    values);    

                FillSettingsPropertiesTable(values);
            }
        }

        private void SaveModifiedSettings(bool confirmationRequired)
        {
            // if no dirty properties, nothing to save
            if (_selectedSettingsGroup != null && IsAnyPropertyDirty())
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

				try
				{
					_configStore.SaveSettingsValues(_selectedSettingsGroup,
						null, null,    // save to the default profile
						values);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.MessageSaveSettingFailed, this.Host.DesktopWindow);
				}

                // refresh the table so that properties are no longer marked dirty
                FillSettingsPropertiesTable(values);
                UpdateActionEnablement();
            }
        }

        private void ResetAllPropertyValues()
        {
            DialogBoxAction action = this.Host.ShowMessageBox("Reset all settings back to default values?", MessageBoxActions.YesNo);
            if (action == DialogBoxAction.Yes)
            {
                foreach (SettingsProperty property in _settingsPropertiesTable.Items)
                {
                    property.Value = property.DefaultValue;
                    _settingsPropertiesTable.Items.NotifyItemUpdated(property);
                }
            }
        }

        private void ResetPropertyValue(SettingsProperty property)
        {
            DialogBoxAction action = this.Host.ShowMessageBox("Reset this setting back to its default value?", MessageBoxActions.YesNo);
            if (action == DialogBoxAction.Yes)
            {
                property.Value = property.DefaultValue;
                _settingsPropertiesTable.Items.NotifyItemUpdated(property);
            }
        }

        private void EditProperty(SettingsProperty property)
        {
            if (property != null)
            {
                SettingEditorComponent editor = new SettingEditorComponent(property.DefaultValue, property.Value);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Edit Value");
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    property.Value = editor.CurrentValue;

                    // update the table to reflect the changed value
                    _settingsPropertiesTable.Items.NotifyItemUpdated(property);
                }
            }
        }

        private void UpdateActionEnablement()
        {
            _resetAction.Enabled = (_selectedSettingsProperty != null && !_selectedSettingsProperty.UsingDefaultValue);
            _editAction.Enabled = (_selectedSettingsProperty != null);
            _saveAllAction.Enabled = (_selectedSettingsGroup != null && IsAnyPropertyDirty());
            _resetAllAction.Enabled = CollectionUtils.Contains<SettingsProperty>(_settingsPropertiesTable.Items,
                delegate(SettingsProperty p) { return !p.UsingDefaultValue; });
        }

        private void FillSettingsPropertiesTable(IDictionary<string, string> storedValues)
        {
            _settingsPropertiesTable.Items.Clear();
            foreach (PropertyInfo pi in SettingsClassMetaDataReader.GetSettingsProperties(_selectedSettingsGroup))
            {
                string value = storedValues.ContainsKey(pi.Name) ? storedValues[pi.Name] : SettingsClassMetaDataReader.GetDefaultValue(pi);
                SettingsProperty property = new SettingsProperty(pi, value);
                property.ValueChanged += SettingsPropertyValueChangedEventHandler;
                _settingsPropertiesTable.Items.Add(property);
            }
        }

        private void SettingsPropertyValueChangedEventHandler(object sender, EventArgs args)
        {
            UpdateActionEnablement();
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
