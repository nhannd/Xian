using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    class ExporterCommandLine : CommandLine
    {
        private string _path;
        private string _dataClass;

        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [CommandLineParameter("class", "c", "Specifies the class of data to export.", Required=true)]
        public string DataClass
        {
            get { return _dataClass; }
            set { _dataClass = value; }
        }

    }
}
