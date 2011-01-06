#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ReconcileCleanupLauncher:IApplicationRoot
    {
        public void RunApplication(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}