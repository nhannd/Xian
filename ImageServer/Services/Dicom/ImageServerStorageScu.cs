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

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
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
        private readonly ServerPartition _partition;
        #endregion

        #region Constructors...
        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        public ImageServerStorageScu(ServerPartition partition, Device remoteDevice)
            :base(partition.AeTitle,remoteDevice.AeTitle,remoteDevice.IpAddress,remoteDevice.Port)
        {
            _remoteDevice = remoteDevice;
            _partition = partition;
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
            _partition = partition;
            _remoteDevice = remoteDevice;
        }
        #endregion

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="read">A read context to read from the database.</param>
        public void LoadPreferredSyntaxes(IReadContext read)
        {
            IDevicePreferredTransferSyntaxEntityBroker select =
                read.GetBroker<IDevicePreferredTransferSyntaxEntityBroker>();

            // Setup the select parameters.
            DevicePreferredTransferSyntaxSelectCriteria criteria = new DevicePreferredTransferSyntaxSelectCriteria();
            criteria.DeviceKey.EqualTo(_remoteDevice.GetKey());

            IList<DevicePreferredTransferSyntax> list = select.Find(criteria);

            // Translate the list returned into the database into a list that is supported by the Storage SCU Component
            List<SupportedSop> sopList = new List<SupportedSop>();
            foreach (DevicePreferredTransferSyntax preferred in list)
            {
                SupportedSop sop = new SupportedSop();
                sop.SopClass = SopClass.GetSopClass(preferred.GetServerSopClass().SopClassUid);
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
                string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ".dcm");
                StorageInstance instance = new StorageInstance(instancePath);
                
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
