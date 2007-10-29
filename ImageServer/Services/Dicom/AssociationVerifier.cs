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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Static class used to verify if an association should be accepted.
    /// </summary>
    public static class AssociationVerifier
    {
        /// <summary>
        /// Do the actual verification if an association is acceptable.
        /// </summary>
        /// <remarks>
        /// This method primarily checks the remote AE title to see if it is a valid device that can 
        /// connect to the partition.
        /// </remarks>
        /// <param name="parms">Generic parameter passed in, is a DicomScpParameters instance.</param>
        /// <param name="assocParms">The association parameters.</param>
        /// <param name="result">Output parameter with the DicomRejectResult for rejecting the association.</param>
        /// <param name="reason">Output parameter with the DicomRejectReason for rejecting the association.</param>
        /// <returns>true if the association should be accepted, false if it should be rejected.</returns>
        public static bool Verify(DicomScpContext parms, ServerAssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason)
        {
            try
            {

                IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

                IQueryDevice select = read.GetBroker<IQueryDevice>();

                // Setup the select parameters.
                DeviceQueryParameters selectParms = new DeviceQueryParameters();
                selectParms.AeTitle = assocParms.CallingAE;
                selectParms.ServerPartitionKey = parms.Partition.GetKey();

                IList<Device> list = select.Execute(selectParms);
                if (list.Count == 0)
                {
                    if (!ImageServerServicesDicomSettings.Default.AcceptAnyDevice)
                    {
                        reason = DicomRejectReason.CallingAENotRecognized;
                        result = DicomRejectResult.Permanent;
                        return false;
                    }

                    if (ImageServerServicesDicomSettings.Default.AutoInsertDevices)
                    {
                        // Auto-insert a new entry in the table.
                        DeviceInsertParameters insertParms = new DeviceInsertParameters();

                        insertParms.AeTitle = assocParms.CallingAE;
                        insertParms.Active = true;
                        insertParms.Description = "";
                        insertParms.Dhcp = false;
                        insertParms.IpAddress = assocParms.RemoteEndPoint.Address.ToString();
                        insertParms.ServerPartitionKey = parms.Partition.GetKey();
                        insertParms.Port = ImageServerServicesDicomSettings.Default.DefaultRemotePort;

                        IInsertDevice insert = read.GetBroker<IInsertDevice>();

                        if (false == insert.Execute(insertParms))
                        {
                            Platform.Log(LogLevel.Error,
                                         "Unexpected failure inserting device into the database, rejecting association from {0} to {1}",
                                         assocParms.CallingAE, assocParms.CalledAE);
                            reason = DicomRejectReason.NoReasonGiven;
                            result = DicomRejectResult.Permanent;
                            read.Dispose();
                            return false;
                        }
                    }
                }
                else
                {
                    if (list[0].Active == false)
                    {
                        Platform.Log(LogLevel.Error,
                                     "Rejecting association from {0} to {1}.  Device is disabled.",
                                     assocParms.CallingAE, assocParms.CalledAE);
                        reason = DicomRejectReason.CallingAENotRecognized;
                        result = DicomRejectResult.Permanent;
                        read.Dispose();
                        return false;
                    }
                }

                read.Dispose();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected error when verifying incoming association from {0} to {1}",
                             assocParms.CallingAE, assocParms.CalledAE);
                reason = DicomRejectReason.NoReasonGiven;
                result = DicomRejectResult.Permanent;
                return false;
            }

            reason = DicomRejectReason.NoReasonGiven;
            result = DicomRejectResult.Permanent;

            return true;
        }

    }
}