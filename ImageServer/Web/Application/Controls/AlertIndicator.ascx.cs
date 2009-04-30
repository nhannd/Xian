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
using System.Data;
using System.Configuration;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.Admin.Alert.View)]
    public partial class AlertIndicator : System.Web.UI.UserControl
    {
        protected IList<Alert> alerts;
       
        protected void Page_Load(object sender, EventArgs e)
        {           
            AlertController controller = new AlertController();
            AlertSelectCriteria criteria = new AlertSelectCriteria();

            criteria.AlertLevelEnum.EqualTo(AlertLevelEnum.Critical);
            criteria.InsertTime.SortDesc(1);

            AlertsCount.Text = controller.GetAlertsCount(criteria).ToString();

            alerts = controller.GetAlerts(criteria);

            if (alerts.Count > 0) {

                int rows = 0;
                foreach (Alert alert in alerts)
                {
                    TableRow alertRow = new TableRow();

                    alertRow.Attributes.Add("class", "AlertTableCell");

                    TableCell component = new TableCell();
                    TableCell source = new TableCell();
                    TableCell description = new TableCell();

                    description.Wrap = false;

                    component.Text = alert.Component;
                    component.Wrap = false;
                    source.Text = alert.Source;
                    source.Wrap = false;

                    string content = alert.Content.GetElementsByTagName("Message").Item(0).InnerText;
                    description.Text = content.Length < 50 ? content : content.Substring(0, 50);
                    description.Text += " ...";
                    description.Wrap = false;

                    alertRow.Cells.Add(component);
                    alertRow.Cells.Add(source);
                    alertRow.Cells.Add(description);

                    AlertTable.Rows.Add(alertRow);

                    rows++;
                    if (rows == 5) break;
                }
            }
        }
    }
}