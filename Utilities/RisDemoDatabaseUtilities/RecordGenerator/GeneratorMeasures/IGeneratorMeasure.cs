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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.GeneratorMeasures
{
    public interface IGeneratorMeasure : ITool
    {
        RecordGeneratorComponent ConnectedComponent { set; }
    }

    public interface IGeneratorMeasureLauncher
    {
        string DisplayName { get; }
        ICollection<GeneratorMeasureSetting> Settings { get; }
        IGeneratorMeasure GetInitializedCopy(ICollection<GeneratorMeasureSetting> settings);
    }
}
