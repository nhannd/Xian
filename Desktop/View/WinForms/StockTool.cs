using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.View.WinForms
{
    public abstract class StockTool : Tool<IDesktopToolContext>
    {
        public StockTool()
        {
			if(Application.GuiToolkitID != GuiToolkitID.WinForms)
			{
				// this tool is not supported for other toolkits
				// so it should not be created
				throw new NotSupportedException();
			}
        }
   }
}
