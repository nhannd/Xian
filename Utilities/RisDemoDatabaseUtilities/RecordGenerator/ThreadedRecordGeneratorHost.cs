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

using ClearCanvas.Common.Utilities;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator;
using ClearCanvas.Desktop;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    public class ThreadedRecordGeneratorHost : IRecordGeneratorHost
    {
        private BackgroundTask _thread;
        private PropertySetDelegate<RecordGeneratorStatistic> _setStatisticDelegate;
        private event EventHandler _generatorStoppedEvent;
        private uint _numberOfEntitiesToBeGenerated;
        private IEntityRecordGenerator _generator;

        #region IRecordGeneratorHost Members

        public void Initialize(IEntityRecordGenerator generator, uint numberOfEntitiesToBeGenerated, PropertySetDelegate<RecordGeneratorStatistic> setStatisticDelegate)
        {
            Platform.CheckForNullReference(numberOfEntitiesToBeGenerated, "numberOfEntitiesGenerated");
            Platform.CheckForNullReference(setStatisticDelegate, "setStatisticDelegate");

            if (_thread != null)
                throw new InvalidOperationException();

            _numberOfEntitiesToBeGenerated = numberOfEntitiesToBeGenerated;
            _generator = generator;

            _setStatisticDelegate = setStatisticDelegate;
        }

        public void Start()
        {
            _thread = new BackgroundTask(delegate(IBackgroundTaskContext context) { Generate(context); }, true);
            if (_thread != null)
            {
                _thread.ProgressUpdated += NewStatsHandler;
                _thread.Terminated += TaskTerminated;
                _thread.Run();
            }
        }

        public void Stop()
        {
            Platform.CheckForNullReference(_thread, "_thread");

            _thread.RequestCancel();
        }

        public event EventHandler GeneratorStoppedEvent
        {
            add { _generatorStoppedEvent += value; }
            remove { _generatorStoppedEvent -= value; }
        }

        #endregion

        private void Generate(IBackgroundTaskContext context)
        {
            uint numEntities = 0;

            while (numEntities < _numberOfEntitiesToBeGenerated)
            {
                if (context.CancelRequested)
                    return;

                IEnumerable<RecordGeneratorStatistic> singleStatBatch = _generator.Run();

                context.ReportProgress(new GeneratorProgress(singleStatBatch));

                numEntities += _generator.EntitiesOfInterestGeneratedInLastRun;
            }

            context.Complete(null);
        }

        public void NewStatsHandler(object sender, BackgroundTaskProgressEventArgs e)
        {
            {
                GeneratorProgress progress = e.Progress as GeneratorProgress;
                foreach (RecordGeneratorStatistic stat in progress.Statistics)
                {
                    _setStatisticDelegate(stat);
                }
            }
        }

        private void TaskTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            //deal with exception
            _thread = null;
            EventsHelper.Fire(_generatorStoppedEvent, this, EventArgs.Empty);
        }
    }

    internal class GeneratorProgress : BackgroundTaskProgress
    {
        private IEnumerable<RecordGeneratorStatistic> _statistics;

        internal GeneratorProgress(IEnumerable<RecordGeneratorStatistic> statistics)
            : base()
        {
            _statistics = statistics;
        }

        internal IEnumerable<RecordGeneratorStatistic> Statistics
        {
            get { return _statistics; }
        }
    }
}
