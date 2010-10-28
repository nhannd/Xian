#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.Xml;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines an interface to a class that imports data in CSV format.
    /// </summary>
    public interface ICsvDataImporter
    {
        /// <summary>
        /// Imports the specified list of rows, where each row is a string of comma separated values (CSV).
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="context"></param>
        void Import(List<string> rows, IUpdateContext context);
    }

    /// <summary>
    /// Defines an extension point for CSV data importers.
    /// </summary>
    [ExtensionPoint]
    public class CsvDataImporterExtensionPoint : ExtensionPoint<ICsvDataImporter>
    {
    }

}
