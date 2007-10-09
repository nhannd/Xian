namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IInputController
	{
		bool ProcessMessage(IInputMessage message);
	}
}
