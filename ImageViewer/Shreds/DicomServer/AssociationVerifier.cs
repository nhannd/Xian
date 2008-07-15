using ClearCanvas.Dicom.Network;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal static class AssociationVerifier
	{
		public static bool VerifyAssociation(IDicomServerContext context, AssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason)
		{
			string calledTitle = (assocParms.CalledAE ?? "").Trim();
			string callingAE = (assocParms.CallingAE ?? "").Trim();

			result = DicomRejectResult.Permanent;
			reason = DicomRejectReason.NoReasonGiven;

			if (null == RemoteServerDirectory.Lookup(callingAE))
			{
				reason = DicomRejectReason.CallingAENotRecognized;
			}
			else if (calledTitle != context.AETitle)
			{
				reason = DicomRejectReason.CalledAENotRecognized;
			}
			else
			{
				return true;
			}

			return false;
		}
	}
}
