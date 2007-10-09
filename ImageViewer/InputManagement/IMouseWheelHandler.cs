namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseWheelHandler
	{
		void Start();

		void Wheel(int wheelDelta);

		void Stop();

		uint StopDelayMilliseconds { get; }
	}
}
