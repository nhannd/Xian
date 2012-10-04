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

namespace ClearCanvas.VSWizards
{
    public class ToolTemplate
    {
        public string Name;
        public string File;
        public string ToolExtPointClass;
        public string ToolContextInterface;

        public ToolTemplate(string name, string file, string extPointClass, string contextInterface)
        {
            Name = name;
            File = file;
            ToolExtPointClass = extPointClass;
            ToolContextInterface = contextInterface;
        }

        public override string  ToString()
        {
 	         return Name;
        }
    }
}
