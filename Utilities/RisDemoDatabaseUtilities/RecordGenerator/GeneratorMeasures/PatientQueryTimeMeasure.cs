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
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Healthcare;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    [ExtensionOf(typeof(GeneratorMeasureExtensionPoint))]
    public class PatientQueryTimeMeasure : BaseGeneratorMeasure
    {
        RisRandomDataGenerator _generator;
        private IAdtService _adtService;
        private PatientProfileSearchCriteria _searchCriteria;
        private Dictionary<BackgroundTask, string> _tasksMap; 

        private readonly object _measuresLock = new object();        
        PatientProfileSearchCriteria _criteria;

        public PatientQueryTimeMeasure()
        {
            _tasksMap = new Dictionary<BackgroundTask, string>();

            _criteria = new PatientProfileSearchCriteria();

            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchCriteria = new PatientProfileSearchCriteria();
            _generator = new RisRandomDataGenerator(true);
            
            int numQueries = 1;
            int interval = 120;
            int offset = 15;
            _measureSettings.Add(new GeneratorMeasureSetting("Polling Interval (s)", interval));
            _measureSettings.Add(new GeneratorMeasureSetting("Thread offset (s)", offset));
            _measureSettings.Add(new GeneratorMeasureSetting("Family Name Criteria (if blank will use random letter)", ""));
            _measureSettings.Add(new GeneratorMeasureSetting("Number of Query Threads", numQueries));
        }

        private PatientQueryTimeMeasure(ICollection<GeneratorMeasureSetting> settings)
        {
            _tasksMap = new Dictionary<BackgroundTask, string>();

            _criteria = new PatientProfileSearchCriteria();

            _adtService = ApplicationContext.GetService<IAdtService>();
            _searchCriteria = new PatientProfileSearchCriteria();
            _generator = new RisRandomDataGenerator();

            _measureSettings = settings as List<GeneratorMeasureSetting>;
        }

        public override IGeneratorMeasure GetInitializedCopy(ICollection<GeneratorMeasureSetting> settings)
        {
            return new PatientQueryTimeMeasure(settings);
        }

        public override string DisplayName
        {
            get { return SR.DisplayNamePatientQueryTimeMeasure; }
        }

        public override ICollection<GeneratorMeasureSetting> Settings
        {
            get { return _measureSettings; }
        }

        protected override void OnGeneratorStart(object sender, EventArgs e)
        {
            StringBuilder displayCriteria = new StringBuilder("Criteria: ");
            //eventually might add more to the criteria
            string enteredCriteria = FamilyNameCriteria;
            displayCriteria.Append("Lastname = " + enteredCriteria + ";");

            _criteria.Name.FamilyName.StartsWith(enteredCriteria);

            for (int i = 0; i < NumberOfQueryThreads; i++)
            {
                int offset = i * OffSetInMilliseconds;
                BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context) { Query(context, _criteria, displayCriteria.ToString(), offset); }, true);
                if (task != null)
                {
                    task.ProgressUpdated += MeasureHandler;
                    task.Terminated += TaskTerminated;
                    task.Run();
                }

                _tasksMap.Add(task, " (Thread #" + i.ToString() + ")" );
            }
        }

        protected override void OnGeneratorStop(object sender, EventArgs e)
        {
            this.Stop();
        }

        private void Stop()
        {
            foreach (BackgroundTask task in _tasksMap.Keys)
            {
                task.RequestCancel();
            }
        }

        protected override void OnNewStatisticAdded(object sender, NewStatisticAddedEventArgs e)
        {
            //Do Nothing
        }

        private void Query(IBackgroundTaskContext context, PatientProfileSearchCriteria criteria, string displayCriteriaString, int offsetDelay)
        {
            DateTime lastPostTime = DateTime.Now;
            IList<PatientProfile> results;
            long start;
            long stop;
            TimeSpan span;

            Thread.Sleep(offsetDelay);

            while (!context.CancelRequested)
            {
                start = DateTime.Now.Ticks;
                results = _adtService.ListPatientProfiles(criteria);
                stop = DateTime.Now.Ticks;
                span = new TimeSpan(stop - start);

                string label = "Patient Query - s/Result:";

                context.ReportProgress(new QueryProgress(new RecordGeneratorMeasureItem(DateTime.Now, label, displayCriteriaString + " # of Results = " + results.Count, (double)span.Seconds / (double)results.Count)));
 
                lastPostTime = DateTime.Now;

                System.Threading.Thread.Sleep(PollTime);
            }

            context.Complete(null);

        }

        private void MeasureHandler(object sender, BackgroundTaskProgressEventArgs e)
        {
            QueryProgress progress = e.Progress as QueryProgress;
            RecordGeneratorMeasureItem measure = progress.Measure;

            string threadname;
            string label = null;

            if (_tasksMap.TryGetValue(sender as BackgroundTask, out threadname))
            {
                label = measure.Label + threadname;
            }

            this.Context.AddMeasureItems(new RecordGeneratorMeasureItem[] { new RecordGeneratorMeasureItem(measure.TimeOfMeasure, label, measure.Info, measure.MeasureValue)  });
        }

        private void TaskTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            //deal with exception
            _tasksMap.Remove(sender as BackgroundTask);
        }

        private int PollTime
        {
            get 
            { 
                return GetIntFromSettingsValue("Polling Interval (s)") * 1000;
            }
        }

        private int OffSetInMilliseconds
        {
            get
            {
                return GetIntFromSettingsValue("Thread offset (s)")*1000;
            }
        }

        private string FamilyNameCriteria
        {
            get
            {
                string criteria = GetSettingValue("Family Name Criteria (if blank will use random letter)").ToString();
                return criteria == "" ? _generator.GetUpperCaseLetter() : criteria;
            }
        }

        private int NumberOfQueryThreads
        {
            get { return GetIntFromSettingsValue("Number of Query Threads"); }
        }
    }

    internal class QueryProgress : BackgroundTaskProgress
    {
        private RecordGeneratorMeasureItem _measure;

        internal QueryProgress(RecordGeneratorMeasureItem measure) : base()
        {
            _measure = measure;
        }

        internal RecordGeneratorMeasureItem Measure
        {
            get { return _measure; }
        }
    }
}
