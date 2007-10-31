#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    public class ImageServerStorageScu : StorageScu
    {
        #region Constructors...
        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        /// <param name="localAe">The local AE title.</param>
        /// <param name="remoteAe">The remote AE title being connected to.</param>
        /// <param name="remoteHost">The hostname or IP address of the remote AE.</param>
        /// <param name="remotePort">The listen port of the remote AE.</param>
        public ImageServerStorageScu(string localAe, string remoteAe, string remoteHost, int remotePort)
            :base(localAe,remoteAe,remoteHost,remotePort)
        {
        }

        /// <summary>
        /// Constructor for Storage SCU Component.
        /// </summary>
        /// <param name="localAe">The local AE title.</param>
        /// <param name="remoteAe">The remote AE title being connected to.</param>
        /// <param name="remoteHost">The hostname or IP address of the remote AE.</param>
        /// <param name="remotePort">The listen port of the remote AE.</param>
        /// <param name="moveOriginatorAe">The Application Entity Title of the application that orginated this C-STORE association.</param>
        /// <param name="moveOrginatorMessageId">The Message ID of the C-MOVE-RQ message that orginated this C-STORE association.</param>
        public ImageServerStorageScu(string localAe, string remoteAe, string remoteHost, int remotePort, string moveOriginatorAe, ushort moveOrginatorMessageId)
            : base(localAe, remoteAe, remoteHost, remotePort, moveOriginatorAe, moveOrginatorMessageId)
        {
        }
        #endregion

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="device"></param>
        public void LoadPreferredSyntaxes(IReadContext read, Device device)
        {
            IQueryDevicePreferredTransferSyntax select = read.GetBroker<IQueryDevicePreferredTransferSyntax>();

            // Setup the select parameters.
            DevicePreferredTransferSyntaxQueryParameters selectParms = new DevicePreferredTransferSyntaxQueryParameters();
            selectParms.DeviceKey = device.GetKey();

            IList<DevicePreferredTransferSyntax> list = select.Execute(selectParms);

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
        /// <param name="device"></param>
        public void LoadPreferredSyntaxes(Device device)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                LoadPreferredSyntaxes(read, device);
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="SeriesXml"/> file into the component for sending.
        /// </summary>
        /// <param name="seriesXml"></param>
        /// <param name="seriesPath"></param>
        public void LoadSeriesFromSeriesXml(string seriesPath, SeriesXml seriesXml)
        {
            foreach (InstanceXml instanceXml in seriesXml)
            {
                string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ".dcm");
                StorageInstance instance = new StorageInstance(instancePath);
                
                AddStorageInstance(instance);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="studyXml"></param>
        /// <param name="studyPath"></param>
        public void LoadStudyFromStudyXml(string studyPath, StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                string seriesPath = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);

                LoadSeriesFromSeriesXml(seriesPath, seriesXml);
            }
        }

        /// <summary>
        /// Loads a specific SOP Instance from a given <see cref="StudyXml"/> file for sending.
        /// </summary>
        /// <param name="studyPath">The filesystem path of the study containing the SOP Instance to send.</param>
        /// <param name="seriesInstanceUid">The Series Instance UID of the SOP Instance to send.</param>
        /// <param name="sopInstanceUid">The SOP Instance UID to send.</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load the instance information from.</param>
        public void LoadInstanceFromStudyXml(string studyPath, string seriesInstanceUid, string sopInstanceUid, StudyXml studyXml)
        {
            SeriesXml seriesXml = studyXml[seriesInstanceUid];
            InstanceXml instanceXml = seriesXml[sopInstanceUid];
            string seriesPath = Path.Combine(studyPath, seriesInstanceUid);
            string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ".dcm");
            StorageInstance instance = new StorageInstance(instancePath);

            AddStorageInstance(instance);

            instance.SopClass = instanceXml.SopClass;
            instance.TransferSyntax = instanceXml.TransferSyntax;
            instance.SopInstanceUid = instanceXml.SopInstanceUid;
        }
    }
}
