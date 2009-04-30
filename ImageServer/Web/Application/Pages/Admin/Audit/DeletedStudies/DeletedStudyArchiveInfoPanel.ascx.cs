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
using System.Web.UI;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public interface IDeletedStudyArchiveUIPanel
    {
        DeletedStudyArchiveInfo ArchiveInfo { get; set; }
    }

    abstract public class BaseDeletedStudyArchiveUIPanel : UserControl, IDeletedStudyArchiveUIPanel
    {
        private DeletedStudyArchiveInfo _archiveInfo;
        #region IDeletedStudyArchiveUIPanel Members

        public DeletedStudyArchiveInfo ArchiveInfo
        {
            get
            {
                return _archiveInfo;
            }
            set
            {
                _archiveInfo = value;
            }
        }

        #endregion
    }

    public partial class DeletedStudyArchiveInfoPanel : UserControl
    {
        #region Private Fields
        private DeletedStudyDetailsDialogViewModel viewModel;
        #endregion

        internal DeletedStudyDetailsDialogViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }


        private void CreateArchivePanel()
        {
            ArchiveTypeEnum archivetype = GetArchiveType(viewModel.DeletedStudyRecord.Archives[0]);
            Control detailsPanel = LoadArchiveInformationPanel(archivetype, viewModel.DeletedStudyRecord.Archives[0]);
            detailsPanel.DataBind();
            ArchiveViewPlaceHolder.Controls.Add(detailsPanel); 
            
        }

        private Control LoadArchiveInformationPanel(ArchiveTypeEnum type, DeletedStudyArchiveInfo info)
        {
            BaseDeletedStudyArchiveUIPanel panel = null;
            if (type == null)
            {
                panel = LoadControl("GeneralArchiveInfoPanel.ascx") as BaseDeletedStudyArchiveUIPanel;
                panel.ArchiveInfo = info;

            }
            else if (type == ArchiveTypeEnum.HsmArchive)
            {
                panel = LoadControl("HsmArchiveInfoPanel.ascx") as BaseDeletedStudyArchiveUIPanel;
                panel.ArchiveInfo = info;
            }
            return panel;
        }

        private static ArchiveTypeEnum GetArchiveType(DeletedStudyArchiveInfo archiveInfo)
        {
            Platform.CheckForNullReference(archiveInfo, "archiveInfo");
            Platform.CheckForNullReference(archiveInfo.ArchiveXml, "archiveInfo.ArchiveXml");

            XmlNode node = archiveInfo.ArchiveXml.DocumentElement;
            if (node.Name.Equals("HsmArchive", StringComparison.InvariantCultureIgnoreCase))
            {
                return ArchiveTypeEnum.HsmArchive;
            }
            else
            {
                return null;//unknown
            }
        }


        public override void DataBind()
        {

            if (viewModel != null && viewModel.DeletedStudyRecord.Archives != null && viewModel.DeletedStudyRecord.Archives.Count>0)
            {
                NoArchiveMessagePanel.Visible = false;

                CreateArchivePanel();
            }
            else
            {
                NoArchiveMessagePanel.Visible = true;
            }

            base.DataBind();
        }

    }
}