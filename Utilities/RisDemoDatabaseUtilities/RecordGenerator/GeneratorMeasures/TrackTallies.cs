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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    [ExtensionOf(typeof(GeneratorMeasureExtensionPoint))]
    public class TrackTallies : BaseGeneratorMeasure
    {
        private Dictionary<string, DateTime> _pollTimes;
        private DateTime _startTime;

        public TrackTallies()
        {
            _pollTimes = new Dictionary<string, DateTime>();
            _startTime = DateTime.Now;
            foreach (string name in Enum.GetNames(typeof(GeneratorEntity)))
            {
                _pollTimes.Add(name, _startTime);
            }
        }

        public override IGeneratorMeasure GetInitializedCopy(ICollection<GeneratorMeasureSetting> settings)
        {
            return new TrackTallies();
        }

        public override string DisplayName
        {
            get { return SR.DisplayNameTrackTallies; }
        }

        protected override void OnNewStatisticAdded(object sender, NewStatisticAddedEventArgs e)
        {
            string entity = Enum.GetName(typeof(GeneratorEntity), e.Statistic.Entity);
            DateTime lastPostTime;
            if (_pollTimes.TryGetValue(entity, out lastPostTime) == false)
                return;

            if (DateTime.Now > lastPostTime.Add(_pollTime))
            {
                PostMeasureItem(entity);

                _pollTimes.Remove(entity);
                _pollTimes.Add(entity, DateTime.Now);
            }            
        }

        private void PostMeasureItem(string entity)
        {
            string measureLabel = "Number of " + entity + " records inserted";

            List<RecordGeneratorStatistic> matches = this.Context.FindAll(delegate(RecordGeneratorStatistic item) { return (Enum.GetName(typeof(GeneratorEntity), item.Entity) == entity) && (item.Type == GeneratorStatisticType.Insertion); });
            RecordGeneratorMeasureItem newMeasure = new RecordGeneratorMeasureItem(DateTime.Now, measureLabel, null, (double)matches.Count);

            this.Context.AddMeasureItems(new RecordGeneratorMeasureItem[] { newMeasure });
        }

        public override ICollection<GeneratorMeasureSetting> Settings
        {
            get { return _measureSettings; }
        }

        protected override void OnGeneratorStart(object sender, EventArgs e)
        {
            //do nothing
        }

        protected override void OnGeneratorStop(object sender, EventArgs e)
        {
            foreach (string entity in _pollTimes.Keys)
            {
                DateTime lastPostTime;
                if (_pollTimes.TryGetValue(entity, out lastPostTime) == true)
                {
                    if (lastPostTime != _startTime)
                    {
                        PostMeasureItem(entity);
                    }
                }
            }
        }
        
    }
}
