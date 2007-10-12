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

namespace ClearCanvas.Dicom.OffisNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.OffisWrapper;

    /// <summary>
    /// Parses the condition code returned from the OFFIS library and determines a proper error message 
    /// as represented by a string.
    /// </summary>
    public static class OffisConditionParser
    {
        internal static string GetTextString(ApplicationEntity ae, DicomRuntimeApplicationException e)
        {
            ushort module = e.Module; // module in the OFFIS library 0x06 is dcmnet
            ushort code = e.Code;
            ushort submodule = (ushort)(code & 0x0f00);
            ushort subcode = (ushort)(code & 0x00ff);

            if (0x00 == module && 0x00 == code)
                return SR.ExceptionApplicationLevelLogic;

            if (0x06 != module)
                throw new System.ArgumentOutOfRangeException("e.Module", SR.ExceptionDicomCannotHandleNonDcmNetModules);

            StringBuilder exceptionMessage = new StringBuilder(256);

            string submoduleName = null;
            string errorText = null;

            switch (submodule)
            {
                case 0x0100:
                    submoduleName = SR.ExceptionOffisModuleAssociation;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisAssociationBadPresentationContextId;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisAssociationMissingTransferSyntax;
                            break;
                        case 0x06:
                            errorText = SR.ExceptionOffisAssociationNullKey;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisAssociationShutdownApplication;
                            break;
                        default:
                            throw new NetworkDicomException(SR.ExceptionOffisUnknownError);
                    }
                    break;
                case 0x0200:
                    submoduleName = SR.ExceptionOffisModuleDimse;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisDimseBadCommandType;
                            break;
                        case 0x02:
                            errorText = SR.ExceptionOffisDimseBadData;
                            break;
                        case 0x03:
                            errorText = SR.ExceptionOffisDimseBadMessage;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisDimseIllegalAssociation;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisDimseNoDataAvailable;
                            break;
                        case 0x08:
                            errorText = SR.ExceptionOffisDimseNoValidPresentationContextId;
                            break;
                        case 0x09:
                            errorText = SR.ExceptionOffisDimseNullKey;
                            break;
                        case 0x0a:
                            errorText = SR.ExceptionOffisDimseOutOfResources;
                            break;
                        case 0x0b:
                            errorText = SR.ExceptionOffisDimseParseFailed;
                            break;
                        case 0x0c:
                            errorText = SR.ExceptionOffisDimseReadPDVFailed;
                            break;
                        case 0x0d:
                            errorText = SR.ExceptionOffisDimseReceiveFailed;
                            break;
                        case 0x0e:
                            errorText = SR.ExceptionOffisDimseSendFailed;
                            break;
                        case 0x0f:
                            errorText = SR.ExceptionOffisDimseUnexpectedPDVType;
                            break;
                        case 0x13:
                            errorText = SR.ExceptionOffisDimseNoDataDictionary;
                            break;
                        default:
                            throw new DicomException(SR.ExceptionOffisUnknownError);
                    }

                    break;
                case 0x0300:
                    submoduleName = SR.ExceptionOffisModuleDul;

                    switch (subcode)
                    {
                        case 0x01:
                            errorText = SR.ExceptionOffisDulAssociationRejected;
                            break;
                        case 0x04:
                            errorText = SR.ExceptionOffisDulIllegalAccept;
                            break;
                        case 0x05:
                            errorText = SR.ExceptionOffisDulIllegalKey;
                            break;
                        case 0x07:
                            errorText = SR.ExceptionOffisDulIllegalPdu;
                            break;
                        case 0x08:
                            errorText = SR.ExceptionOffisDulIllegalPduLength;
                            break;
                        case 0x0b:
                            errorText = SR.ExceptionOffisDulIllegalRequest;
                            break;
                        case 0x0d:
                            errorText = SR.ExceptionOffisDulIncorrectBufferLength;
                            break;
                        case 0xe:
                            errorText = SR.ExceptionOffisDulInsufficientBufferLength;
                            break;
                        case 0x0f:
                            errorText = SR.ExceptionOffisDulListError;
                            break;
                        case 0x10:
                            errorText = SR.ExceptionOffisDulNetworkClosed;
                            break;
                        case 0x12:
                            errorText = SR.ExceptionOffisDulNoAssociationRequest;
                            break;
                        case 0x13:
                            errorText = SR.ExceptionOffisDulNoPdvs;
                            break;
                        case 0x14:
                            errorText = SR.ExceptionOffisDulNullKey;
                            break;
                        case 0x15:
                            errorText = SR.ExceptionOffisDulPCTranslationFailure;
                            break;
                        case 0x16:
                            errorText = SR.ExceptionOffisDulPDataPduArrived;
                            break;
                        case 0x17:
                            errorText = SR.ExceptionOffisDulPeerAbortedAssociation;
                            break;
                        case 0x19:
                            errorText = SR.ExceptionOffisDulPeerRequestedRelease;
                            break;
                        case 0x1a:
                            errorText = SR.ExceptionOffisDulReadTimeout;
                            break;
                        case 0x1b:
                            errorText = SR.ExceptionOffisDulRequestAssociationFailed;
                            break;
                        case 0x1f:
                            errorText = SR.ExceptionOffisDulUnexpectedPdu;
                            break;
                        case 0x22:
                            errorText = SR.ExceptionOffisDulUnsupportedPeerProtocol;
                            break;
                        case 0x23:
                            errorText = SR.ExceptionOffisDulWrongDataType;
                            break;
                        default:
							throw new DicomException(SR.ExceptionOffisUnknownError);
                    }

                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(SR.ExceptionDecodingBadModuleNumber);
            }

            exceptionMessage.AppendFormat(SR.ExceptionOffisFormatNetwork, submoduleName, errorText, ae.AE, ae.Host + ":" + ae.Port, e.Code);
            return exceptionMessage.ToString();
        }
    }
}
