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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

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
		/// Static helper method to prompt user for time in a single line of code.
		/// </summary>
		/// <param name="desktopWindow"></param>
		/// <param name="title"></param>
		/// <param name="time"></param>
		/// <param name="allowNull"></param>
		/// <returns></returns>
		public static bool PromptForTime(IDesktopWindow desktopWindow, string title, bool allowNull, ref DateTime? time)
		{
			DateTimeEntryComponent component = new DateTimeEntryComponent(time, allowNull);
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

		#region Presentation Model

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
			Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
