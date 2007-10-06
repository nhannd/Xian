using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IContextMenuProvider
	{
		ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation);
	}
}
