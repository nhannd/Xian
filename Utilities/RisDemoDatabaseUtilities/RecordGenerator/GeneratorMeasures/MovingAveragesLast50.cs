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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    [ExtensionOf(typeof(GeneratorMeasureExtensionPoint))]
    public class MovingAveragesLast50 : BaseGeneratorMeasure
    {
        public MovingAveragesLast50()
        {
            _pollTime = new TimeSpan(0, 0, 30);
            _measureSettings.Add(new GeneratorMeasureSetting("GeneratorEntityEnumValues", GetSettingsValueFromArray(Enum.GetValues(typeof(GeneratorEntity)))));
            _measureSettings.Add(new GeneratorMeasureSetting("GeneratorStatisticTypeEnumValues", GetSettingsValueFromArray(Enum.GetValues(typeof(GeneratorStatisticType)))));
        }

        private MovingAveragesLast50(ICollection<GeneratorMeasureSetting> settings)
        {
            _pollTime = new TimeSpan(0, 0, 30);
            _measureSettings = settings as List<GeneratorMeasureSetting>;
        }

        public override IGeneratorMeasure GetInitializedCopy(ICollection<GeneratorMeasureSetting> settings)
        {
            return new MovingAveragesLast50(settings);
        }

        public override string DisplayName
        {
            get { return SR.DisplayNameMovingAverageLast50; }
        }

        protected override void OnGeneratorStart(object sender, EventArgs e)
        {
            //do nothing
        }

        protected override void OnNewStatisticAdded(object sender, NewStatisticAddedEventArgs e)
        {
            if (DateTime.Now > _lastPostTime.Add(_pollTime))
            {
                List<RecordGeneratorMeasureItem> measures = new List<RecordGeneratorMeasureItem>();
                foreach (GeneratorEntity entity in this.GeneratorEntityEnumValues)
                {
                    foreach (GeneratorStatisticType type in this.GeneratorStatisticTypeEnumValues)
                    {
                        double sum = 0.0;
                        List<RecordGeneratorStatistic> filtered = this.Context.FindAll(delegate(RecordGeneratorStatistic entry) { return entry.Entity == entity && entry.Type == type; });
                        if (filtered.Count > 50)
                        {
                            filtered.Sort(delegate(RecordGeneratorStatistic one, RecordGeneratorStatistic two) { return one.TimeOfSample.CompareTo(two.TimeOfSample); });
                            filtered.RemoveRange(0, filtered.Count - 50);
                        }
                        foreach (RecordGeneratorStatistic stat in filtered)
                        {
                            sum += stat.ExecutionTime;
                        }

                        string label = Enum.GetName(typeof(GeneratorEntity), entity) + "s " + Enum.GetName(typeof(GeneratorStatisticType), type) + " Time - Moving Average - Last 50";
                        measures.Add(new RecordGeneratorMeasureItem(DateTime.Now, label, null, sum / (double)filtered.Count));
                    }
                }

                this.Context.AddMeasureItems(measures);
                _lastPostTime = DateTime.Now;
            }
        }

        protected override void OnGeneratorStop(object sender, EventArgs e)
        {
            //do nothing
        }

        public override ICollection<GeneratorMeasureSetting> Settings
        {
            get { return _measureSettings; }
        }

        private GeneratorEntity[] GeneratorEntityEnumValues
        {
            get { return (GeneratorEntity[])GetArrayFromEnumArraySettingsValue("GeneratorEntityEnumValues", typeof(GeneratorEntity)); }
        }

        private GeneratorStatisticType[] GeneratorStatisticTypeEnumValues
        {
            get { return (GeneratorStatisticType[])GetArrayFromEnumArraySettingsValue("GeneratorStatisticTypeEnumValues", typeof(GeneratorStatisticType)); }
        }
    }
}
