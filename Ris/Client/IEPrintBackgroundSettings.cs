#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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