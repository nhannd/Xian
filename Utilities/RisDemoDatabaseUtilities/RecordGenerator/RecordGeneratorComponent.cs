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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using System.ComponentModel;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    /// <summary>
    /// Extension point for views onto <see cref="RecordGeneratorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RecordGeneratorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IGeneratorMeasureContext : IToolContext
    {
        event EventHandler UserBeganGenerationEvent;
        event EventHandler UserStoppedGenerationEvent;
        event EventHandler<NewStatisticAddedEventArgs> NewStatisticAdded;
        void AddMeasureItems(IEnumerable<RecordGeneratorMeasureItem> items);
        List<RecordGeneratorStatistic> FindAll(Predicate<RecordGeneratorStatistic> match);        
    }

    /// <summary>
    /// RecordGeneratorComponent class
    /// </summary>
    [AssociateView(typeof(RecordGeneratorComponentViewExtensionPoint))]
    public class RecordGeneratorComponent : ApplicationComponent
    {
        public class GeneratorMeasureContext : ToolContext, IGeneratorMeasureContext
        {
            RecordGeneratorComponent _component;

            public GeneratorMeasureContext(RecordGeneratorComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component;
            }

            #region IGeneratorMeasureContext Members

            public event EventHandler UserBeganGenerationEvent
            {
                add { _component._userBeganGenerationEvent += value; }
                remove { _component._userBeganGenerationEvent -= value; }
            }

            public event EventHandler UserStoppedGenerationEvent
            {
                add { _component._userStoppedGenerationEvent += value; }
                remove { _component._userStoppedGenerationEvent -= value; }
            }

            public event EventHandler<NewStatisticAddedEventArgs> NewStatisticAdded
            {
                add { _component._newStatisticAddedEvent += value; }
                remove { _component._newStatisticAddedEvent -= value; }
            }

            public void AddMeasureItems(IEnumerable<RecordGeneratorMeasureItem> items)
            {
                foreach (RecordGeneratorMeasureItem item in items)
                {
                    _component._measures.Add(item);
                    int matchIndex = -1;
                    matchIndex = _component._measureTable.Items.FindIndex(delegate(RecordGeneratorMeasureItem measureItem) { return measureItem.Label == item.Label; });
                    if (matchIndex != -1)
                    {
                        _component._measureTable.Items[matchIndex] = item;

                    }
                    else
                    {
                        _component._measureTable.Items.Add(item);
                    }
                }
            }

            public List<RecordGeneratorStatistic> FindAll(Predicate<RecordGeneratorStatistic> match)
            {
                return _component._stats.FindAll(match);
            }
        #endregion


        }
        
        private IRecordGeneratorHost _generatorHost;
        private IEntityRecordGenerator _generator;
        private uint _numberOfEntitiesToBeGenerated;

        private Table<RecordGeneratorStatistic> _statTable;
        private Table<RecordGeneratorMeasureItem> _measureTable;
        private List<RecordGeneratorStatistic> _stats;
        private List<RecordGeneratorMeasureItem> _measures;

        private bool _startEnabled;
        private bool _stopEnabled;
        private bool _statsExportEnabled;
        private bool _measureExportEnabled;

        private bool _running;

        private event EventHandler<NewStatisticAddedEventArgs> _newStatisticAddedEvent;
        private event EventHandler _userBeganGenerationEvent;
        private event EventHandler _userStoppedGenerationEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordGeneratorComponent()
        {
            _generatorHost = new ThreadedRecordGeneratorHost();
            _stats = new List<RecordGeneratorStatistic>();
            _generatorHost.GeneratorStoppedEvent += GeneratorStoppedEventHandler;
            _measures = new List<RecordGeneratorMeasureItem>();
        }

        public override void Start()
        {
            _statTable = new Table<RecordGeneratorStatistic>();
            _statTable.Columns.Add(new TableColumn<RecordGeneratorStatistic, string>(SR.ColumnTimeOfSample,
                delegate(RecordGeneratorStatistic row) { return row.TimeOfSample.ToString(SR.DateTimeStringFormat); }));
            _statTable.Columns.Add(new TableColumn<RecordGeneratorStatistic, string>(SR.ColumnGeneratorEntity,
                delegate(RecordGeneratorStatistic row) { return Enum.GetName(typeof(GeneratorEntity),row.Entity); }));
            _statTable.Columns.Add(new TableColumn<RecordGeneratorStatistic, string>(SR.ColumnGeneratorStatisticType,
                delegate(RecordGeneratorStatistic row) { return Enum.GetName(typeof(GeneratorStatisticType), row.Type); }));
            _statTable.Columns.Add(new TableColumn<RecordGeneratorStatistic, string>(SR.ColumnExecutionTime,
                delegate(RecordGeneratorStatistic row) { return row.ExecutionTime.ToString(); }));

            _measureTable = new Table<RecordGeneratorMeasureItem>();
            _measureTable.Columns.Add(new TableColumn<RecordGeneratorMeasureItem, string>(SR.ColumnTimeOfMeasure,
                delegate(RecordGeneratorMeasureItem row) { return row.TimeOfMeasure.ToString(SR.DateTimeStringFormat); }));
            _measureTable.Columns.Add(new TableColumn<RecordGeneratorMeasureItem, string>(SR.ColumnMeasureLabel,
                delegate(RecordGeneratorMeasureItem row) { return row.Label; }));
            _measureTable.Columns.Add(new TableColumn<RecordGeneratorMeasureItem, string>(SR.ColumnMeasureInfo,
                delegate(RecordGeneratorMeasureItem row) { return row.Info; }));
            _measureTable.Columns.Add(new TableColumn<RecordGeneratorMeasureItem, string>(SR.ColumnMeasureValue,
                delegate(RecordGeneratorMeasureItem row) { return row.MeasureValue.ToString(); }));

            _startEnabled = true;
            _stopEnabled = false;
            _statsExportEnabled = false;
            _measureExportEnabled = false;

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            _generatorHost.Stop();
            base.Stop();
        }

        public Table<RecordGeneratorStatistic> StatisticTable
        {
            get 
            {
                return _statTable;
            }
        }

        public Table<RecordGeneratorMeasureItem> MeasureTable
        {
            get { return _measureTable; }
        }

        public void initialize(IEntityRecordGenerator generator, uint numberOfEntitiesToBeGenerated)
        {
            _generator = generator;
            _numberOfEntitiesToBeGenerated = numberOfEntitiesToBeGenerated;
        }

        public void BeginGeneration()
        {
            if (_stats.Count != 0 || _measures.Count != 0 || _statTable.Items.Count != 0 || _measureTable.Items.Count != 0)
            {
                if (Platform.ShowMessageBox(SR.MessageClearComponentTables, MessageBoxActions.YesNo) != DialogBoxAction.Yes)
                {
                    return;
                }
                else
                {
                    _stats.Clear();
                    _measures.Clear();
                    _statTable.Items.Clear();
                    _measureTable.Items.Clear();
                }
            }

            _generatorHost.Initialize(_generator, _numberOfEntitiesToBeGenerated, AddRecordGeneratorStatistic);
            
            _generatorHost.Start(); 
            this.Running = true;

            EventsHelper.Fire(_userBeganGenerationEvent, this, EventArgs.Empty);
        }

        public void StopGeneration()
        {
            _generatorHost.Stop();
            this.Running = false;
        }

        public bool StartEnabled
        {
            get { return _startEnabled; }
            set { 
                    _startEnabled = value;
                    NotifyPropertyChanged("StartEnabled");
                }
        }

        public bool StopEnabled
        {
            get { return _stopEnabled; }
            set
            {
                _stopEnabled = value;
                NotifyPropertyChanged("StopEnabled");
            }
        }

        public bool StatsExportEnabled
        {
            get { return _statsExportEnabled; }
            set
            {
                _statsExportEnabled = value;
                NotifyPropertyChanged("StatsExportEnabled");
            }
        }

        public bool MeasureExportEnabled
        {
            get { return _measureExportEnabled; }
            set
            {
                _measureExportEnabled = value;
                NotifyPropertyChanged("MeasureExportEnabled");
            }
        }

        public void ExportStats(Stream stream)
        {
            BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext taskContext)
            {
                FileStream fs = (FileStream)stream;

                if (fs != null)
                {
                    StringBuilder csv = new StringBuilder();
                    foreach (TableColumn<RecordGeneratorStatistic, string> column in _statTable.Columns)
                    {
                        csv.Append(column.Name);
                        csv.Append(',');
                    }
                    csv.Remove(csv.Length - 1, 1);
                    csv.Append(System.Environment.NewLine);

                    int i = 0;
                    while (i < _stats.Count)
                    {
                        if (taskContext.CancelRequested)
                        {
                            taskContext.Cancel();
                            return;
                        }

                        csv.Append(_stats[i].TimeOfSample.ToString(SR.DateTimeStringFormat));
                        csv.Append(',');
                        csv.Append(Enum.GetName(typeof(GeneratorEntity), _stats[i].Entity));
                        csv.Append(',');
                        csv.Append(Enum.GetName(typeof(GeneratorStatisticType), _stats[i].Type));
                        csv.Append(',');
                        csv.Append(_stats[i].ExecutionTime);
                        csv.Append(System.Environment.NewLine);

                        taskContext.ReportProgress(new BackgroundTaskProgress((int)(((double)(i + 1) / (double)_stats.Count) * 100.0), "Writing to File..."));
                        i++;
                    }

                    StreamWriter writer = new StreamWriter(fs);

                    writer.Write(csv.ToString());
                    writer.Flush();
                    writer.Close();


                    fs.Close();
                }

                taskContext.Complete(null);

            }, true);

            ProgressDialog.Show(task, true, this.Host.DesktopWindow);
        }

        public void ExportMeasures(Stream stream)
        {
            BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext taskContext)
            {
                FileStream fs = (FileStream)stream;

                if (fs != null)
                {
                    StringBuilder csv = new StringBuilder();
                    foreach (TableColumn<RecordGeneratorMeasureItem, string> column in _measureTable.Columns)
                    {
                        csv.Append(column.Name);
                        csv.Append(',');
                    }
                    csv.Remove(csv.Length - 1, 1);
                    csv.Append(System.Environment.NewLine);

                    int i = 0;
                    while (i < _measures.Count)
                    {
                        if (taskContext.CancelRequested)
                        {
                            taskContext.Cancel();
                            return;
                        }

                        csv.Append(_measures[i].TimeOfMeasure.ToString(SR.DateTimeStringFormat));
                        csv.Append(',');
                        csv.Append(_measures[i].Label);
                        csv.Append(',');
                        csv.Append(_measures[i].Info);
                        csv.Append(',');
                        csv.Append(_measures[i].MeasureValue);
                        csv.Append(System.Environment.NewLine);

                        taskContext.ReportProgress(new BackgroundTaskProgress((int)(((double)(i + 1) / (double)_measures.Count) * 100.0), "Writing to File..."));
                        i++;
                    }

                    StreamWriter writer = new StreamWriter(fs);

                    writer.Write(csv.ToString());
                    writer.Flush();
                    writer.Close();


                    fs.Close();
                }

                taskContext.Complete(null);

                }, true);

            ProgressDialog.Show(task, true, this.Host.DesktopWindow);
        }

        public void AddRecordGeneratorStatistic(RecordGeneratorStatistic newStat)
        {
            _stats.Add(newStat); // eventually replace with type specific lists that only keep max moving avg number of entries
            int matchIndex = -1;
            matchIndex = _statTable.Items.FindIndex(delegate(RecordGeneratorStatistic statsItem) { return (statsItem.Entity == newStat.Entity) && (statsItem.Type == newStat.Type); });
            if (matchIndex != -1)
            {
                _statTable.Items[matchIndex] = newStat;
            }
            else
            {
                _statTable.Items.Add(newStat);
            }

            EventsHelper.Fire(_newStatisticAddedEvent, this, new NewStatisticAddedEventArgs(newStat));
        }

        private bool Running
        {
            set 
            {
                _running = value;
                this.UpdateButtons(_running);
                if (_running == false)
                    EventsHelper.Fire(_userStoppedGenerationEvent, this, EventArgs.Empty);
            }
        }

        private void UpdateButtons(bool running)
        {
            if (running == true)
            {
                this.StopEnabled = true;
                this.StartEnabled = false;
                this.StatsExportEnabled = false;
                this.MeasureExportEnabled = false;
            }   
            else
            {
                this.StopEnabled = false;
                this.StartEnabled = true;
                if (_statTable.Items.Count != 0)
                    this.StatsExportEnabled = true;
                if (_measureTable.Items.Count != 0)
                    this.MeasureExportEnabled = true;
            }
        }

        private void GeneratorStoppedEventHandler(object sender, EventArgs e)
        {
            this.Running = false;
        }

        
    }
}
