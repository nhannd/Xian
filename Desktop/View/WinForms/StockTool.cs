using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class StockTool : Tool<IDesktopToolContext>
    {
        public StockTool()
        {
			if(Application.GuiToolkit != GuiToolkitID.WinForms)
			{
				// this tool is not supported for other toolkits
				// so it should not be created
				throw new NotSupportedException();
			}
        }
   }
}
