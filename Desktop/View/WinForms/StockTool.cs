using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class StockTool : Tool
    {
        private DesktopView _mainView;

        public StockTool()
        {
			// check if the main view is *the* view implemented by this plugin
            IDesktopView view = DesktopApplication.View;
			if(view is DesktopView)
			{
				_mainView = (DesktopView)view;
			}
			else
			{
				// this tool is not supported for other main views
				// TODO add a message
				throw new NotSupportedException();
			}
        }

        protected DesktopForm MainForm
        {
            get { return (DesktopForm)_mainView.GuiElement; }
        }

   }
}
