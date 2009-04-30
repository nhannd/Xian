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