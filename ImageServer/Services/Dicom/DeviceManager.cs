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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    class DeviceManager
    {
    	/// <summary>
    	/// Lookup the device entity in the database corresponding to the remote AE of the association.
    	/// </summary>
    	/// <param name="partition">The partition to look up the devices</param>
    	/// <param name="association">The association</param>
    	/// <param name="isNew">Indicates whether the device returned is created by the call.</param>
    	/// <returns>The device record corresponding to the called AE of the association</returns>
        static public Device LookupDevice(ServerPartition partition, AssociationParameters association, out bool isNew)
        {
            isNew = false;

            Device device = null;

            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IQueryDevice queryDevice = updateContext.GetBroker<IQueryDevice>();

                // Setup the select parameters.
                DeviceQueryParameters queryParameters = new DeviceQueryParameters();
                queryParameters.AeTitle = association.CallingAE;
                queryParameters.ServerPartitionKey = partition.GetKey();

                IList<Device> list = queryDevice.Execute(queryParameters);
                if (list.Count == 0)
                {
                    if (!ImageServerServicesDicomSettings.Default.AcceptAnyDevice)
                    {
                        return null;
                    }

                    if (ImageServerServicesDicomSettings.Default.AutoInsertDevices)
                    {
                        // Auto-insert a new entry in the table.
                        DeviceInsertParameters insertParms = new DeviceInsertParameters();

                        insertParms.AeTitle = association.CallingAE;
                        insertParms.Active = true;
                        insertParms.Description = String.Format("AE: {0}", association.CallingAE);
                        insertParms.Dhcp = false;
                        insertParms.IpAddress = association.RemoteEndPoint.Address.ToString();
                        insertParms.ServerPartitionKey = partition.GetKey();
                        insertParms.Port = ImageServerServicesDicomSettings.Default.DefaultRemotePort;

                        IInsertDevice insert = updateContext.GetBroker<IInsertDevice>();

                        if (false == insert.Execute(insertParms))
                        {
                            Platform.Log(LogLevel.Error,
                                         "Unexpected failure inserting device into the database, rejecting association from {0} to {1}",
                                         association.CallingAE, association.CalledAE);
                            return null;
                        }
                        updateContext.Commit();

                        // now load the device from the database
                        list = queryDevice.Execute(queryParameters);

                        isNew = true;
                    }
                }

                if (list != null && list.Count > 0)
                    device = list[0];

            }

            return device;
        }


    }
}
