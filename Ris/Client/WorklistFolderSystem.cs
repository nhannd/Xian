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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public interface IWorklistFolderSystem : IFolderSystem
    {
        /// <summary>
        /// Gets a list of class names of worklist classes supported by this folder system.
        /// </summary>
        IList<string> SupportedWorklistClasses { get; }


        /// <summary>
        /// Attempts to add the specified worklist to this folder system.
        /// </summary>
        /// <remarks>
        /// Returns the new folder if successful, or null if this folder system does not support the
        /// specified worklist class.
        /// </remarks>
        IWorklistFolder AddWorklistFolder(WorklistSummary worklist);

		/// <summary>
		/// Updates the specified worklist folder to reflect the specified worklist.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="worklist"></param>
    	void UpdateWorklistFolder(IWorklistFolder folder, WorklistSummary worklist);
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
        : WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint, WorklistSearchParams>, IWorklistFolderSystem
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

        /// <summary>
        /// Gets a list of class names of worklist classes supported by this folder system.
        /// </summary>
        public IList<string> SupportedWorklistClasses
        {
            get
            {
                return CollectionUtils.Map<ExtensionInfo, string>(
                    new TFolderExtensionPoint().ListExtensions(),
                    delegate(ExtensionInfo info) { return GetWorklistClassForFolderClass(info.ExtensionClass); });
            }
        }

        /// <summary>
        /// Attempts to add the specified worklist to this folder system.
        /// </summary>
        /// <remarks>
        /// Returns the new folder if successful, or null if this folder system does not support the
        /// specified worklist class.
        /// </remarks>
        public IWorklistFolder AddWorklistFolder(WorklistSummary worklist)
        {
            return AddWorklistFolderHelper(worklist);
        }

    	/// <summary>
    	/// Updates the specified worklist folder to reflect the specified worklist.
    	/// </summary>
    	/// <param name="folder"></param>
    	/// <param name="worklist"></param>
    	public void UpdateWorklistFolder(IWorklistFolder folder, WorklistSummary worklist)
    	{
			if(!this.Folders.Contains(folder))
				return;

    		InitializeFolderProperties(folder, worklist);

			// notify that the folder properties have changed
			NotifyFolderPropertiesChanged(folder);
    	}

    	public override SearchParams CreateSearchParams(string searchText)
        {
            return new WorklistSearchParams(searchText);
        }

        public override void LaunchSearchComponent()
        {
            SearchComponent.Launch(this.DesktopWindow);
        }

        public override Type SearchComponentType
        {
            get { return typeof(SearchComponent); }
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

                if (added == null)
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
                    delegate(ExtensionInfo info)
                    {
                        return worklist.ClassName == GetWorklistClassForFolderClass(info.ExtensionClass);
                    });

            if (folder == null || !(folder is IInitializeWorklistFolder))
                return null;

            InitializeFolderProperties(folder, worklist);

        	// add to folders list
            this.Folders.Add(folder);

            return folder;
        }

		/// <summary>
		/// Initializes the specified folder's properties from the specified worklist.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="folder"></param>
    	private void InitializeFolderProperties(IWorklistFolder folder, WorklistSummary worklist)
    	{
			IInitializeWorklistFolder initFolder = (IInitializeWorklistFolder)folder;

    		// augment default base path with worklist name
    		Path path = folder.FolderPath;
    		if (!string.IsNullOrEmpty(worklist.DisplayName))
    		{
    			path = path.Append(new PathSegment(worklist.DisplayName, folder.ResourceResolver));
    		}

    		// init folder
    		initFolder.Initialize(
    			path,
    			worklist.WorklistRef,
    			worklist.Description,
    			GetWorklistOwnership(worklist),
    			GetWorklistOwnerName(worklist)
    			);
    	}

    	private string GetWorklistClassForFolderClass(Type folderClass)
        {
            return WorklistFolder<TItem, TWorklistService>.GetWorklistClassName(folderClass);
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
