using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Platform.Log("RebuildDatabase application started up");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RebuildDatabaseForm());
        }
    }
}