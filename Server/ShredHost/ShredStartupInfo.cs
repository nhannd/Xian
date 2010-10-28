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

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredStartupInfo : MarshalByRefObject
    {
        public ShredStartupInfo(Uri assemblyPath, string shredName, string shredTypeName)
        {
            Platform.CheckForNullReference(assemblyPath, "assemblyPath");
            Platform.CheckForNullReference(shredName, "shredName");
            Platform.CheckForEmptyString(shredName, "shredName");
            Platform.CheckForNullReference(shredTypeName, "shredTypeName");
            Platform.CheckForEmptyString(shredTypeName, "shredTypeName");

            _assemblyPath = assemblyPath;
            _shredName = shredName;
            _shredTypeName = shredTypeName;
        }

        #region Properties
        private Uri _assemblyPath;
        private string _shredName;
        private string _shredTypeName;

        public string ShredTypeName
        {
            get { return _shredTypeName; }
        }
	

        public string ShredName
        {
            get { return _shredName; }
        }
	
        public Uri AssemblyPath
        {
            get { return _assemblyPath; }
        }
        #endregion
    }
}
