using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.BaseTools
{
	internal sealed class ImageViewerToolAttributeProcessor
	{
		private ImageViewerToolAttributeProcessor()
		{
		}

		public static void Process(ImageViewerTool tool)
		{
			Platform.CheckForNullReference(tool, "tool");

			object[] attributes = tool.GetType().GetCustomAttributes(typeof(MouseWheelHandlerAttribute), false);
			if (attributes == null || attributes.Length == 0)
				return;

			MouseWheelHandlerAttribute attribute = (MouseWheelHandlerAttribute)attributes[0];

			tool.MouseWheelStopDelayMilliseconds = attribute.StopDelayMilliseconds;
			tool.MouseWheelShortcut = attribute.Shortcut;
		}
	}
}
