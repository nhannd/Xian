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
	public interface IWorklistFolderSystem
	{
		
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

		/// <summary>
		/// This method should be overridden by any folder systems that require folders not added through the extension mechanism.
		/// </summary>
		protected virtual void AddDefaultFolders()
		{
		}

		/// <summary>
		/// Creates the folder system based on the specified extension point.
		/// </summary>
		/// <param name="folderExtensionPoint"></param>
		private void InitializeFolders(ExtensionPoint<IWorklistFolder> folderExtensionPoint)
		{
			AddDefaultFolders();

			Dictionary<string, Type> mapWorklistClassToFolderClass = new Dictionary<string, Type>();

			// collect worklist class names, and add unfiltered folders if authorized
			foreach (IWorklistFolder folder in folderExtensionPoint.CreateExtensions())
			{
				if (!string.IsNullOrEmpty(folder.WorklistClassName))
					mapWorklistClassToFolderClass.Add(folder.WorklistClassName, folder.GetType());

				// if unfiltered folders are visible, add the root folder
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
				{
					this.Folders.Add(folder);
				}

			}

			if (mapWorklistClassToFolderClass.Keys.Count > 0)
			{
				ListWorklistsForUserResponse response = QueryWorklistSet(new ListWorklistsForUserRequest(new List<string>(mapWorklistClassToFolderClass.Keys)));
				foreach (WorklistSummary summary in response.Worklists)
				{
					try
					{
						Type folderClass = mapWorklistClassToFolderClass[summary.ClassName];
						IWorklistFolder folder = (IWorklistFolder)folderExtensionPoint.CreateExtension(new ClassNameExtensionFilter(folderClass.FullName));
						if (folder is IInitializeWorklistFolder)
						{
							IInitializeWorklistFolder initFolder = folder as IInitializeWorklistFolder;

							// augment default base path with worklist name
							Path path = folder.FolderPath;
							if (!string.IsNullOrEmpty(summary.DisplayName))
							{
								// escape slashes in the display name, otherwise they are interpreted as separators
								string name = summary.DisplayName.Replace("/", "//");
								path = new Path(string.Concat(path.ToString(), "/", name), folder.ResourceResolver);
							}

							initFolder.Initialize(path, summary.WorklistRef, summary.Description, false);
						}

						this.Folders.Add(folder);
					}
					catch (KeyNotFoundException e)
					{
						Platform.Log(LogLevel.Error, e, string.Format("Worklist class {0} is not mapped to a folder class.", summary.ClassName));
					}
				}
			}
		}
	}
}
