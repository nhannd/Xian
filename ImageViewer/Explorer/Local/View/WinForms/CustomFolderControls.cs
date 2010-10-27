#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	internal class CustomFolderTree : FolderTree
	{
		private event EventHandler<ItemEventArgs<Exception>> _exceptionRaised;

		public event EventHandler<ItemEventArgs<Exception>> ExceptionRaised
		{
			add { _exceptionRaised += value; }
			remove { _exceptionRaised -= value; }
		}

		protected override void HandleBrowseException(Exception exception)
		{
			EventsHelper.Fire(_exceptionRaised, this, new ItemEventArgs<Exception>(exception));
		}

		protected override void HandleInitializationException(Exception exception)
		{
			Platform.Log(LogLevel.Error, exception, "Failed to initialize the {0} control.", this.GetType().Name);
		}
	}

	internal class CustomFolderView : FolderView
	{
		private event EventHandler<ItemEventArgs<Exception>> _exceptionRaised;

		public event EventHandler<ItemEventArgs<Exception>> ExceptionRaised
		{
			add { _exceptionRaised += value; }
			remove { _exceptionRaised -= value; }
		}

		protected override void HandleBrowseException(Exception exception)
		{
			EventsHelper.Fire(_exceptionRaised, this, new ItemEventArgs<Exception>(exception));
		}

		protected override void HandleInitializationException(Exception exception)
		{
			Platform.Log(LogLevel.Error, exception, "Failed to initialize the {0} control.", this.GetType().Name);
		}
	}
}