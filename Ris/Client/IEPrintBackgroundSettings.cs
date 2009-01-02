using Microsoft.Win32;

namespace ClearCanvas.Ris.Client
{
	public class IEPrintBackgroundSettings
	{
		private readonly RegistryKey _iePrintBackgroundKey;
		private readonly string _iePrintBackgroundKeyPath = @"Software\Microsoft\Internet Explorer\Main";

		private bool _printBackground;
		private string _oldPrintBackground;

		public IEPrintBackgroundSettings()
			: this(true)
		{
		}

		public IEPrintBackgroundSettings(bool printBackground)
		{
			_iePrintBackgroundKey = Registry.CurrentUser.OpenSubKey(_iePrintBackgroundKeyPath, true);
			_printBackground = printBackground;

			if(_iePrintBackgroundKey != null)
			{
				_oldPrintBackground = (string) _iePrintBackgroundKey.GetValue("Print_Background");

				_iePrintBackgroundKey.SetValue("Print_Background", _printBackground ? "yes" : "no");
			}
		}

		public void Revert()
		{
			if (_iePrintBackgroundKey != null)
			{
				_iePrintBackgroundKey.SetValue("Print_Background", _oldPrintBackground);
			}
		}
	}
}