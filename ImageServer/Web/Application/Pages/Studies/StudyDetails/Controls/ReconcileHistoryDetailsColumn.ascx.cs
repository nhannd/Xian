#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class ReconcileHistoryDetailsColumn : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;
        private StudyReconcileDescriptor _description;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public StudyHistory HistoryRecord
        {
            set { _historyRecord = value; }
        }

        public StudyReconcileDescriptor ReconcileHistory
        {
            get
            {
                if (_description == null && _historyRecord!=null)
                {
                    StudyReconcileDescriptorParser parser = new StudyReconcileDescriptorParser();
                    _description =parser.Parse(_historyRecord.ChangeDescription);
                }
                return _description;
            }
        }

        
        protected string ChangeSummaryText
        {
            get{
                return ActionTranslator.Translate(ReconcileHistory.Action);
            }
        }

        protected string PerformedBy
        {
            get
            {
                return String.Format(SR.StudyDetails_History_Reconcile_PerformedBy, ReconcileHistory.UserName ?? SR.Unknown);
            }
        }
    }


    public static class ActionTranslator
    {
        public static string Translate(StudyReconcileAction action)
        {
            switch (action)
            {
                case StudyReconcileAction.CreateNewStudy:
                    return HtmlUtility.Encode(SR.StudyDetails_Reconcile_NewStudy_Description);
                case StudyReconcileAction.Discard:
                    return HtmlUtility.Encode(SR.StudyDetails_Reconcile_Discard_Description);
                case StudyReconcileAction.Merge:
                    return HtmlUtility.Encode(SR.StudyDetails_Reconcile_Merge_Description);
                case StudyReconcileAction.ProcessAsIs:
                    return HtmlUtility.Encode(SR.StudyDetails_Reconcile_ProcessAsIs_Description);
            }

            return HtmlUtility.Encode(HtmlUtility.GetEnumInfo(action).LongDescription);
        }
    }
}