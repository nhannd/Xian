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
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    /// <summary>
    /// Plugin for handling DICOM Verification (C-ECHO) requests.
    /// </summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class VerificationScpExtension : BaseScp, IDicomScp<DicomScpContext>
    {
        #region Private members
        private List<SupportedSop> _list = new List<SupportedSop>();
        #endregion

        #region Contructors
        /// <summary>
        /// Public default constructor.  Implements the Verification SOP Class.
        /// </summary>
        public VerificationScpExtension()
        {
            SupportedSop sop = new SupportedSop();
            sop.SopClass = SopClass.VerificationSopClass;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);
        }
        #endregion

        #region IDicomScp Members

        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            server.SendCEchoResponse(presentationID,message.MessageId,DicomStatuses.Success);
            return true;
        }

        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            return _list;
        }

        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {
            return DicomPresContextResult.Accept;
        }

        #endregion Overridden BaseSCP methods
    }
}