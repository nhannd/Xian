using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public class ExportCommandLine : CommandLine
    {
        private string _path;
        private string _dataClass;
        private bool _allClasses;
        private int _itemsPerFile;


        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [CommandLineParameter("class", "c", "Specifies the class of data to export. Required unless /all is specified.")]
        public string DataClass
        {
            get { return _dataClass; }
            set { _dataClass = value; }
        }

        [CommandLineParameter("all", "a", "Specifies that all data classes should be imported/exported.")]
        public bool AllClasses
        {
            get { return _allClasses; }
            set { _allClasses = value; }
        }

        [CommandLineParameter("i", "Specifies the number of items per file.  If 0 or ommitted, all items will be written to one file.")]
        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
