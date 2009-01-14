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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    public partial class AlertsPanel : UserControl
    {
        #region Private Members

        // used for database interaction
        private AlertController _theController;

        #endregion Private Members

        #region Public Properties

        // Sets/Gets the controller used to retrieve load partitions.
        public AlertController Controller
        {
            get { return _theController; }
            set { _theController = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Determines if filters are being specified.
        /// </summary>
        /// <returns></returns>
        protected bool HasFilters()
        {
            if (ComponentFilter.Text.Length > 0 || InsertDateFilter.Text.Length > 0 || LevelFilter.SelectedIndex > 0 || CategoryFilter.SelectedIndex > 0)
                return true;
            else
                return false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateUI();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            GridPagerTop.Target = AlertsGridPanel.AlertGrid;
            GridPagerTop.ItemName = App_GlobalResources.SR.GridPagerAlertSingleItemFound;
            GridPagerTop.PuralItemName = App_GlobalResources.SR.GridPagerAlertMultipleItemsFound;
            GridPagerBottom.ItemName = App_GlobalResources.SR.GridPagerAlertSingleItemFound;
            GridPagerBottom.PuralItemName = App_GlobalResources.SR.GridPagerAlertMultipleItemsFound;
            GridPagerBottom.Target = AlertsGridPanel.AlertGrid;

            ClearInsertDateButton.OnClientClick = ScriptHelper.ClearDate(InsertDateFilter.ClientID, InsertDateCalendarExtender.ClientID);
            
            IList<AlertLevelEnum> levelEnums = AlertLevelEnum.GetAll();
            IList<AlertCategoryEnum> categoryEnums = AlertCategoryEnum.GetAll();

            int prevSelectedIndex = LevelFilter.SelectedIndex;
            LevelFilter.Items.Clear();
            LevelFilter.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (AlertLevelEnum ale in levelEnums)
                LevelFilter.Items.Add(new ListItem(ale.Description, ale.Lookup));
            LevelFilter.SelectedIndex = prevSelectedIndex;

            prevSelectedIndex = CategoryFilter.SelectedIndex;
            CategoryFilter.Items.Clear();
            CategoryFilter.Items.Add(new ListItem(App_GlobalResources.SR.Any, string.Empty));
            foreach (AlertCategoryEnum ace in categoryEnums)
                CategoryFilter.Items.Add(new ListItem(ace.Description, ace.Lookup));
            CategoryFilter.SelectedIndex = prevSelectedIndex;

            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            GridPagerTop.GetRecordCountMethod = delegate { return AlertsGridPanel.ResultCount; };
            GridPagerBottom.GetRecordCountMethod = delegate { return AlertsGridPanel.ResultCount; };

            AlertsGridPanel.DataSourceCreated += delegate(AlertDataSource source)
                            {
                                if (!String.IsNullOrEmpty(ComponentFilter.Text))
                                    source.Component = "%" + ComponentFilter.Text + "%";
                                if (LevelFilter.SelectedIndex > 0)
                                    source.Level = AlertLevelEnum.GetEnum(LevelFilter.SelectedValue);
                                if (CategoryFilter.SelectedIndex > 0)
                                    source.Category = AlertCategoryEnum.GetEnum(CategoryFilter.SelectedValue);
                                source.DateFormats = InsertDateCalendarExtender.Format;
                                if (!String.IsNullOrEmpty(InsertDateFilter.Text))
                                    source.InsertTime = InsertDateFilter.Text;
                            };
        }

        protected void Clear()
        {
            ComponentFilter.Text = string.Empty;
            InsertDateFilter.Text = string.Empty;
            LevelFilter.SelectedIndex = 0;
            LevelFilter.Text = string.Empty;
        }

        protected void DeleteAlertButton_Click(object sender, ImageClickEventArgs e)
        {
            Alert alert = AlertsGridPanel.SelectedAlert.TheAlertItem;
            if (alert != null)
            {
                ((Default)Page).DeleteAlert(alert.Key);
            }
        }
        
        protected void DeleteAllAlertsButton_Click(object sender, ImageClickEventArgs e)
        {
            ((Default)Page).DeleteAllAlerts();
        }

        protected void SearchButton_Click(object sender, ImageClickEventArgs e)
        {
            AlertsGridPanel.DataBind();   
        }

        protected void RefreshButton_Click(object sender, ImageClickEventArgs e)
        {
            // refresh the list
            Clear();
            UpdateUI();
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateUI()
        {
            AlertsGridPanel.DataBind();   
            UpdatePanel1.Update();

        }

        #endregion Public methods
       
    }
}