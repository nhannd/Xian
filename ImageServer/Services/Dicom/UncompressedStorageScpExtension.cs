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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class UncompressedStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _list;
        private readonly string _type = "Uncompressed C-STORE-RQ Image";  
        #endregion

        public override string StorageScpType
        {
            get { return _type; }
        }

        #region IDicomScp Members

        /// <summary>
        /// Returns a list of the services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (_list == null)
            {
                _list = new List<SupportedSop>();

                // Get the SOP Classes
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Set the input parameters for query
                    PartitionSopClassQueryParameters inputParms = new PartitionSopClassQueryParameters();
                    inputParms.ServerPartitionKey = Partition.GetKey();

                    IQueryServerPartitionSopClasses broker = read.GetBroker<IQueryServerPartitionSopClasses>();
                    IList<PartitionSopClass> sopClasses = broker.Find(inputParms);

                    // Now process the SOP Class List
                    foreach (PartitionSopClass partitionSopClass in sopClasses)
                    {
                        if (partitionSopClass.Enabled
                            && !partitionSopClass.NonImage)
                        {
                            SupportedSop sop = new SupportedSop();

                            sop.SopClass = SopClass.GetSopClass(partitionSopClass.SopClassUid);
                            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
                            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);

                            _list.Add(sop);
                        }
                    }
                }
            }
            return _list;
        }


        #endregion
    }
}