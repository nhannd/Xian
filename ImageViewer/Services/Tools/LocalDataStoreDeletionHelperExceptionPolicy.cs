using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
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
