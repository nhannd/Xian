#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// An ImageServer specific customization of the <see cref="StorageScu"/> component.
    /// </summary>
    public class ImageServerStorageScu : StorageScu
    {
        #region Private Members
        private readonly Device _remoteDevice;

    	#endregion

        #region Constructors...
        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        public ImageServerStorageScu(ServerPartition partition, Device remoteDevice)
            :base(partition.AeTitle,remoteDevice.AeTitle,remoteDevice.IpAddress,remoteDevice.Port)
        {
            _remoteDevice = remoteDevice;
        }

        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        /// <param name="partition">The <see cref="ServerPartition"/> instagating the association.</param>
        /// <param name="remoteDevice">The <see cref="Device"/> being connected to.</param>
        /// <param name="moveOriginatorAe">The Application Entity Title of the application that orginated this C-STORE association.</param>
        /// <param name="moveOrginatorMessageId">The Message ID of the C-MOVE-RQ message that orginated this C-STORE association.</param>
        public ImageServerStorageScu(ServerPartition partition, Device remoteDevice, string moveOriginatorAe, ushort moveOrginatorMessageId)
            : base(partition.AeTitle, remoteDevice.AeTitle, remoteDevice.IpAddress, remoteDevice.Port, moveOriginatorAe, moveOrginatorMessageId)
        {
        	_remoteDevice = remoteDevice;
        }
        #endregion

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="read">A read context to read from the database.</param>
        public void LoadPreferredSyntaxes(IPersistenceContext read)
        {
            var select = read.GetBroker<IDevicePreferredTransferSyntaxEntityBroker>();

            // Setup the select parameters.
            var criteria = new DevicePreferredTransferSyntaxSelectCriteria();
            criteria.DeviceKey.EqualTo(_remoteDevice.GetKey());

            IList<DevicePreferredTransferSyntax> list = select.Find(criteria);

            // Translate the list returned into the database into a list that is supported by the Storage SCU Component
            var sopList = new List<SupportedSop>();
            foreach (DevicePreferredTransferSyntax preferred in list)
            {
                var sop = new SupportedSop
                              {
                                  SopClass = SopClass.GetSopClass(preferred.GetServerSopClass().SopClassUid)
                              };
            	sop.AddSyntax(TransferSyntax.GetTransferSyntax(preferred.GetServerTransferSyntax().Uid));

                sopList.Add(sop);
            }

            SetPreferredSyntaxList(sopList);
        }

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        public void LoadPreferredSyntaxes()
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                LoadPreferredSyntaxes(read);
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="SeriesXml"/> file into the component for sending.
        /// </summary>
        /// <param name="seriesXml"></param>
        /// <param name="seriesPath"></param>
        /// <param name="patientsName"></param>
        /// <param name="patientId"></param>
        /// <param name="studyXml"></param>
        public void LoadSeriesFromSeriesXml(StudyXml studyXml, string seriesPath, SeriesXml seriesXml, string patientsName, string patientId)
        {
            foreach (InstanceXml instanceXml in seriesXml)
            {
                string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ServerPlatform.DicomFileExtension);
                var instance = new StorageInstance(instancePath);
                
                AddStorageInstance(instance);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
            	instance.PatientId = patientId;
            	instance.PatientsName = patientsName;
            	instance.StudyInstanceUid = studyXml.StudyInstanceUid;
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="studyPath"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadStudyFromStudyXml(string studyPath, StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                string seriesPath = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);

				LoadSeriesFromSeriesXml(studyXml, seriesPath, seriesXml, studyXml.PatientsName, studyXml.PatientId);
            }
        }

    }
}
