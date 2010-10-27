#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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