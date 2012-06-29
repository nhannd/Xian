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
using System.Diagnostics;
using System.Text;

namespace ClearCanvas.ImageServer.Core.Diagnostics
{
    static public class Settings
    {
        static public bool SimulateEditError
        {
            get { return DiagnosticSettings.Default.SimulateEditError; }
        }

        static public bool SimulateFileIOError
        {
            get { return DiagnosticSettings.Default.SimulateFileIOError; }
        }

        static public bool SimulateTierMigrationError
        {
            get { return DiagnosticSettings.Default.SimulateTierMigrationError; }
        }

        static public bool SimulateFileCorruption
        {
            get { return DiagnosticSettings.Default.SimulateFileCorruption; }
        }
    }
}