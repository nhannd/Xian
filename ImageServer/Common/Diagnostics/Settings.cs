using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Diagnostics
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
    }
}