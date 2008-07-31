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
