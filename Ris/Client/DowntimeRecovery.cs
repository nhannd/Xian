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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public static class DowntimeRecovery
	{
		private static bool _inDowntimeRecoveryMode;
		private static event EventHandler _inDowntimeRecoveryModeChanged;

		/// <summary>
		/// Gets a value indicating whether the client application is running in downtime recovery mode.
		/// </summary>
		public static bool InDowntimeRecoveryMode
		{
			get { return _inDowntimeRecoveryMode; }
			internal set
			{
				if(_inDowntimeRecoveryMode != value)
				{
					_inDowntimeRecoveryMode = value;
					EventsHelper.Fire(_inDowntimeRecoveryModeChanged, null, EventArgs.Empty);
				}
			}
		}

		public static event EventHandler InDowntimeRecoveryModeChanged
		{
			add { _inDowntimeRecoveryModeChanged += value; }
			remove { _inDowntimeRecoveryModeChanged -= value; }
		}
	}
}
