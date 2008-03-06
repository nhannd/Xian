
namespace ClearCanvas.ImageViewer.Clipboard
{
	public static class Clipboard
	{
		public static void Add(IPresentationImage image)
		{
			ClipboardComponent.AddToClipboard(image);
		}

		public static void Add(IDisplaySet displaySet)
		{
			ClipboardComponent.AddToClipboard(displaySet);
		}
	}
}
