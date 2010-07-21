#if UNIT_TESTS
//#define UNHANDLEDEXCEPTIONGUITESTS

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Tests
{
#if UNHANDLEDEXCEPTIONGUITESTS

	[MenuAction("throw", "global-menus/MenuFile/Throw from UI Thread", "Throw")]
	[MenuAction("throwFromThread", "global-menus/MenuFile/Throw from Thread", "ThrowFromThread")]
	[MenuAction("throwFromBackgroundTask", "global-menus/MenuFile/Throw from BackgroundTask", "ThrowFromBackgroundTask")]
	[MenuAction("throwFromThreadPool", "global-menus/MenuFile/Throw from Thread Pool", "ThrowFromThreadPool")]
	[MenuAction("throwAndReport", "global-menus/MenuFile/Throw and Report", "ThrowAndReport")]
	[MenuAction("showMessageBoxAndThrow", "global-menus/MenuFile/Show Message Box and Throw", "showMessageBoxAndThrow")]
	//[MenuAction("throwAndContinue", "global-menus/MenuFile/Throw and Report", "ThrowAndContinue")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	internal class UnhandledExceptionTestTool : Tool<IDesktopToolContext>
	{
		public UnhandledExceptionTestTool()
		{
		}

		private void Throw(string message)
		{
			throw new TestException1();
		}

		public void Throw()
		{
			Throw("Throw from UI thread");
		}

		public void ThrowAndReport()
		{
			try
			{
				Throw("Caught exception.");
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void ThrowFromThread()
		{
			Thread thread = new Thread(() => Throw("Thread error."));
			thread.IsBackground = true;
			thread.Start();
		}

		public void ThrowFromBackgroundTask()
		{
			BackgroundTask task = new BackgroundTask(ignore => Throw("Background task error."), false);
			task.Run();
		}

		public void ThrowFromThreadPool()
		{
			ThreadPool.QueueUserWorkItem(ignore => Throw("Thread pool error."));
		}

		public void showMessageBoxAndThrow()
		{

			ThreadPool.QueueUserWorkItem(delegate
			                             	{
			                             		Thread.Sleep(1500);
			                             		Throw("Thread pool error.");
			                             	});

			this.Context.DesktopWindow.ShowMessageBox("Throw!", MessageBoxActions.Ok);
		}
	}

	internal class TestException1 : Exception
	{ }

	[ExceptionPolicyFor(typeof(TestException1))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	internal class UnhandledExceptionTestPolicy : IExceptionPolicy
	{
		#region IExceptionPolicy Members

		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			exceptionHandlingContext.ShowMessageBox(e.GetType().FullName);
		}

		#endregion
	}

#endif
}
#endif