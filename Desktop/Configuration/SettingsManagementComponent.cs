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
            private SettingsPropertyDescriptor _descriptor;
            private string _value;
            private string _startingValue;

            private event EventHandler _valueChanged;

            public SettingsProperty(SettingsPropertyDescriptor descriptor, string value)
            {
                _descriptor = descriptor;
                _startingValue = _value = value;
            }

            public string Name
            {
                get { return _descriptor.Name; }
            }

            public string TypeName
            {
                get { return _descriptor.TypeName; }
            }

            public string Description
            {
                get { return _descriptor.Description; }
            }

            public SettingScope Scope
            {
                get { return _descriptor.Scope; }
            }

            public string DefaultValue
            {
                get { return _descriptor.DefaultValue; }
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
                get { return _value == _descriptor.DefaultValue; }
            }

            public bool Dirty
            {
                get { return _value != _startingValue; }
            }
        }

        #endregion

        private IConfigurationStore _configStore;

        private Table<SettingsGroupDescriptor> _settingsGroupTable;
        private SettingsGroupDescriptor _selectedSettingsGroup;
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
            _settingsGroupTable = new Table<SettingsGroupDescriptor>();
            _settingsGroupTable.Columns.Add(new TableColumn<SettingsGroupDescriptor, string>("Group",
                delegate(SettingsGroupDescriptor t) { return t.Name; }));
            _settingsGroupTable.Columns.Add(new TableColumn<SettingsGroupDescriptor, string>("Version",
                delegate(SettingsGroupDescriptor t) { return t.Version.ToString(); }));
            _settingsGroupTable.Columns.Add(new TableColumn<SettingsGroupDescriptor, string>("Description",
                delegate(SettingsGroupDescriptor t) { return t.Description; }));


            // define the settings properties table
            _settingsPropertiesTable = new Table<SettingsProperty>();
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Property",
                delegate(SettingsProperty p) { return p.Name; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Description",
               delegate(SettingsProperty p) { return p.Description; }));
            _settingsPropertiesTable.Columns.Add(new TableColumn<SettingsProperty, string>("Scope",
                delegate(SettingsProperty p) { return p.Scope.ToString(); }));
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
            try
            {
                foreach (SettingsGroupDescriptor group in _configStore.ListSettingsGroups())
                {
                    _settingsGroupTable.Items.Add(group);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override bool CanExit(UserInteraction interactive)
        {
            if (interactive == UserInteraction.Allowed)
            {
                SaveModifiedSettings(true);
                return true;
            }
            else
            {
                // return false if anything modified
                return _selectedSettingsGroup == null || !IsAnyPropertyDirty();
            }
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
                SettingsGroupDescriptor settingsClass = (SettingsGroupDescriptor)value.Item;
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
                try
                {
                    Dictionary<string, string> values = _configStore.LoadSettingsValues(
                            _selectedSettingsGroup,
                            null, null // load the default profile
                            );

                    FillSettingsPropertiesTable(values);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
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
					_configStore.SaveSettingsValues(
                        _selectedSettingsGroup,
						null, null,    // save to the default profile
						values);

                    // refresh the table so that properties are no longer marked dirty
                    FillSettingsPropertiesTable(values);
                    UpdateActionEnablement();

                    // update any loaded instances
                    ApplicationSettingsRegister.Instance.Synchronize(_selectedSettingsGroup);

                }
				catch (Exception e)
				{
					ExceptionHandler.Report(e, SR.MessageSaveSettingFailed, this.Host.DesktopWindow);
				}
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
            foreach (SettingsPropertyDescriptor pi in _configStore.ListSettingsProperties(_selectedSettingsGroup))
            {
                string value = storedValues.ContainsKey(pi.Name) ? storedValues[pi.Name] : pi.DefaultValue;
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
