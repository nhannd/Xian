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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;
using System.Collections;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    //[ExtensionOf(typeof(GeneratorMeasureExtensionPoint))]
    public abstract class BaseGeneratorMeasure : Tool<IGeneratorMeasureContext>, IGeneratorMeasureLauncher, IGeneratorMeasure
    {
        protected TimeSpan _pollTime;
        protected DateTime _lastPostTime;

        private RecordGeneratorComponent _pairedComponent;
        protected List<GeneratorMeasureSetting> _measureSettings;

        public BaseGeneratorMeasure()
        {
            _pollTime = new TimeSpan(0, 0, 1);
            _lastPostTime = DateTime.Now;
            _measureSettings = new List<GeneratorMeasureSetting>();
        }

        #region IGeneratorMeasureLauncher Members

        public abstract string DisplayName { get;}        

        public abstract ICollection<GeneratorMeasureSetting> Settings { get; }

        public abstract IGeneratorMeasure GetInitializedCopy(ICollection<GeneratorMeasureSetting> settings);

        #endregion

        #region IGeneratorMeasure Members

        public RecordGeneratorComponent ConnectedComponent
        {
            set 
            { 
                _pairedComponent = value;
                SetContext(new ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.RecordGeneratorComponent.GeneratorMeasureContext(_pairedComponent));
                this.Context.NewStatisticAdded += new EventHandler<NewStatisticAddedEventArgs>(OnNewStatisticAdded);
                this.Context.UserBeganGenerationEvent += new EventHandler(OnGeneratorStart);
                this.Context.UserStoppedGenerationEvent += new EventHandler(OnGeneratorStop);
            }
        }

        #endregion

        protected abstract void OnGeneratorStart(object sender, EventArgs e);

        protected abstract void OnGeneratorStop(object sender, EventArgs e);

        protected abstract void OnNewStatisticAdded(object sender, NewStatisticAddedEventArgs e);

        #region Settings Helper Methods
        
        protected object GetSettingValue(string id)
        {
            GeneratorMeasureSetting match = _measureSettings.Find(delegate(GeneratorMeasureSetting setting) { return setting.Name == id; });
            return match.Setting;
        }

        protected int GetIntFromSettingsValue(string id)
        {
            return int.Parse(GetSettingValue(id).ToString());
        }

        protected bool GetBoolFromSettingsValue(string id)
        {
            return bool.Parse(GetSettingValue(id).ToString());
        }

        protected object GetSettingsValueFromArray(Array settingsArray)
        {
            string settingvalue = null;
            foreach (object setting in settingsArray)
            {
                settingvalue += setting.ToString() + ";";
            }

            return settingvalue;
        }

        protected Array GetArrayFromEnumArraySettingsValue(string id, Type enumType)
        {
            string settingValue = GetSettingValue(id).ToString();
            //toss exception if setting to string fails
            //List<object> values = new List<object>();
            ArrayList values = new ArrayList();

            int index = settingValue.IndexOf(';');
            while (index != -1 && settingValue.Length > 0)
            {
                values.Add(Enum.Parse(enumType, settingValue.Substring(0, index)));
                settingValue = settingValue.Substring(index + 1, settingValue.Length - (index + 1));
                index = settingValue.IndexOf(';');
            }

            if (!settingValue.EndsWith(";") && settingValue.Length > 0)
                values.Add(Enum.Parse(enumType, settingValue));

            return values.ToArray(enumType);
        }

        #endregion
    }
}
