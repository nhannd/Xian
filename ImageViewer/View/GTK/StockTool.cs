using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.ImageViewer.Tools;

namespace ClearCanvas.ImageViewer.View.GTK
{
    public abstract class StockTool : Tool
    {
        protected WorkstationView _mainView;

        public StockTool()
        {
			// check if the main is *the* view implemented by this plugin
			IWorkstationView view = WorkstationModel.View;
			if(view is WorkstationView)
			{
				_mainView = (WorkstationView)view;
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
