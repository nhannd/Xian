using System.Collections.Generic;
using System.IO;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.BaseTools
{
	public static class StudyFilterComponentLoadHelper
	{
		public static bool Load(StudyFilterComponent component, IDesktopWindow desktop, params string[] paths)
		{
			return Load(component, desktop, (IEnumerable<string>) paths);
		}

		public static bool Load(StudyFilterComponent component, IDesktopWindow desktop, IEnumerable<string> paths)
		{
			return Load(component, desktop, true, paths);
		}

		public static bool Load(StudyFilterComponent component, IDesktopWindow desktop, bool allowCancel, params string[] paths)
		{
			return Load(component, desktop, allowCancel, (IEnumerable<string>) paths);
		}

		public static bool Load(StudyFilterComponent component, IDesktopWindow desktop, bool allowCancel, IEnumerable<string> paths)
		{
			bool success = false;

			BackgroundTask task = new BackgroundTask(LoadWorker, allowCancel, new State(component, paths));
			task.Terminated += delegate(object sender, BackgroundTaskTerminatedEventArgs e) { success = e.Reason == BackgroundTaskTerminatedReason.Completed; };
			ProgressDialog.Show(task, desktop, true, ProgressBarStyle.Continuous);

			return success;
		}

		private static void LoadWorker(IBackgroundTaskContext context)
		{
			State state = context.UserState as State;
			if (state == null)
			{
				context.Cancel();
				return;
			}

			context.ReportProgress(new BackgroundTaskProgress(0, 1000, SR.MessageLoading));
			if (context.CancelRequested)
			{
				context.Cancel();
				return;
			}

			List<string> fileList = new List<string>();
			foreach (string path in state.Paths)
				fileList.AddRange(EnumerateFiles(path));

			for (int n = 0; n < fileList.Count; n++)
			{
				context.ReportProgress(new BackgroundTaskProgress(n, fileList.Count, SR.MessageLoading));
				if (context.CancelRequested)
				{
					context.Cancel();
					return;
				}
				state.SynchronizationContext.Send(delegate { state.Component.Load(fileList[n]); }, null);
			}

			if (context.CancelRequested)
			{
				context.Cancel();
				return;
			}

			context.Complete();
		}

		private static IEnumerable<string> EnumerateFiles(string path)
		{
			if (File.Exists(path))
			{
				yield return path;
			}
			if (Directory.Exists(path))
			{
				foreach (string directory in Directory.GetDirectories(path))
					foreach (string filename in EnumerateFiles(directory))
						yield return filename;
				foreach (string filename in Directory.GetFiles(path))
					yield return filename;
			}
		}

		private class State
		{
			public readonly StudyFilterComponent Component;
			public readonly IEnumerable<string> Paths;
			public readonly SynchronizationContext SynchronizationContext;

			public State(StudyFilterComponent component, IEnumerable<string> paths)
			{
				this.Component = component;
				this.Paths = paths;
				this.SynchronizationContext = SynchronizationContext.Current;
			}
		}
	}
}