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
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator
{
    public interface IRecordGeneratorHost
    {
        void Initialize(IEntityRecordGenerator generator, uint numberOfEntitiesToBeGenerated, PropertySetDelegate<RecordGeneratorStatistic> setStatisticDelegate);
        void Start();
        void Stop();
        /// <summary>
        /// Notifies that the generation of records has stopped.
        /// </summary>
        event EventHandler GeneratorStoppedEvent;        
    }
}
