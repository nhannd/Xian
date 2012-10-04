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
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator
{
    [ExtensionOf(typeof(EntityRecordGeneratorExtensionPoint))]
    public class PatientGenerator : IEntityRecordGeneratorLauncher, IEntityRecordGenerator
    {
        private RisRandomDataGenerator _risDataGenerator;
        private double _lastPatientGenerationTime;
        private string _displayName;

        public PatientGenerator()
        {
            _displayName = SR.DisplayNamePatientGenerator;
        }

        private PatientGenerator(RisRandomDataGenerator randomizer) : this()
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
            get { return new IEntityGeneratorSettingsList[] { new PatientGeneratorSettings() }; }
        }

        public IEntityRecordGenerator GetInitializedCopy(IEnumerable<IEntityGeneratorSettingsList> settings)
        {
            RisRandomDataGenerator randomizer = new RisRandomDataGenerator(true);
            randomizer.EntitySettings = settings;

            return new PatientGenerator(randomizer);
        }

        #endregion

        #region IEntityRecordGenerator Members

        public IEnumerable<RecordGeneratorStatistic> Run()
        {
            List<RecordGeneratorStatistic> stats = new List<RecordGeneratorStatistic>();

            long start = DateTime.Now.Ticks;
            _risDataGenerator.GeneratePatient();
            long stop = DateTime.Now.Ticks;
            TimeSpan span = new TimeSpan(stop - start);
            _lastPatientGenerationTime = span.TotalSeconds;
            stats.Add(new RecordGeneratorStatistic(DateTime.Now, GeneratorEntity.Patient, GeneratorStatisticType.RandomRecordGeneration, _lastPatientGenerationTime));
            foreach (InsertionStatistic insertStat in _risDataGenerator.LastPatientInsertionTimes)
            {
                stats.Add(new RecordGeneratorStatistic(insertStat.TimeOfSample, GeneratorEntity.Patient, GeneratorStatisticType.Insertion, insertStat.ExecutionTime));
            }

            return stats;
        }

        public uint EntitiesOfInterestGeneratedInLastRun
        {
            get { return 1; }
        }

        #endregion

        
    }
}
