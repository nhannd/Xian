namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseButtonHandler
	{
		bool Start(IMouseInformation mouseInformation);
		bool Track(IMouseInformation mouseInformation);
		bool Stop(IMouseInformation mouseInformation);
		void Cancel();

		// TODO: Convert these two methods into one enum
		bool SuppressContextMenu { get; }
		bool ConstrainToTile { get; }
	}
}
