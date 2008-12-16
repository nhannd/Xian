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