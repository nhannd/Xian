using Gtk;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.Workstation.View.GTK
{
	[GuiToolkit(GuiToolkitID.GTK)]
	[ExtensionOf(typeof(ClearCanvas.Common.MessageBoxExtensionPoint))]
	public class MessageBox : IMessageBox
	{
		public MessageBox()
		{
			
		}
		
		public void Show(string msg)
		{
			Window parent = null;
			// check if there is a main view and if it is *the* view implemented by this plugin
			IWorkstationView view = WorkstationModel.View;
			if(view != null && view is WorkstationView)
			{
				parent = ((WorkstationView)view).MainWindow;
			}
			
			MessageDialog mb = new MessageDialog(parent, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, msg);
			mb.Run();
			mb.Destroy();
		}
	}
}
