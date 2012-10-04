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
using ClearCanvas.Utilities.RisDemoDatabaseUtilities;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator
{
    public interface IEntityRecordGenerator
    {
        IEnumerable<RecordGeneratorStatistic> Run();
        /// <summary>
        /// The number of Entities Of Interest Generated during last execution of Run.  0 in the event that Run has not yet been called.
        /// </summary>
        uint EntitiesOfInterestGeneratedInLastRun { get; }
        /// <summary>
        /// Returns a collection of EntityGeneratorSettingsLists that the EntityRecordGenerator makes use of.
        /// </summary>
    }
}
