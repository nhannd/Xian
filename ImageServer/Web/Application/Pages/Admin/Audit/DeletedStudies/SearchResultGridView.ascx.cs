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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Data.Model;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class SearchResultGridView : GridViewPanel
    {
        #region Private Fields

        private DeletedStudyDataSource _dataSource;

        #endregion

        #region Public Properties

        public GridView GridViewControl
        {
            get { return ListControl; }
        }

        public int ResultCount
        {
            get { return _dataSource.SelectCount(); }
        }

        public DeletedStudyInfo SelectedItem
        {
            get { return _dataSource.Find(ListControl.SelectedValue); }
        }

        public ObjectDataSource DataSourceContainer
        {
            get { return DataSource; }
        }

        #endregion

        #region Overriden Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DataSourceContainer.ObjectCreated += DataSourceContainer_ObjectCreated;
            ListControl.DataSource = DataSource;
            TheGrid = ListControl;
        }

        #endregion

        #region Private Methods

        private void DataSourceContainer_ObjectCreated(object sender, ObjectDataSourceEventArgs e)
        {
            // keep a reference to the data source created, used for other purposes
            _dataSource = e.ObjectInstance as DeletedStudyDataSource;
        }

        #endregion
    }
}