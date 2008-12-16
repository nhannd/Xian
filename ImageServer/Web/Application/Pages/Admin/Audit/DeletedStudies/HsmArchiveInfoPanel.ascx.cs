using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies
{
    public partial class HsmArchiveInfoPanel : BaseDeletedStudyArchiveUIPanel
    {
        public override void DataBind()
        {
            IList<DeletedStudyArchiveInfo> infoList = new List<DeletedStudyArchiveInfo>();
            if (ArchiveInfo!=null)
                infoList.Add(ArchiveInfo);

            ArchiveInfoView.DataSource = CollectionUtils.Map<DeletedStudyArchiveInfo, HsmArchivePanelInfoDataModel>(
                infoList,
                delegate(DeletedStudyArchiveInfo info)
                    {
                        HsmArchivePanelInfoDataModel dataModel = new HsmArchivePanelInfoDataModel();
                        dataModel.PartitionArchive = info.PartitionArchive;
                        dataModel.ArchiveTime = info.ArchiveTime;
                        dataModel.TransferSyntaxUid = info.TransferSyntaxUid;
                        dataModel.ArchiveXml = XmlUtils.Deserialize<HsmArchiveXml>(info.ArchiveXml);
                        return dataModel;
                    });

            base.DataBind();
        }

    }

    /// <summary>
    /// View Model for the <see cref="HsmArchiveInfoPanel"/>
    /// </summary>
    public class HsmArchivePanelInfoDataModel
    {
        #region Private Fields
        private DateTime _archiveTime;
        private string _transferSyntaxUid;
        private HsmArchiveXml _archiveXml;
        private string _archivePath;
        private PartitionArchive _archive;
        #endregion

        #region Public Properties
        public DateTime ArchiveTime
        {
            get { return _archiveTime; }
            set { _archiveTime = value; }
        }

        public string TransferSyntaxUid
        {
            get { return _transferSyntaxUid; }
            set { _transferSyntaxUid = value; }
        }

        public HsmArchiveXml ArchiveXml
        {
            get { return _archiveXml; }
            set { _archiveXml = value; }
        }


        public string ArchiveFolderPath
        {
            get
            {
                if (String.IsNullOrEmpty(_archivePath))
                {

                    HsmArchiveConfigXml config = XmlUtils.Deserialize<HsmArchiveConfigXml>(_archive.ConfigurationXml);
                    _archivePath = StringUtilities.Combine(new string[]
                                                                       {
                                                                           config.RootDir, ArchiveXml.StudyFolder, ArchiveXml.Uid, ArchiveXml.Filename
                                                                       }, String.Format("{0}", Path.DirectorySeparatorChar));

                }
                return _archivePath;
            }

        }

        public PartitionArchive PartitionArchive
        {
            get { return _archive; }
            set { _archive = value; }
        }
        #endregion
    }


    /// <summary>
    /// Represents the data of an Hsm Archive entry.
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveXml
    {
        #region Private Fields
        private string _studyFolder;
        private string _filename;
        private string _uid;
        #endregion

        #region Public Properties
        public string StudyFolder
        {
            get { return _studyFolder; }
            set { _studyFolder = value; }
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }
        #endregion

    }

    /// <summary>
    /// Represents the Hsm Archive configuration
    /// </summary>
    [Serializable]
    [XmlRoot("HsmArchive")]
    public class HsmArchiveConfigXml
    {
        #region Private Fields
        private string _rootDir;
        #endregion

        #region Public Properties
        public string RootDir
        {
            get { return _rootDir; }
            set { _rootDir = value; }
        }
        #endregion
    }

   
}