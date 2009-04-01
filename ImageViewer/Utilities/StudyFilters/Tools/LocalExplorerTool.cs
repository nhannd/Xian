using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/MenuOpenInStudyFilters", "Open")]
	[Tooltip("Open", "TooltipOpenInStudyFilters")]
	//[IconSet("Open", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
	[ExtensionOf(typeof (LocalImageExplorerToolExtensionPoint))]
	public class LocalExplorerTool : Tool<ILocalImageExplorerToolContext>
	{
		private SynchronizationContext _syncContext;

		public void Open()
		{
			if (_syncContext == null)
				_syncContext = SynchronizationContext.Current;

			List<string> paths = new List<string>();
			foreach (string path in base.Context.SelectedPaths)
				paths.Add(path);

			BackgroundTask task = new BackgroundTask(this.LoadFilterComponent, false, paths.AsReadOnly());
			ProgressDialog.Show(task, base.Context.DesktopWindow, true, ProgressBarStyle.Marquee);
		}

		private void LoadFilterComponent(IBackgroundTaskContext context)
		{
			IList<string> selectedPaths = context.UserState as IList<string>;
			if (selectedPaths == null)
				return;

			StudyFilterComponent component = new StudyFilterComponent();
			component.Load(selectedPaths);
			//for (int n = 0; n < selectedPaths.Count; n++)
			//{
			//    context.ReportProgress(new BackgroundTaskProgress(n, selectedPaths.Count, "SR.MessageLoading..."));
			//    component.Load(selectedPaths[n]);
			//}
			component.Refresh();

			_syncContext.Send(this.AddNewWorkspace, component);
		}

		private void AddNewWorkspace(object component)
		{
			base.Context.DesktopWindow.Workspaces.AddNew((IApplicationComponent) component, SR.StudyFilters);
		}
	}
}