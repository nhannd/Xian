using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "folderexplorer-folders-contextmenu/Properties", "Launch")]
	[Tooltip("launch", "Folder Properties")]
	[IconSet("launch", IconScheme.Colour, "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png")]
	[EnabledStateObserver("launch", "Enabled", "EnabledChanged")]
	//[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class WorklistPropertiesTool : Tool<IFolderExplorerGroupToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		#region API Methods and Properties

		public override void Initialize()
		{
			base.Initialize();

			this.Context.SelectedFolderChanged += delegate { this.Enabled = this.Context.SelectedFolder != null; };
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		#endregion

		public void Launch()
		{
			try
			{
				WorklistSummaryComponent component;
				if (this.Context.SelectedFolder is FolderTreeNode.ContainerFolder)
				{
					WorklistAdminDetail worklistDetail = new WorklistAdminDetail(null, this.Context.SelectedFolder.Name, "Container folder", null);
					component = new WorklistSummaryComponent(worklistDetail, false);
				}
				else
				{
					IWorklistFolder folder = (IWorklistFolder)this.Context.SelectedFolder;

					if (folder.WorklistRef == null)
					{
						WorklistAdminDetail worklistDetail = new WorklistAdminDetail(null, folder.Name, "Static folder", null);
						component = new WorklistSummaryComponent(worklistDetail, false);
					}
					else
					{
						WorklistAdminDetail worklistDetail = null;
						Platform.GetService<IWorklistAdminService>(
							delegate(IWorklistAdminService service)
							{
								LoadWorklistForEditResponse response = service.LoadWorklistForEdit(new LoadWorklistForEditRequest(folder.WorklistRef));
								worklistDetail = response.Detail;
							});

						component = new WorklistSummaryComponent(worklistDetail, worklistDetail.IsUserWorklist == false);
					}
				}

				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, "Worklist Properties");
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
