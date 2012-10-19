#region License

//MPPS Support for Clear Canvas RIS
//Copyright (C)  2012 Aaron Boxer

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

namespace ClearCanvas.Ris.Shreds.MPPS
{
    [ExtensionOf(typeof (DicomScpExtensionPoint<MPPSShredContext>))]
    internal class ScpExtension : IDicomScp<MPPSShredContext>
    {
        private readonly List<SupportedSop> _supportedSops = new List<SupportedSop>();

        public ScpExtension()
        {
            var sop = new SupportedSop {SopClass = SopClass.VerificationSopClass};
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrBigEndian);


            sop = new SupportedSop {SopClass = SopClass.ModalityPerformedProcedureStepSopClass};
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrBigEndian);

            sop = new SupportedSop {SopClass = SopClass.ModalityPerformedProcedureStepNotificationSopClass};
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrBigEndian);

            sop = new SupportedSop {SopClass = SopClass.ModalityPerformedProcedureStepRetrieveSopClass};
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrBigEndian);
        }

        #region IDicomScp<MPPSShredContext> Members

        public IList<SupportedSop> GetSupportedSopClasses()
        {
            return _supportedSops;
        }

        public bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID,
                                     DicomMessage message)
        {
            if (message.CommandField == DicomCommandField.CEchoRequest)
            {
                server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
                return true;
            }

            if (message.CommandField == DicomCommandField.NCreateRequest)
            {
                Platform.Log(LogLevel.Debug,
                             String.Format("MPPS NCreate request From {0} at ip {1}:{2}", association.CallingAE,
                                           association.RemoteEndPoint.Address,
                                           association.RemoteEndPoint.Port));
                try
                {
                    server.SendNCreateResponse(presentationID, message.MessageId, new MPPS().Create(message),
                                               DicomStatuses.Success);
                    return true;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Exception thrown while processing MPPS N-CREATE");
                    server.SendNCreateResponse(presentationID, message.MessageId, new DicomMessage(),
                                               DicomStatuses.ProcessingFailure);
                    return true;
                }
            }
            if (message.CommandField == DicomCommandField.NSetRequest)
            {
                Platform.Log(LogLevel.Debug,
                             String.Format("MPPS NSet request From {0} at ip {1}:{2}", association.CallingAE,
                                           association.RemoteEndPoint.Address,
                                           association.RemoteEndPoint.Port));
                try
                {
                    server.SendNSetResponse(presentationID, message.MessageId, new MPPS().Set(message),
                                            DicomStatuses.Success);
                }
                catch (Exception e)
                {

                    Platform.Log(LogLevel.Error, e, "Exception thrown while processing MPPS N-SET");
                    server.SendNSetResponse(presentationID, message.MessageId, new DicomMessage(),
                                               DicomStatuses.ProcessingFailure);
                    return true;
                }

                return true;
            }

            return false;
        }

        public void SetContext(MPPSShredContext context)
        {
        }

        public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
        {
            return DicomPresContextResult.Accept; //just accept for now
        }

        public void Cleanup()
        {
        }

        #endregion

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