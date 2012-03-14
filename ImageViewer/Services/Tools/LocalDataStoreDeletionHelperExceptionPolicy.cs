#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExceptionPolicyFor(typeof(LocalDataStoreDeletionHelper.ConnectionLostException))]
	[ExceptionPolicyFor(typeof(LocalDataStoreDeletionHelper.UnableToConnectException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public sealed class LocalDataStoreDeletionHelperExceptionPolicy : IExceptionPolicy
	{
		#region IExceptionPolicy Members

		public void Handle(System.Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (!(e.InnerException is EndpointNotFoundException))
				exceptionHandlingContext.Log(LogLevel.Error, e);

			if (e is LocalDataStoreDeletionHelper.ConnectionLostException)
				exceptionHandlingContext.ShowMessageBox(SR.MessageDeletionHelperConnectionLost);
			else if (e is LocalDataStoreDeletionHelper.UnableToConnectException)
				exceptionHandlingContext.ShowMessageBox(SR.MessageDeletionHelperUnableToConnect);
		}

		#endregion
	}
}
