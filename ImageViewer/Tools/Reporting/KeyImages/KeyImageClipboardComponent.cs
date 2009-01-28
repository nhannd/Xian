using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal class KeyImageClipboardComponent : ClipboardComponent
	{
		private KeyImageInformation _keyImageInformation;

		public KeyImageClipboardComponent(KeyImageInformation keyImageInformation)
			: base(KeyImageClipboard.ToolbarSite, KeyImageClipboard.MenuSite, keyImageInformation.ClipboardItems, false)
		{
			_keyImageInformation = keyImageInformation;
		}

		public KeyImageInformation KeyImageInformation
		{
			get { return _keyImageInformation; }
			set
			{
				_keyImageInformation = value;
				DataSource = _keyImageInformation.ClipboardItems;
			}
		}
	}
}
