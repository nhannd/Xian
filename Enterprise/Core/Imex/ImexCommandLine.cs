using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    class ImexCommandLine : CommandLine
    {
        private string _path;
        private bool _import;
        private bool _export;

        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [CommandLineParameter("import", "i", "Specifies that data should be imported")]
        public bool Import
        {
            get { return _import; }
            set { _import = value; }
        }

        [CommandLineParameter("export", "e", "Specifies that data should be exported")]
        public bool Export
        {
            get { return _export; }
            set { _export = value; }
        }
    }
}
