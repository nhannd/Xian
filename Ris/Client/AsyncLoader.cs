using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class AsyncLoader : IDisposable
	{
		private BackgroundTask _loaderTask;
		private Action<Exception> _continuationCode;

		public void Run(Action<object> asyncCode, Action<Exception> continuationCode)
		{
			// clear any previous task
			Clear();

			_continuationCode = continuationCode;

			_loaderTask = new BackgroundTask(
				delegate (IBackgroundTaskContext context)
				{
					try
					{
						asyncCode(null);
						context.Complete();
					}
					catch(Exception e)
					{
						context.Error(e);
					}
				}, false);

			_loaderTask.Terminated += TerminatedEventHandler;
			_loaderTask.Run();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Clear();
		}

		#endregion

		private void TerminatedEventHandler(object sender, BackgroundTaskTerminatedEventArgs args)
		{
			Exception error = (args.Reason == BackgroundTaskTerminatedReason.Exception) ? args.Exception : null;
			if(_continuationCode != null)
			{
				_continuationCode(error);
			}
		}

		private void Clear()
		{
			if (_loaderTask != null)
			{
				_continuationCode = null;
				_loaderTask.Terminated -= TerminatedEventHandler;
				_loaderTask.Dispose();
				_loaderTask = null;
			}
		}

	}
}
