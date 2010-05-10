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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="DateTimeEntryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DateTimeEntryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DateTimeEntryComponent class
	/// </summary>
	[AssociateView(typeof(DateTimeEntryComponentViewExtensionPoint))]
	public class DateTimeEntryComponent : ApplicationComponent
	{
		/// <summary>
		/// Delegate used to validate date time.
		/// </summary>
		public delegate bool DateTimeValidationDelegate(DateTime? dateTime, out string failedReason);

		/// <summary>
		/// Static helper method to prompt user for time in a single line of code.
		/// </summary>
		/// <param name="desktopWindow"></param>
		/// <param name="title"></param>
		/// <param name="time"></param>
		/// <param name="allowNull"></param>
		/// <param name="validationCallback"></param>
		/// <returns></returns>
		public static bool PromptForTime(IDesktopWindow desktopWindow, string title, bool allowNull, ref DateTime? time, DateTimeValidationDelegate validationCallback)
		{
			var component = new DateTimeEntryComponent(time, allowNull) {ValidationCallback = validationCallback};
			if (LaunchAsDialog(desktopWindow, component, title)
				== ApplicationComponentExitCode.Accepted)
			{
				time = component.DateAndTime;
				return true;
			}
			return false;
		}



		private DateTime? _dateAndTime;
		private readonly bool _allowNull;


		/// <summary>
		/// Constructor
		/// </summary>
		public DateTimeEntryComponent(DateTime? initialValue, bool allowNull)
		{
			_dateAndTime = initialValue;
			_allowNull = allowNull;
		}

		public override void Start()
		{
			if (this.ValidationCallback != null)
			{
				this.Validation.Add(new ValidationRule("DateAndTime",
					delegate
						{
							string failedReason;
							var success = ValidationCallback(_dateAndTime, out failedReason);
							return new ValidationResult(success, failedReason);
						}));
			}

			base.Start();
		}

		#region Presentation Model

		public DateTimeValidationDelegate ValidationCallback { get; set; }

		public DateTime? DateAndTime
		{
			get { return _dateAndTime; }
			set { _dateAndTime = value;  }
		}

		public bool AllowNull
		{
			get { return _allowNull; }
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
