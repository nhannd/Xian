#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Exception handling policy for <see cref="NoVisibleDisplaySetsException"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(NoVisibleDisplaySetsException))]

	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public sealed class NoVisibleDisplaySetsExceptionHandlingPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NoVisibleDisplaySetsExceptionHandlingPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.Log(LogLevel.Error, e);
			exceptionHandlingContext.ShowMessageBox(SR.MessageNoVisibleDisplaySets);
		}

		#endregion
	}

	/// <summary>
	/// Exception thrown by the <see cref="LayoutManager"/> when no display sets
	/// were added to the <see cref="ILogicalWorkspace"/>.
	/// </summary>
	public class NoVisibleDisplaySetsException : Exception
	{
		internal NoVisibleDisplaySetsException(string message)
			: base(message)
		{
		}
	}
}
