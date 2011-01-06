#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public partial class StudyFilterComponent
	{
		public bool Load(IDesktopWindow desktopWindow, params string[] paths)
		{
			return Load(desktopWindow, true, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, params string[] paths)
		{
			return Load(desktopWindow, allowCancel, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, IEnumerable<string> paths)
		{
			return Load(desktopWindow, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, IEnumerable<string> paths, bool recursive)
		{
			return Load(desktopWindow, true, paths, recursive);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths)
		{
			return Load(desktopWindow, allowCancel, paths, true);
		}

		public bool Load(IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths, bool recursive)
		{
			return BackgroundFileLoader.Load(this, desktopWindow, allowCancel, paths, recursive);
		}

		private static class BackgroundFileLoader
		{
			public static bool Load(StudyFilterComponent component, IDesktopWindow desktopWindow, bool allowCancel, IEnumerable<string> paths, bool recursive)
			{
				bool success = false;

				BackgroundTask task = new BackgroundTask(LoadWorker, allowCancel, new State(component, paths, recursive));
				task.Terminated += delegate(object sender, BackgroundTaskTerminatedEventArgs e) { success = e.Reason == BackgroundTaskTerminatedReason.Completed; };
				ProgressDialog.Show(task, desktopWindow, true, ProgressBarStyle.Continuous);

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
					fileList.AddRange(EnumerateFiles(path, state.Recursive));

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

			private static IEnumerable<string> EnumerateFiles(string path, bool recurse)
			{
				if (File.Exists(path))
				{
					yield return path;
				}
				if (recurse && Directory.Exists(path))
				{
					foreach (string directory in Directory.GetDirectories(path))
						foreach (string filename in EnumerateFiles(directory, true))
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
				public readonly bool Recursive;

				public State(StudyFilterComponent component, IEnumerable<string> paths, bool recursive)
				{
					this.Component = component;
					this.Paths = paths;
					this.SynchronizationContext = SynchronizationContext.Current;
					this.Recursive = recursive;
				}
			}
		}
	}
}