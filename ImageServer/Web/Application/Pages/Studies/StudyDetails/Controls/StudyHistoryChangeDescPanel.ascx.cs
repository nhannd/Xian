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
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyHistoryChangeDescPanel : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;

        public StudyHistory HistoryRecord
        {
            get { return _historyRecord; }
            set { _historyRecord = value; }
        }

        public override void DataBind()
        {
            if (_historyRecord != null)
            {
                // Use different control to render the content of the ChangeDescription column.
                IStudyHistoryColumnControlFactory render = GetColumnControlFactory(_historyRecord);
                Control ctrl = render.GetChangeDescColumnControl(this, _historyRecord);
                SummaryPlaceHolder.Controls.Add(ctrl);
            }
            base.DataBind();
        } 

        private static IStudyHistoryColumnControlFactory GetColumnControlFactory(StudyHistory record)
        {
            if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
                return new ReconcileStudyRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited)
                return new StudyEditRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Duplicate)
                return new ProcessDuplicateChangeLogRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.Reprocessed)
                return new StudyReprocessedChangeLogRendererFactory();
            else
                return new DefaultStudyHistoryRendererFactory();
        }

    }
}