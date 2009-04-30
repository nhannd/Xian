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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// The series details view panel within the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public partial class SeriesDetailsView : System.Web.UI.UserControl
    {
        #region Private members
        private Model.Series _series;

        #endregion Private members

        #region Public Properties

        public Model.Series Series
        {
            get { return _series; }
            set { _series = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Series!=null)
            {
                IList<SeriesDetails> seriesDetails = new List<SeriesDetails>();
                seriesDetails.Add(SeriesDetailsAssembler.CreateSeriesDetails(Series));
                DetailsView1.DataSource = seriesDetails;
                DetailsView1.DataBind();
            }
        }

        #endregion Protected Methods

        protected void DetailsView1_DataBound(object sender, EventArgs e)
        {
            SeriesDetails series = DetailsView1.DataItem as SeriesDetails;
        }
    }
}