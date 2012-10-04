#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;

using ClearCanvas.Utilities.RisDemoDatabaseUtilities;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    /// <summary>
    /// Extension point for views onto <see cref="RecordGeneratorLaunchPadComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RecordGeneratorLaunchPadComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RecordGeneratorLaunchPadComponent class
    /// </summary>
    [AssociateView(typeof(RecordGeneratorLaunchPadComponentViewExtensionPoint))]
    public class RecordGeneratorLaunchPadComponent : ApplicationComponent
    {
        private Dictionary<string, IEntityRecordGeneratorLauncher> _generatorMap;
        private EntityRecordGeneratorExtensionPointManager _extManager;
        private IEnumerable<IEntityGeneratorSettingsList> _randomizerSettings;
        private string[] _generatorChoices;
        private string _selectedGeneratorName;
        private uint _numberOfEntitiesToBeGenerated;
        private Dictionary<string, IEnumerable<IEntityGeneratorSettingsList>> _settingsMap;

        private IDesktopWindow _desktopWindow;
        private bool _launchEnabled;
        private Table<EntityGeneratorSetting> _settingsTable;
        
        private Table<GeneratorMeasureSelector> _measureSelectionTable;
        private ISelection _currentMeasureSelection;
        private Table<GeneratorMeasureSetting> _measureSettingsTable;
        private string _selectedMeasureName;

        private IGeneratorMeasureManager _measureManager;

        private Dictionary<string, GeneratorMeasureSelector> _measureSelectionMap;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordGeneratorLaunchPadComponent()
        {         
        }

        public RecordGeneratorLaunchPadComponent(IDesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }

        public override void Start()
        {
            this.LaunchEnabled = true;
            this.NumberOfEntitiesToBeGenerated = "10";
            
            //_randomizer = new RisRandomDataGenerator();
            //_randomizer.PerformanceLogging = true;
            _randomizerSettings = new List<IEntityGeneratorSettingsList>();

            _settingsTable = new Table<EntityGeneratorSetting>();
            _settingsTable.Columns.Add(new TableColumn<EntityGeneratorSetting, string>(SR.ColumnSettingIdString,
                delegate(EntityGeneratorSetting item) { return item.Name; }));
            _settingsTable.Columns.Add(new TableColumn<EntityGeneratorSetting, string>(SR.ColumnSettingValue,
                delegate(EntityGeneratorSetting item) { return item.Setting == null ? "" : item.Setting.ToString(); },
                delegate(EntityGeneratorSetting item, string value) {   item.Setting = value;
                                                                        ApplyGeneratorSettingsChange();
                                                                    }));
            
            _generatorMap = new Dictionary<string, IEntityRecordGeneratorLauncher>();
            _settingsMap = new Dictionary<string, IEnumerable<IEntityGeneratorSettingsList>>();
            _extManager = new EntityRecordGeneratorExtensionPointManager();
            List<string> generatorChoices = new List<string>();
            foreach (IEntityRecordGeneratorLauncher generator in _extManager.EntityRecordGeneratorCollection)
            {
                generatorChoices.Add(generator.DisplayName);
                _generatorMap.Add(generator.DisplayName, generator);

                _settingsMap.Add(generator.DisplayName, generator.RandomizerSettings);
            }
            _generatorChoices = generatorChoices.ToArray();

            this.SelectedGeneratorName = _generatorChoices[0];

            _measureManager = new GeneratorMeasureManager();
            _measureSelectionMap = new Dictionary<string, GeneratorMeasureSelector>();
            _measureSelectionTable = new Table<GeneratorMeasureSelector>();
            _measureSelectionTable.Columns.Add(new TableColumn<GeneratorMeasureSelector, string>(SR.ColumnMeasureDisplayName,
                delegate(GeneratorMeasureSelector item) { return item.Name; }));
            _measureSelectionTable.Columns.Add(new TableColumn<GeneratorMeasureSelector, string>(SR.ColumnMeasureSelectedValue,
                delegate(GeneratorMeasureSelector item) { return item.Selected.ToString(); },
                delegate(GeneratorMeasureSelector item, string value) {  item.Selected = bool.Parse(value); }));

            _measureSettingsTable = new Table<GeneratorMeasureSetting>();
            _measureSettingsTable.Columns.Add(new TableColumn<GeneratorMeasureSetting, string>(SR.ColumnMeasureSettingIdString,
                delegate(GeneratorMeasureSetting item) { return item.Name; }));
            _measureSettingsTable.Columns.Add(new TableColumn<GeneratorMeasureSetting, string>(SR.ColumnMeasureSettingValue,
                delegate(GeneratorMeasureSetting item) { return item.Setting == null ? "" : item.Setting.ToString(); },
                delegate(GeneratorMeasureSetting item, string value) { item.Setting = value;
                                                                       RefreshMeasureSettingsTable();
                                                                     }));

            foreach (string measureName in _measureManager.MeasureDisplayNames)
            {
                GeneratorMeasureSelector measureSelection = new GeneratorMeasureSelector(measureName, true);
                _measureSelectionTable.Items.Add(measureSelection);
                _measureSelectionMap.Add(measureName, measureSelection);

            }
            _selectedMeasureName = _measureSelectionTable.Items[0].Name;
            _currentMeasureSelection = new Selection(_measureSelectionTable.Items[0]);
            RefreshMeasureSettingsTable();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public string[] GeneratorChoices
        {
            get { return _generatorChoices; }
        }

        public string SelectedGeneratorName
        {
            get { return _selectedGeneratorName; }
            set 
            { 
                _selectedGeneratorName = value;
                NotifyPropertyChanged("SelectedGeneratorName");
                RefreshSettingsTable();
            }
        }

        public void SetMeasureSelection(ISelection selection)
        {
            if (_currentMeasureSelection != selection)
            {
                _currentMeasureSelection = selection;

                GeneratorMeasureSelector selector = (GeneratorMeasureSelector) _currentMeasureSelection.Item;

                if (selector != null)
                {
                    _selectedMeasureName = selector.Name;
                    this.RefreshMeasureSettingsTable();
                }
            }
        }

        public string NumberOfEntitiesToBeGenerated
        {
            get { return _numberOfEntitiesToBeGenerated.ToString(); }
            set 
            { 
                _numberOfEntitiesToBeGenerated = uint.Parse(value);
                NotifyPropertyChanged("NumberOfEntitiesToBeGenerated");
            }
        }

        public Table<EntityGeneratorSetting> Settings
        {
            get { return _settingsTable; }
        }

        public Table<GeneratorMeasureSelector> MeasureSelections
        {
            get { return _measureSelectionTable; }
        }

        public Table<GeneratorMeasureSetting> MeasureSettings
        {
            get { return _measureSettingsTable; }
        }

        public bool LaunchEnabled
        {
            get { return _launchEnabled; }
            set 
            {
                _launchEnabled = value;
                NotifyPropertyChanged("LaunchEnabled");
            }
        }

        public void Launch()
        {
            RecordGeneratorComponent recordComponent = new RecordGeneratorComponent();
            
            IEntityRecordGenerator selectedGenerator = GetSelectedGenerator();

            recordComponent.initialize(selectedGenerator, _numberOfEntitiesToBeGenerated);

            _measureManager.ApplyMeasuresToComponent(GetSelectedMeasureNames(), recordComponent);

            string title = _selectedGeneratorName + " - Generates " + NumberOfEntitiesToBeGenerated + " Key Records";
            ApplicationComponent.LaunchAsWorkspace(_desktopWindow,
                recordComponent,
                title,
                delegate(IApplicationComponent component) { component = null; });
        }

        private IEntityRecordGenerator GetSelectedGenerator()
        {
            IEntityRecordGeneratorLauncher mappedValue;
            List<IEntityGeneratorSettingsList> settingLists = new List<IEntityGeneratorSettingsList>();

            if (_generatorMap.TryGetValue(_selectedGeneratorName, out mappedValue) == true)
            {
                foreach (IEntityGeneratorSettingsList list in GetSelectedGeneratorSettings())
                {
                    settingLists.Add(list.Copy());
                }

                return mappedValue.GetInitializedCopy(settingLists);
            }
            else
            {
                return null;
            }
        }

        private void ApplyGeneratorSettingsChange()
        {
            _randomizerSettings = GetSelectedGeneratorSettings();
        }

        private IEnumerable<IEntityGeneratorSettingsList> GetSelectedGeneratorSettings()
        {
            IEnumerable<IEntityGeneratorSettingsList> mappedValue;
            if (_settingsMap.TryGetValue(_selectedGeneratorName, out mappedValue) == true)
            {
                return mappedValue;
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<GeneratorMeasureSetting> GetSelectedMeasureSettings()
        {
             return _measureManager.GetMeasureSettings(_selectedMeasureName);
        }

        private ICollection<string> GetSelectedMeasureNames()
        {
            List<string> measures = new List<string>();

            foreach (KeyValuePair<string, GeneratorMeasureSelector> kvp in _measureSelectionMap)
            {
                if (kvp.Value.Selected == true)
                {
                    measures.Add(kvp.Value.Name);
                }
            }

            return measures;
        }

        private void RefreshSettingsTable()
        {
            _settingsTable.Items.Clear();

            foreach (IEntityGeneratorSettingsList list in GetSelectedGeneratorSettings())
            {
                foreach (EntityGeneratorSetting setting in list.Settings)
                {
                    _settingsTable.Items.Add(setting);
                }
            }
        }

        private void RefreshMeasureSettingsTable()
        {
            _measureSettingsTable.Items.Clear();

            foreach (GeneratorMeasureSetting setting in GetSelectedMeasureSettings())
            {
                _measureSettingsTable.Items.Add(setting);
            }
        }
    }
}
