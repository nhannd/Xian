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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Imex
{
    public class ImportCommandLine : CommandLine
    {
        private string _path;
        private string _dataClass;


        [CommandLineParameter(0, "path", Required = true)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [CommandLineParameter("class", "c", "Specifies the class of data to import.", Required = true)]
        public string DataClass
        {
            get { return _dataClass; }
            set { _dataClass = value; }
        }

    }
}
