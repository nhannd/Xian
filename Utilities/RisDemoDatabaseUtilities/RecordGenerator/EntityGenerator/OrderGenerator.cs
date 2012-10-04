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
using ClearCanvas.Healthcare;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator
{
    [ExtensionOf(typeof(EntityRecordGeneratorExtensionPoint))]
    public class OrderGenerator : IEntityRecordGeneratorLauncher, IEntityRecordGenerator
    {
        private double _lastPatientGenerationTime;
        private double _lastVisitGenerationTime;
        private double _lastOrderGenerationTime;
        private uint _numberOfOrdersGeneratedInLastRun;
        private string _displayName;
        private RisRandomDataGenerator _risDataGenerator;

        public OrderGenerator()
        {
            _displayName = SR.DisplayNameOrderGenerator;
            _numberOfOrdersGeneratedInLastRun = 0;
        }

        private OrderGenerator(RisRandomDataGenerator randomizer) : this()
        {
            _risDataGenerator = randomizer;
        }

        #region IEntityRecordGeneratorLauncher Members

        public string DisplayName
        {
            get { return _displayName; }
        }

        public IEnumerable<IEntityGeneratorSettingsList> RandomizerSettings
        {
            get
            {
                List<IEntityGeneratorSettingsList> settingsCollection = new List<IEntityGeneratorSettingsList>();
                settingsCollection.Add(new PatientGeneratorSettings());
                settingsCollection.Add(new VisitGeneratorSettings());
                settingsCollection.Add(new OrderGeneratorSettings());
                return settingsCollection;
            }
        }

        public IEntityRecordGenerator GetInitializedCopy(IEnumerable<IEntityGeneratorSettingsList> settings)
        {
            RisRandomDataGenerator randomizer = new RisRandomDataGenerator(true);
            randomizer.EntitySettings = settings;

            return new OrderGenerator(randomizer);
        }

        #endregion

        #region IEntityRecordGenerator Members

        public IEnumerable<RecordGeneratorStatistic> Run()
        {
            List<RecordGeneratorStatistic> stats = new List<RecordGeneratorStatistic>();
            _numberOfOrdersGeneratedInLastRun = 0;
            Patient patient = null;
            Visit[] visits = null;

            long start;
            long stop;
            TimeSpan span;

            start = DateTime.Now.Ticks;
            patient = _risDataGenerator.GeneratePatient();
            stop = DateTime.Now.Ticks;
            span = new TimeSpan(stop - start);
            _lastPatientGenerationTime = span.TotalSeconds;
            stats.Add(new RecordGeneratorStatistic(DateTime.Now, GeneratorEntity.Patient, GeneratorStatisticType.RandomRecordGeneration, _lastPatientGenerationTime));
            foreach(InsertionStatistic insertStat in _risDataGenerator.LastPatientInsertionTimes)
            {
                stats.Add(new RecordGeneratorStatistic(insertStat.TimeOfSample, GeneratorEntity.Patient, GeneratorStatisticType.Insertion, insertStat.ExecutionTime));
            }

            start = DateTime.Now.Ticks;
            visits = _risDataGenerator.GenerateVisits(patient);
            stop = DateTime.Now.Ticks;
            span = new TimeSpan(stop - start);
            _lastVisitGenerationTime = span.TotalSeconds;
            stats.Add(new RecordGeneratorStatistic(DateTime.Now, GeneratorEntity.Visit, GeneratorStatisticType.RandomRecordGeneration, _lastVisitGenerationTime));
            foreach(InsertionStatistic insertStat in _risDataGenerator.LastVisitInsertionTimes)
            {
                stats.Add(new RecordGeneratorStatistic(insertStat.TimeOfSample, GeneratorEntity.Visit, GeneratorStatisticType.Insertion, insertStat.ExecutionTime));
            }

            start = DateTime.Now.Ticks;
            _risDataGenerator.GeneratePlacedOrders(visits);
            stop = DateTime.Now.Ticks;
            span = new TimeSpan(stop - start);
            _lastOrderGenerationTime = span.TotalSeconds;
            stats.Add(new RecordGeneratorStatistic(DateTime.Now, GeneratorEntity.Order, GeneratorStatisticType.RandomRecordGeneration, _lastOrderGenerationTime));
            foreach (InsertionStatistic insertStat in _risDataGenerator.LastOrderInsertionTimes)
            {
                stats.Add(new RecordGeneratorStatistic(insertStat.TimeOfSample, GeneratorEntity.Order, GeneratorStatisticType.Insertion, insertStat.ExecutionTime));
                _numberOfOrdersGeneratedInLastRun++;
            }
            
            return stats;
        }

        public uint EntitiesOfInterestGeneratedInLastRun
        {
            get { return _numberOfOrdersGeneratedInLastRun; }
        }

        #endregion
    }
}
