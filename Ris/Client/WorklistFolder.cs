using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Internal inteface used to initialize a <see cref="WorkflowFolder"/> once,
	/// without having to define a constructor.
	/// </summary>
	internal interface IInitializeWorklistFolder
	{
		/// <summary>
		/// Initializes this folder with the specified arguments.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="worklistRef"></param>
		/// <param name="description"></param>
		/// <param name="isStatic"></param>
		void Initialize(Path path, EntityRef worklistRef, string description, bool isStatic);
	}

	public interface IWorklistFolder : IFolder
	{
		/// <summary>
		/// Gets the reference to the worklist that populates this folder, or null if the folder is not associated with a worklist instance.
		/// </summary>
		EntityRef WorklistRef { get;}

		/// <summary>
		/// Gets the name of the worklist class that this folder is associated with.
		/// </summary>
		string WorklistClassName { get;}
	}


	/// <summary>
	/// Abstract base class for folders that display the contents of worklists.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TWorklistService"></typeparam>
	public abstract class WorklistFolder<TItem, TWorklistService> : WorkflowFolder<TItem>, IInitializeWorklistFolder, IWorklistFolder
		where TItem : DataContractBase
		where TWorklistService : IWorklistService<TItem>
	{
		private EntityRef _worklistRef;

		/// <summary>
		/// Obtains the name of the worklist class associated with the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <returns></returns>
		public static string GetWorklistClassName(Type folderClass)
		{
			FolderForWorklistClassAttribute a = AttributeUtils.GetAttribute<FolderForWorklistClassAttribute>(folderClass);
			return a == null ? null : a.WorklistClassName;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="itemsTable"></param>
		protected WorklistFolder(Table<TItem> itemsTable)
			: base(itemsTable)
		{
		}

		#region IInitializeWorklistFolder Members

		void IInitializeWorklistFolder.Initialize(Path path, EntityRef worklistRef, string description, bool isStatic)
		{
			this.FolderPath = path;
			_worklistRef = worklistRef;
			this.Tooltip = description;
			this.IsStatic = isStatic;
		}

		#endregion

		public override string Id
		{
			get
			{
				return this.IsStatic ? base.Id : _worklistRef.ToString(false);
			}
		}

		#region IWorklistFolder Members

		/// <summary>
		/// Gets the reference to the worklist that populates this folder, or null if the folder is not associated with a worklist instance.
		/// </summary>
		public EntityRef WorklistRef
		{
			get { return _worklistRef; }
		}

		/// <summary>
		/// Gets the name of the worklist class that this folder is associated with,
		/// typically defined by the <see cref="FolderForWorklistClassAttribute"/>.
		/// </summary>
		public string WorklistClassName
		{
			get
			{
				return GetWorklistClassName(this.GetType());
			}
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the set of items in the folder.
		/// </summary>
		/// <returns></returns>
		protected override QueryItemsResult QueryItems()
		{
			QueryItemsResult result = null;

			Platform.GetService<TWorklistService>(
				delegate(TWorklistService service)
				{
					QueryWorklistRequest request = this.WorklistRef == null
						? new QueryWorklistRequest(this.WorklistClassName, true, true, DowntimeRecovery.InDowntimeRecoveryMode)
						: new QueryWorklistRequest(this.WorklistRef, true, true, DowntimeRecovery.InDowntimeRecoveryMode);

					QueryWorklistResponse<TItem> response = service.QueryWorklist(request);
					result = new QueryItemsResult(response.WorklistItems, response.ItemCount);
				});

			return result;
		}


		/// <summary>
		/// Called to obtain a count of the logical total number of items in the folder (which may be more than the number in memory).
		/// </summary>
		/// <returns></returns>
		protected override int QueryCount()
		{
			int count = -1;

			Platform.GetService<TWorklistService>(
				delegate(TWorklistService service)
				{
					QueryWorklistRequest request = this.WorklistRef == null
						? new QueryWorklistRequest(this.WorklistClassName, false, true, DowntimeRecovery.InDowntimeRecoveryMode)
						: new QueryWorklistRequest(this.WorklistRef, false, true, DowntimeRecovery.InDowntimeRecoveryMode);

					QueryWorklistResponse<TItem> response = service.QueryWorklist(request);
					count = response.ItemCount;
				});

			return count;
		}

		#endregion
	}
}
