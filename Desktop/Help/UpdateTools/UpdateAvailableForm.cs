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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	internal partial class UpdateAvailableForm : Form
	{
		private string _downloadLink;

		private UpdateAvailableForm()
		{
			InitializeComponent();
		}

		public static void Show(string text, string downloadLink)
		{
			UpdateAvailableForm form = new UpdateAvailableForm();
			form._text.Text = text;
			form._downloadLink = downloadLink ?? "";

			form.ShowDialog();
		}

		private void OnOk(object sender, EventArgs e)
		{
			Close();
		}

		private void OnDownloadNow(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (String.IsNullOrEmpty(_downloadLink))
				return;

			try
			{
				Process.Start(_downloadLink);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, String.Format("Failed to navigate to download link '{0}'.", _downloadLink));
			}
		}
	}
}
