using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    class ImporterCommandLine : CommandLine
    {
        private string _path;

        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
