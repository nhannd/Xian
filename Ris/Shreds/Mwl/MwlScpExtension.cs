#region License

//MWL Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;

namespace ClearCanvas.Ris.Shreds.Mwl
{
    [ExtensionOf(typeof (DicomScpExtensionPoint<MwlShredContext>))]
    internal class MwlScpExtension : IDicomScp<MwlShredContext>
    {
        private readonly List<SupportedSop> _supportedSops = new List<SupportedSop>();

        public MwlScpExtension()
        {

            var sop = new SupportedSop {SopClass = SopClass.ModalityWorklistInformationModelFind};
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            _supportedSops.Add(sop);

        }

        public IList<SupportedSop> GetSupportedSopClasses()
        {
            return _supportedSops;
        }

        public bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID,
                                     DicomMessage message)
        {

            Platform.Log(LogLevel.Debug,
                         String.Format("Modality Worklist Query From {0} at ip {1}:{2}", association.CallingAE,
                                       association.RemoteEndPoint.Address,
                                       association.RemoteEndPoint.Port));
            IList<DicomMessage> results;
            try
            {
                results = new RisQuerier().Query(message, association.CallingAE);
            }
            catch (NotSupportedException)
            {
                server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
                                         DicomStatuses.QueryRetrieveOutOfResources);
                return true;
            }
            foreach (var res in results)
            {
                server.SendCFindResponse(presentationID, message.MessageId, res, DicomStatuses.Pending);
            }
            server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(), DicomStatuses.Success);
            return true;
        }

        public void SetContext(MwlShredContext context)
        {
        }

        public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
        {
            
            var verifier = new AssociationVerifierExtensionPoint().CreateExtension() as IAssociationVerifier;
            if (verifier != null)
            {
                if (verifier.Verify(association.CalledAE))
                    return DicomPresContextResult.Accept;
                return DicomPresContextResult.RejectUser;
            }
            return DicomPresContextResult.Accept; 
        }

        public void Cleanup()
        {
        }

        public void OnDimseTimeout(DicomServer server, ServerAssociationParameters association)
        {
        }

        public void OnReceiveAbort(DicomServer server, ServerAssociationParameters association, DicomAbortSource source,
                                   DicomAbortReason reason)
        {
        }

        public void OnNetworkError(DicomServer server, ServerAssociationParameters association, Exception e)
        {
        }

        public void OnReceiveCancel(DicomServer server, ServerAssociationParameters association, string studyInstanceUid)
        {
        }

        public void OnNetworkClosed(DicomServer server, ServerAssociationParameters association, string studyInstanceUid)
        {
        }
    }
}