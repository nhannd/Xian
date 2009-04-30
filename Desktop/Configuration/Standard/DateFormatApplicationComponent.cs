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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Globalization;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="DateFormatApplicationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DateFormatApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// Component that allows the date format to be set for the application.
	/// </summary>
	[AssociateView(typeof(DateFormatApplicationComponentViewExtensionPoint))]
	public sealed class DateFormatApplicationComponent : ConfigurationApplicationComponent
	{
		/// <summary>
		/// An enumeration of date format options.
		/// </summary>
		public enum DateFormatOptions
		{
			/// <summary>
			/// A custom date format.
			/// </summary>
			Custom = 0, 
			
			/// <summary>
			/// The 'short' format defined by the system (varies by user).
			/// </summary>
			SystemShort,

			/// <summary>
			/// The 'long' format defined by the system (varies by user).
			/// </summary>
			SystemLong
		};
		
		private string _customFormat;
		private DateFormatOptions _formatOption;
		private List<string> _availableCustomFormats;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DateFormatApplicationComponent()
		{
			_availableCustomFormats = new List<string>();
			_formatOption = DateFormatOptions.SystemShort;
			_customFormat = "";
		}

		private static string SystemLongFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern; }
		}

		private static string SystemShortFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern; }
		}

		#region Presentation Model

		/// <summary>
		/// Gets the available custom date formats.
		/// </summary>
		public IEnumerable<string> AvailableCustomFormats
		{
			get { return _availableCustomFormats; }
		}

		/// <summary>
		/// Get the currently selected format option (<see cref="DateFormatOptions"/>).
		/// </summary>
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

		/// <summary>
		/// Gets the currently selected custom format.
		/// </summary>
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

		/// <summary>
		/// Gets whether or not the 'custom' format option is enabled.
		/// </summary>
		public bool CustomFormatsEnabled
		{
			get { return _availableCustomFormats.Count > 0; }
		}

		/// <summary>
		/// Gets the currently selected date format.
		/// </summary>
		public string DateFormat
		{
			get
			{
				if (_formatOption == DateFormatOptions.Custom)
					return _customFormat;

				if (_formatOption == DateFormatOptions.SystemLong)
					return SystemLongFormat;

				return SystemShortFormat;
			}
		}

		/// <summary>
		/// Returns a 'sample date', formatted according to the currently selected format.
		/// </summary>
		public string SampleDate
		{
			get
			{
				return Platform.Time.ToString(this.DateFormat);
			}
		}

		#endregion

		/// <summary>
		/// Saves the changes.
		/// </summary>
		public override void Save()
		{
			//Save the settings to the persistent store.
			Format.DateFormat = this.DateFormat;
		}

		/// <summary>
		/// Starts/initializes the component.
		/// </summary>
		public override void Start()
		{
			base.Start();

			foreach (string format in CustomDateFormatSettings.Default.AvailableFormats)
			{
				if (!String.IsNullOrEmpty(format))
					_availableCustomFormats.Add(format);
			}

			if (_availableCustomFormats.Contains(SystemShortFormat))
				_availableCustomFormats.Remove(SystemShortFormat);

			if (_availableCustomFormats.Contains(SystemLongFormat))
				_availableCustomFormats.Remove(SystemLongFormat);

			//always select a custom format for display, regardless of whether or not it is going to be used.
			//The view should restrict the user to only be allowed to select from the list.
			if (_availableCustomFormats.Count > 0)
				_customFormat = _availableCustomFormats[0];

			if (Format.DateFormat == SystemLongFormat)
			{
				_formatOption = DateFormatOptions.SystemLong;
			}
			else if (!String.IsNullOrEmpty(Format.DateFormat) && Format.DateFormat != SystemShortFormat)
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
	}
}
