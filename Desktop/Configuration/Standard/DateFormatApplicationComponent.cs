#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Globalization;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="DateFormatApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DateFormatApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DateFormatApplicationComponent class
	/// </summary>
	[AssociateView(typeof(DateFormatApplicationComponentViewExtensionPoint))]
	public sealed class DateFormatApplicationComponent : ConfigurationApplicationComponent
	{
		public enum DateFormatOptions { Custom = 0, SystemShort, SystemLong };
		
		private string _customFormat;
		private DateFormatOptions _formatOption;
		private List<string> _availableCustomFormats;

		public DateFormatApplicationComponent()
		{
			_availableCustomFormats = new List<string>();
			_formatOption = DateFormatOptions.SystemShort;
			_customFormat = "";
		}
		
		public IEnumerable<string> AvailableCustomFormats
		{
			get { return _availableCustomFormats; }
		}

		private string SystemLongFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern; }
		}

		private string SystemShortFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern; }
		}

		public DateFormatOptions FormatOption
		{
			get { return _formatOption; }
			set
			{
				if (_formatOption == value)
					return;

				this.Modified = true;

				_formatOption = value;
				NotifyPropertyChanged("FormatOption");
			}
		}

		public string SelectedCustomFormat
		{
			get { return _customFormat; }
			set
			{
				if (_customFormat == value)
					return;

				if (!_availableCustomFormats.Contains(value))
					throw new ArgumentException(SR.InvalidCustomDateFormat);

				this.Modified = true;

				_customFormat = value;
				NotifyPropertyChanged("SelectedCustomFormat");
			}
		}

		public bool CustomFormatsEnabled
		{
			get { return _availableCustomFormats.Count > 0; }
		}

		public string DateFormat
		{
			get
			{
				if (_formatOption == DateFormatOptions.Custom)
					return _customFormat;

				if (_formatOption == DateFormatOptions.SystemLong)
					return this.SystemLongFormat;

				return this.SystemShortFormat;
			}
		}

		public string SampleDate
		{
			get
			{
				return Platform.Time.ToString(this.DateFormat);
			}
		}

		public override void Save()
		{
			//Save the settings to the persistent store.
			Format.DateFormat = this.DateFormat;
		}

		public override void Start()
		{
			base.Start();

			foreach (string format in CustomDateFormatSettings.Default.AvailableFormats)
			{
				if (!String.IsNullOrEmpty(format))
					_availableCustomFormats.Add(format);
			}

			if (_availableCustomFormats.Contains(this.SystemShortFormat))
				_availableCustomFormats.Remove(this.SystemShortFormat);

			if (_availableCustomFormats.Contains(this.SystemLongFormat))
				_availableCustomFormats.Remove(this.SystemLongFormat);

			//always select a custom format for display, regardless of whether or not it is going to be used.
			//The view should restrict the user to only be allowed to select from the list.
			if (_availableCustomFormats.Count > 0)
				_customFormat = _availableCustomFormats[0];

			if (Format.DateFormat == this.SystemLongFormat)
			{
				_formatOption = DateFormatOptions.SystemLong;
			}
			else if (!String.IsNullOrEmpty(Format.DateFormat) && Format.DateFormat != this.SystemShortFormat)
			{
				_formatOption = DateFormatOptions.Custom;
				_customFormat = Format.DateFormat;

				//if, for some reason, the current format is not in the custom list, add it.
				if (!_availableCustomFormats.Contains(_customFormat))
					_availableCustomFormats.Add(_customFormat);
			}
			else
			{
				_formatOption = DateFormatOptions.SystemShort;
			}
		}

		public override void Stop()
		{
			base.Stop();
		}
	}
}
