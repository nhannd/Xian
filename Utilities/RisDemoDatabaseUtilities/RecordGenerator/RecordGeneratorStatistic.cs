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
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    public class RecordGeneratorStatistic
    {
        private GeneratorStatisticType _type;
        private GeneratorEntity _entity;
        private DateTime _timeOfSample;
        private double _executionTime;

        public RecordGeneratorStatistic(DateTime TimeOfSample, GeneratorEntity Entity, GeneratorStatisticType Type, double ExecutionTime)
        {
            _timeOfSample = TimeOfSample;
            _entity = Entity;
            _type = Type;
            _executionTime = ExecutionTime;
        }

        public DateTime TimeOfSample
        {
            get { return _timeOfSample;}
        }
        
        public GeneratorEntity Entity
        {
            get { return _entity; }
        }
        
        public GeneratorStatisticType Type
        {
            get { return _type; }
        }

        public double ExecutionTime
        {
            get { return _executionTime; }
        }
    }
}
