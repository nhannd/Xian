#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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