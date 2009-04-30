#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using Microsoft.Win32;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Specifies the header and footer settings for printing from webbrowser controls.
	/// </summary>
	/// <remarks>
	/// TODO: an alternative way to implement this follows, but pvaIn needs to be constructed per 
	/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q267240
	///  but it cannot be constructed in c# code, only through a managed c++ library.
	/// <code>
	/// IWebBrowser2 wb2 = (IWebBrowser2) _webBrowser.ActiveXInstance;
	/// object pvaIn = new object();
	/// object pvaOut = null;
	/// wb2.ExecWB(
	/// 	OLECMDID.OLECMDID_PAGESETUP,
	/// 	OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
	/// 	ref pvaIn,
	/// 	ref pvaOut);
	/// </code>
	/// </remarks>
	public class IEHeaderFooterSettings
	{
		private readonly RegistryKey _iePageSetupKey;
		private readonly string _iePageSetupKeyPath = @"Software\Microsoft\Internet Explorer\PageSetup";

		private readonly string _oldHeader = "";
		private readonly string _oldFooter = "";

		private readonly string _header;
		private readonly string _footer;

		/// <summary>
		/// Page setup without a header and footer.
		/// </summary>
		public IEHeaderFooterSettings()
			: this("", "")
		{
		}

		/// <summary>
		/// Page setup with the specifed header and footer.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="footer"></param>
		public IEHeaderFooterSettings(string header, string footer)
		{
			_iePageSetupKey = Registry.CurrentUser.OpenSubKey(_iePageSetupKeyPath, true);
			_header = header;
			_footer = footer;

			if (_iePageSetupKey != null)
			{
				_oldHeader = (string)_iePageSetupKey.GetValue("header");
				_oldFooter = (string)_iePageSetupKey.GetValue("footer");

				_iePageSetupKey.SetValue("header", _header);
				_iePageSetupKey.SetValue("footer", _footer);
			}
		}

		public void Revert()
		{
			if (_iePageSetupKey != null)
			{
				_iePageSetupKey.SetValue("header", _oldHeader);
				_iePageSetupKey.SetValue("footer", _oldFooter);
			}
		}
	}
}