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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.EntityGenerator
{
    public interface IEntityRecordGeneratorLauncher
    {
        string DisplayName { get; }
        IEnumerable<IEntityGeneratorSettingsList> RandomizerSettings { get; }
        IEntityRecordGenerator GetInitializedCopy(IEnumerable<IEntityGeneratorSettingsList> settings);
    }
}
