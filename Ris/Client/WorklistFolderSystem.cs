using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public interface IWorklistFolderSystem : IFolderSystem
	{
		IWorklistFolder AddWorklistFolder(WorklistSummary worklist);
	}


	/// <summary>
	/// Abstract base class for folder systems that consist of <see cref="WorklistFolder{TItem,TWorklistService}"/>s.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TFolderExtensionPoint"></typeparam>
	/// <typeparam name="TFolderToolExtensionPoint"></typeparam>
	/// <typeparam name="TItemToolExtensionPoint"></typeparam>
	/// <typeparam name="TWorklistService"></typeparam>
	public abstract class WorklistFolderSystem<TItem, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, TWorklistService>
		: WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint>, IWorklistFolderSystem
		where TItem : DataContractBase
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TWorklistService : IWorklistService<TItem>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		protected WorklistFolderSystem(string title)
			: base(title)
		{
		}

		#region IWorklistFolderSystem Members

		public IWorklistFolder AddWorklistFolder(WorklistSummary worklist)
		{
			return AddWorklistFolderHelper(worklist);
		}

		#endregion

		#region Overrides

		///<summary>
		///Initializes or re-initializes the folder system.
		///</summary>
		///<remarks>
		/// This method will be called after <see cref="M:ClearCanvas.Ris.Client.IFolderSystem.SetContext(ClearCanvas.Ris.Client.IFolderSystemContext)" /> has been called. 
		///</remarks>
		public override void Initialize()
		{
			base.Initialize();

			this.Folders.Clear();

			InitializeFolders(new TFolderExtensionPoint());
		}


		/// <summary>
		/// Called to obtain the set of worklists for the current user.  May be overridden, but typically not necessary.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected virtual ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request)
		{
			ListWorklistsForUserResponse response = null;
			Platform.GetService<TWorklistService>(
				delegate(TWorklistService service)
				{
					response = service.ListWorklistsForUser(request);
				});

			return response;
		}

		#endregion

		#region Protected API

		/// <summary>
		/// This method should be overridden by any folder systems that require folders not added through the extension mechanism.
		/// </summary>
		protected virtual void AddDefaultFolders()
		{
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Creates the folder system based on the specified extension point.
		/// </summary>
		/// <param name="folderExtensionPoint"></param>
		private void InitializeFolders(ExtensionPoint<IWorklistFolder> folderExtensionPoint)
		{
			AddDefaultFolders();

			List<string> worklistClassNames = new List<string>();

			// collect worklist class names, and add unfiltered folders if authorized
			foreach (IWorklistFolder folder in folderExtensionPoint.CreateExtensions())
			{
				if (!string.IsNullOrEmpty(folder.WorklistClassName))
					worklistClassNames.Add(folder.WorklistClassName);

				// if unfiltered folders are visible, add the root folder
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
				{
					this.Folders.Add(folder);
				}

			}

			// query the set of worklists to which the current user is subscribed, add a folder for each
			ListWorklistsForUserResponse response = QueryWorklistSet(new ListWorklistsForUserRequest(new List<string>(worklistClassNames)));
			foreach (WorklistSummary summary in response.Worklists)
			{
				IWorklistFolder added = AddWorklistFolderHelper(summary);

				if(added == null)
					Platform.Log(LogLevel.Error, string.Format("Worklist class {0} not added to folder system, most likely because it is not mapped to a folder class.", summary.ClassName));
			}
		}

		/// <summary>
		/// Creates and adds a worklist folder for the specified worklist.
		/// </summary>
		/// <param name="worklist"></param>
		/// <returns></returns>
		private IWorklistFolder AddWorklistFolderHelper(WorklistSummary worklist)
		{
			// create an instance of the folder corresponding to the specified worklist class
			IWorklistFolder folder = (IWorklistFolder)new TFolderExtensionPoint()
				.CreateExtension(
					delegate (ExtensionInfo info)
					{
						return worklist.ClassName == WorklistFolder<TItem, TWorklistService>.GetWorklistClassName(info.ExtensionClass);
					});

			if (folder == null || !(folder is IInitializeWorklistFolder))
				return null;

			IInitializeWorklistFolder initFolder = folder as IInitializeWorklistFolder;

			// augment default base path with worklist name
			Path path = folder.FolderPath;
			if (!string.IsNullOrEmpty(worklist.DisplayName))
			{
				// escape slashes in the display name, otherwise they are interpreted as separators
				string name = worklist.DisplayName.Replace("/", "//");
				path = new Path(string.Concat(path.ToString(), "/", name), folder.ResourceResolver);
			}

			// init folder
			initFolder.Initialize(
                path,
                worklist.WorklistRef,
                worklist.Description,
                GetWorklistOwnership(worklist),
                GetWorklistOwnerName(worklist)
                );

			// add to folders list
			this.Folders.Add(folder);

			return folder;
		}

        private WorklistOwnership GetWorklistOwnership(WorklistSummary worklist)
        {
            return worklist.IsUserWorklist ?
                (worklist.IsStaffOwned ? WorklistOwnership.Staff : WorklistOwnership.Group)
                : WorklistOwnership.Admin;
        }

        private string GetWorklistOwnerName(WorklistSummary worklist)
        {
            if (worklist.IsStaffOwned)
                return Formatting.StaffNameAndRoleFormat.Format(worklist.OwnerStaff);
            if (worklist.IsGroupOwned)
                return worklist.OwnerGroup.Name;
            return null;
        }

		#endregion

	}
}
