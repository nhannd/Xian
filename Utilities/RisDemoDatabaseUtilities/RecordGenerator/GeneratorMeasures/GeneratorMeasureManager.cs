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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    public interface IGeneratorMeasureManager
    {
        ICollection<string> MeasureDisplayNames { get; }
        ICollection<GeneratorMeasureSetting> GetMeasureSettings(string name);
        void ApplyMeasuresToComponent(ICollection<string> displayNames, RecordGeneratorComponent component);
    }



    public class GeneratorMeasureManager : IGeneratorMeasureManager
    {
        private GeneratorMeasureExtensionPointManager _measureExtManager;
        private Dictionary<string, IGeneratorMeasureLauncher> _measureMap;
        private Dictionary<string, ICollection<GeneratorMeasureSetting>> _measureSettingsMap;

        public GeneratorMeasureManager()
        {
            _measureExtManager = new GeneratorMeasureExtensionPointManager();
            _measureMap = new Dictionary<string, IGeneratorMeasureLauncher>();
            _measureSettingsMap = new Dictionary<string, ICollection<GeneratorMeasureSetting>>();

            foreach (IGeneratorMeasureLauncher measure in _measureExtManager.EntityRecordGeneratorCollection)
            {
                _measureMap.Add(measure.DisplayName, measure);  

                GeneratorMeasureSelector measureSelection = new GeneratorMeasureSelector(measure.DisplayName, true);
                _measureSettingsMap.Add(measure.DisplayName, measure.Settings);
            }
        }

        #region IGeneratorMeasureHost Members

        public ICollection<string> MeasureDisplayNames
        {
            get { return _measureMap.Keys; }
        }

        public ICollection<GeneratorMeasureSetting> GetMeasureSettings(string name)
        {
            ICollection<GeneratorMeasureSetting> settings;
            if (_measureSettingsMap.TryGetValue(name, out settings))
            {
                return settings;
            }
            else
            {
                return null;
            }
        }

        public void ApplyMeasuresToComponent(ICollection<string> displayNames, RecordGeneratorComponent component)
        {
            IGeneratorMeasureLauncher launcher;
            IGeneratorMeasure measure;
            List<GeneratorMeasureSetting> settings = new List<GeneratorMeasureSetting>();

            foreach (string name in displayNames)
            {
                if (_measureMap.TryGetValue(name, out launcher))
                {
                    foreach (GeneratorMeasureSetting setting in GetMeasureSettings(name))
                    {
                        settings.Add(new GeneratorMeasureSetting(setting.Name, setting.Setting));
                    }

                    measure = launcher.GetInitializedCopy(settings);
                    measure.ConnectedComponent = component;
                }
            }
        }
        #endregion
    }
}
