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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.SeriesDetails;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.StudyDetails
{
    /// <summary>
    /// Series list panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public partial class SeriesGridView : System.Web.UI.UserControl
    {
        #region Private members
        private IList<Model.Series> _series;
        #endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public IList<Model.Series> Series
        {
            get { return _series; }
            set { _series = value; }
        }


        #endregion Public properties

        #region Protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
            GridView1.DataSource = Series;
            GridView1.DataBind();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Series series = e.Row.DataItem as Series;

                Label performedDateTime = e.Row.FindControl("SeriesPerformedDateTime") as Label;

                if (!String.IsNullOrEmpty(series.PerformedProcedureStepStartDate))
                {
                    string dt;
                    if (DateTimeFormatter.TryFormatDA(series.PerformedProcedureStepStartDate, out dt))
                    {
                        performedDateTime.Text = dt;
                    }
                    else
                    {
                        performedDateTime.Text =
                            String.Format("<i style='color:red'>[Invalid date:{0}]</i>",
                                          series.PerformedProcedureStepStartDate);
                    }
                }

                if (!String.IsNullOrEmpty(series.PerformedProcedureStepStartTime))
                {
                    string dt;
                    if (DateTimeFormatter.TryFormatTM(series.PerformedProcedureStepStartTime, out dt))
                    {
                        performedDateTime.Text += " " + dt;
                    }
                    else
                    {
                        performedDateTime.Text += " " + String.Format("<i style='color:red'>[Invalid time:{0}]</i>", series.PerformedProcedureStepStartTime);
                    }
                }
                


                e.Row.Attributes["seriesuid"] = series.SeriesInstanceUid;


            }
        }

        protected void GridView1_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataBind();
        }

        #endregion Protected methods

    }
}