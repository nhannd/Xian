using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class KeyboardButtonDownPreview : IInputMessage
	{
		private KeyboardButtonShortcut _buttonShortcut;

		public KeyboardButtonDownPreview(XKeys keyData)
		{
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
