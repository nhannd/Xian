#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class CompressedStorageScpExtension : StorageScp
    {
        #region Private Members
        private IList<SupportedSop> _sopList;
        private IList<PartitionTransferSyntax> _syntaxList;
        private readonly string _type = "Compressed C-STORE-RQ";

        public override string StorageScpType
        {
            get { return _type; }
        }

        #endregion
        
        #region Private Methods

        #endregion

        #region IDicomScp Members

        /// <summary>
        /// Returns a list of the services supported by this plugin.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            if (_sopList == null)
            {
                _sopList = new List<SupportedSop>();

                // Get the SOP Classes
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Get the transfer syntaxes
                    _syntaxList = LoadTransferSyntaxes(read, Partition.GetKey(),true);

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
                            foreach (PartitionTransferSyntax syntax in _syntaxList)
                            {
                                sop.SyntaxList.Add(TransferSyntax.GetTransferSyntax(syntax.Uid));
                            }
                            _sopList.Add(sop);
                        }
                    }
                }
            }
            return _sopList;
        }

        #endregion
    }
}
