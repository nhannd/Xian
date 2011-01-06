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
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public abstract class StockTool : Tool
    {
        //protected WorkstationView _mainView;
        protected DesktopView _mainView;

        public StockTool()
        {
			// check if the main is *the* view implemented by this plugin
			//IWorkstationView view = WorkstationModel.View;
			IDesktopView view = DesktopApplication.View;
			//if(view is WorkstationView)
			if(view is DesktopView)
			{
				//_mainView = (WorkstationView)view;
				_mainView = (DesktopView)view;
			}
			else
			{
				// this tool is not supported for other main views
				// TODO add a message
				throw new NotSupportedException();
			}
        }
   }
}
