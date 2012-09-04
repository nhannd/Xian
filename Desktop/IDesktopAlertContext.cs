#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	public interface IDesktopAlertContext
	{
		void ShowAlertViewer();
		void AcknowledgeAll();
		IconSet GetIcon(AlertLevel level);

	    /// TODO (CR Jun 2012): That's a mouthful :)
        int UnacknowledgedErrorWarningCount { get; }
	}
}
