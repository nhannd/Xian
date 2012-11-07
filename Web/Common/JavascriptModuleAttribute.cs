#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Web.Common
{
    public class JavascriptModuleAttribute : Attribute
    {
        public JavascriptModuleAttribute(string modulePath)
        {
            ModulePath = modulePath;
            ModuleName = ModulePath.Replace('/', '.');
            LoadAsynchronously = true;
        }

        public string ModulePath { get; private set; }
        public string ModuleName { get; private set; }
        public bool LoadAsynchronously { get; set; }
    }
}