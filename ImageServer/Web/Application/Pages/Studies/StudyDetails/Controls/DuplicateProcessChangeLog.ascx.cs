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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DuplicateProcessChangeLog : System.Web.UI.UserControl
    {
        private ProcessDuplicateChangeLog _changeLog;
        public StudyHistory HistoryRecord;

        protected ProcessDuplicateChangeLog ChangeLog
        {
            get
            {
                if (_changeLog==null)
                {
                    _changeLog = XmlUtils.Deserialize<ProcessDuplicateChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

        /// <summary>
        /// </summary>
        protected String ActionDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return "N/A";
                }
                else
                {
                    switch(ChangeLog.Action)
                    {
                        case ProcessDuplicateAction.Delete:
                            return "Delete duplicate SOPs.";
                        case ProcessDuplicateAction.OverwriteAsIs:
                            return "Overwrite existing SOPs and preserve data in duplicates.";
                        case ProcessDuplicateAction.OverwriteUseDuplicates:
                            return "Overwrite existing SOPs and update study with information in duplicates.";

                        case ProcessDuplicateAction.OverwriteUseExisting:
                            return "Overwrite existing SOPs and use the existing study information.";
                    
                        default:
                            return ChangeLog.Action.ToString();

                    }
                }
            }
        }

        protected String ChangeLogShortDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return "N/A";
                }
                else
                {
                    switch (ChangeLog.Action)
                    {
                        case ProcessDuplicateAction.Delete:
                            return "Delete duplicate SOPs.";
                        case ProcessDuplicateAction.OverwriteAsIs:
                            return "Accept As Is.";
                        case ProcessDuplicateAction.OverwriteUseDuplicates:
                            return "Accept + Update Study";

                        case ProcessDuplicateAction.OverwriteUseExisting:
                            return "Accept Modified.";

                        default:
                            return ChangeLog.Action.ToString();

                    }
                }
            }
        }

    }
}