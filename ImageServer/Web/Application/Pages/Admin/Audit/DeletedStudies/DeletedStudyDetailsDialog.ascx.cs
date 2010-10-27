#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    /// <summary>
    /// View model for <see cref="DeletedStudyDetailsDialog"/>
    /// </summary>
    internal class DeletedStudyDetailsDialogViewModel
    {
        public DeletedStudyInfo DeletedStudyRecord { get; set; }
    }

    public partial class DeletedStudyDetailsDialog : UserControl
    {
        #region Private Fields

        #endregion

        #region Internal Properties

        internal DeletedStudyDetailsDialogViewModel ViewModel { get; set; }

        #endregion

        #region Public Methods

        public void Show()
        {
            DialogContent.ViewModel = ViewModel;
            DataBind();
            ModalDialog.Show();
        }

        #endregion

        #region Protected Methods

        protected void CloseClicked(object sender, ImageClickEventArgs e)
        {
            ModalDialog.Hide();
        }

        #endregion
    }
}