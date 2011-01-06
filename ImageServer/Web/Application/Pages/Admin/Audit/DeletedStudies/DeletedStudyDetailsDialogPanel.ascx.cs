#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class DeletedStudyDetailsDialogPanel : UserControl
    {
        private DeletedStudyDetailsDialogViewModel viewModel;

        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }

        public override void DataBind()
        {
            GeneralInfoPanel.ViewModel = ViewModel;
            ArchiveInfoPanel.ViewModel = ViewModel;

            base.DataBind();
        }
    }
}