namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseWheelHandler
	{
		void Start();

		void Wheel(int wheelDelta);

		void Stop();

		//TODO: this could be a global setting for all mouse wheel activity.  Hide it in TileController for now.
		uint StopDelayMilliseconds { get; }
	}
}
