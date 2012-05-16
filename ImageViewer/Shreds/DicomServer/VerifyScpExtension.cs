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

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class VerifyScpExtension : ScpExtension
	{
		public VerifyScpExtension()
			: base(GetSupportedSops())
		{
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
		    var sop = new SupportedSop
		                  {
		                      SopClass = SopClass.VerificationSopClass
		                  };
		    sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

        public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			server.SendCEchoResponse(presentationID, message.MessageId, DicomStatuses.Success);
			return true;
		}
	}
}
