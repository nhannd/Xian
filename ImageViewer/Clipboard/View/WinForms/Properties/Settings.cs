#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Configuration;
using System.Globalization;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms.Properties
{
	[SettingsGroupDescription("View settings in the clipboard component.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class Settings
	{
		public Settings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public int[] CustomColorsArray
		{
			get
			{
				string value = this.CustomColors;
				if (string.IsNullOrEmpty(value))
					return new int[0];

				List<int> list = new List<int>();
				foreach (string s in value.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
				{
					int v;
					if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v))
						list.Add(v);
				}
				return list.ToArray();
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.CustomColors = string.Empty;
					return;
				}

				StringBuilder sb = new StringBuilder();
				foreach (int i in value)
				{
					sb.AppendFormat(i.ToString("x6", CultureInfo.InvariantCulture));
					sb.Append(',');
				}
				this.CustomColors = sb.ToString(0, Math.Max(0, sb.Length - 1));
			}
		}
	}
}