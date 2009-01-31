using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Exception handling policy for <see cref="OpenStudyException"/>s.
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

	public class NoVisibleDisplaySetsException : Exception
	{
		internal NoVisibleDisplaySetsException(string message)
			: base(message)
		{
		}
	}
}
