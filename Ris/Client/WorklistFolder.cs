#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
    /// Describes worklist ownership options.
    /// </summary>
    public enum WorklistOwnership
    {
        /// <summary>
        /// Worklist is owned by administrators.
        /// </summary>
        Admin,

        /// <summary>
        /// Worklist is owned by a staff person.
        /// </summary>
        Staff,

        /// <summary>
        /// Worklist is owned by a staff group.
        /// </summary>
        Group
    }

	/// <summary>
	/// Internal inteface used to initialize a <see cref="WorkflowFolder"/> once,
	/// without having to define a constructor.
	/// </summary>
	internal interface IInitializeWorklistFolder
	{
		/// <summary>
		/// Initializes this folder with the specified arguments.
		/// </summary>
		void Initialize(Path path, EntityRef worklistRef, string description, WorklistOwnership ownership, string ownerName);
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

        /// <summary>
        /// Gets the ownership of the worklist associated with this folder.
        /// </summary>
        WorklistOwnership Ownership { get; }

        /// <summary>
        /// Gets the name of the worklist owner, or null if not applicable.
        /// </summary>
        string OwnerName { get; }
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
        private WorklistOwnership _ownership;
        private string _ownerName;

        private static readonly IconSet _closedUserWorklistIconSet = new IconSet("UserFolderClosedSmall.png");
        private static readonly IconSet _openUserWorklistIconSet = new IconSet("UserFolderOpenSmall.png");

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

        #region Folder overrides

        protected override IconSet ClosedIconSet
        {
            get { return _ownership == WorklistOwnership.Admin ? base.ClosedIconSet : _closedUserWorklistIconSet; } 
        }

        protected override IconSet OpenIconSet
        {
            get { return _ownership == WorklistOwnership.Admin ? base.OpenIconSet : _openUserWorklistIconSet; }
        }

        #endregion

        #region IInitializeWorklistFolder Members

        void IInitializeWorklistFolder.Initialize(Path path, EntityRef worklistRef, string description, WorklistOwnership ownership, string ownerName)
		{
			_worklistRef = worklistRef;
            _ownership = ownership;
            _ownerName = ownerName;

            this.FolderPath = path;
            this.Tooltip = description;
			this.IsStatic = false;  // folder is not static
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

        /// <summary>
        /// Gets the ownership of the worklist associated with this folder.
        /// </summary>
        public WorklistOwnership Ownership
        {
            get { return _ownership; }
        }

        /// <summary>
        /// Gets the name of the worklist owner, or null if not applicable.
        /// </summary>
        public string OwnerName
        {
            get { return _ownerName; }
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
