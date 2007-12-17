#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems
{
     /// <summary>
     /// Panel to display list of FileSystems for a particular server partition.
     /// </summary>
    public partial class FileSystemsPanel : System.Web.UI.UserControl
    {
        #region Private members
        // the controller used for interaction with the database.
        private FileSystemsConfigurationController _theController;
        // the filesystems whose information will be displayed in this panel
        private IList<Filesystem> _filesystems;
        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets/Gets the filesystems whose information are displayed in this panel.
        /// </summary>
        public IList<Filesystem> FileSystems
        {
            get { return _filesystems; }
            set { _filesystems = value; }
        }

        #endregion

        #region Public Delegates

        /// <summary>
        /// Defines the method which this panel will call to add a new filesystem.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        public delegate void AddFileSystemMethodDelegate(FileSystemsConfigurationController controller);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to add a new filesystem.
        /// </summary>
        public AddFileSystemMethodDelegate AddFileSystemDelegate;

        /// <summary>
        /// Defines the method which this panel will call to edit a new filesystem.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        /// <param name="filesystem">The filesystem displayed in this panel.</param>
        public delegate void EditFileSystemMethodDelegate(FileSystemsConfigurationController controller, Filesystem filesystem);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to edit a new device.
        /// </summary>
        public EditFileSystemMethodDelegate EditFileSystemDelegate;

        /// <summary>
        /// Defines the method which this panel will call to delete a new device.
        /// </summary>
        /// <param name="controller">The controller used by this panel for database interaction.</param>
        /// <param name="filesystem">The filesystem being deleted.</param>
        public delegate void DeleteFileSystemMethodDelegate(FileSystemsConfigurationController controller, Filesystem filesystem);

        /// <summary>
        /// Sets/Retrieves the delegate which this panel will call to delete a new device.
        /// </summary>
        public DeleteFileSystemMethodDelegate DeleteFileSystemDelegate;
        
        #endregion

        #region protected methods
        /// <summary>
        /// Set up event handlers for the child controls.
        /// </summary>
        protected void SetUpEventHandlers()
        {
            FileSystemsToolBar1.OnAddFileSystemButtonClick += delegate
                                                                {
                                                                    // Call the add filesystem delegate 
                                                                    if (AddFileSystemDelegate != null)
                                                                        AddFileSystemDelegate(_theController);
                                                                };


            FileSystemsToolBar1.OnEditFileSystemButtonClick += delegate
                                                                 {
                                                                     // Call the edit filesystem delegate 
                                                                     if (EditFileSystemDelegate != null)
                                                                     {
                                                                         Filesystem fs = FileSystemsGridView1.SelectedFileSystem;
                                                                         if (fs != null)
                                                                         {
                                                                             EditFileSystemDelegate(_theController, fs);
                                                                         }
                                                                     }
                                                                 };

            FileSystemsToolBar1.OnRefreshButtonClick += delegate
                                                              {
                                                                  // Clear all filters and reload the data
                                                                  FileSystemsFilterPanel1.Clear();
                                                                  LoadFileSystems();
                                                              };

            FileSystemsToolBar1.OnDeleteFileSystemButtonClick += delegate
                                                                   {
                                                                       // Call the delete filesystem delegate 
                                                                       if (DeleteFileSystemDelegate != null)
                                                                       {
                                                                           Filesystem fs = FileSystemsGridView1.SelectedFileSystem;
                                                                           if (fs != null)
                                                                           {
                                                                               DeleteFileSystemDelegate(_theController, fs);
                                                                           }
                                                                       }
                                                                   };

            FileSystemsToolBar1.GetSelectedFileSystem= delegate
                                                         {
                                                             return FileSystemsGridView1.SelectedFileSystem;
                                                         };

            GridPager1.GetRecordCountMethod = delegate()
                                                          {
                                                              return FileSystemsGridView1.FileSystems.Count;
                                                          };
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // initialize the controller
            _theController = new FileSystemsConfigurationController();


            // setup child controls
            GridPager1.ItemName = "FileSystem";
            GridPager1.PuralItemName = "FileSystems";
            GridPager1.Grid = FileSystemsGridView1.TheGrid;

            FileSystemsFilterPanel1.Tiers = _theController.GetFileSystemTiers();

            // setup event handler for child controls
            SetUpEventHandlers();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This make sure we have the list to work with. 
            // the list may be out-dated if the add/update event is fired later
            // In those cases, the list must be refreshed again.
            LoadFileSystems();

        }

        

        #endregion Protected methods

        /// <summary>
        /// Load the FileSystems for the partition based on the filters specified in the filter panel.
        /// </summary>
        /// <remarks>
        /// This method only reloads and binds the list bind to the internal grid. <seealso cref="UpdateUI()"/> should be called
        /// to explicit update the list in the grid. 
        /// <para>
        /// This is intentionally so that the list can be reloaded so that it is available to other controls during postback.  In
        /// some cases we may not want to refresh the list if there's no change. Calling <seealso cref="UpdateUI()"/> will
        /// give performance hit as the data will be transfered back to the browser.
        ///  
        /// </para>
        /// </remarks>
        public void LoadFileSystems()
        {
            FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
            
            FileSystemsFilterPanel.FilterSettings filters = FileSystemsFilterPanel1.Filters;

            if (String.IsNullOrEmpty(filters.Description)==false)
            {
                string key = filters.Description.Replace("*", "%") + "%";
                criteria.Description.Like(key);
            }

            if (filters.SelectedTier!=null)
            {
                criteria.Tier.EqualTo(filters.SelectedTier);
            }

            FileSystemsGridView1.FileSystems = _theController.GetFileSystems(criteria);
            FileSystemsGridView1.DataBind();
        }

        /// <summary>
        /// Updates the FileSystem list window in the panel.
        /// </summary>
        /// <remarks>
        /// This method should only be called when necessary as the information in the list window needs to be transmitted back to the client.
        /// If the list is not changed, call <seealso cref="LoadFileSystems()"/> instead.
        /// </remarks>
        public void UpdateUI()
        {
            LoadFileSystems();

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            UpdatePanel.Update();
        }
        
    }
}