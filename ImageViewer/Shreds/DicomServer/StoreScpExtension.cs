using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices;
using ClearCanvas.DicomServices.Codec;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class StoreScpExtension : ScpExtension, IDicomScp<IDicomServerContext>
	{
		public StoreScpExtension()
			: base(GetSupportedSops())
		{
		}

		private new static IEnumerable<SopClass> GetSupportedSopClasses()
		{
			//TODO: load this from a setting or xml file
			yield return SopClass.ComputedRadiographyImageStorage;
			yield return SopClass.CtImageStorage;
			
			yield return SopClass.DigitalIntraOralXRayImageStorageForPresentation;
			yield return SopClass.DigitalIntraOralXRayImageStorageForProcessing;

			yield return SopClass.DigitalMammographyXRayImageStorageForPresentation;
			yield return SopClass.DigitalMammographyXRayImageStorageForProcessing;

			yield return SopClass.DigitalXRayImageStorageForPresentation;
			yield return SopClass.DigitalXRayImageStorageForProcessing;

			yield return SopClass.EnhancedCtImageStorage;
			yield return SopClass.EnhancedMrImageStorage;

			yield return SopClass.EnhancedXaImageStorage;

			yield return SopClass.EnhancedXrfImageStorage;

			yield return SopClass.MrImageStorage;

			yield return SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameSingleBitSecondaryCaptureImageStorage;
			yield return SopClass.MultiFrameTrueColorSecondaryCaptureImageStorage;

			yield return SopClass.NuclearMedicineImageStorageRetired;
			yield return SopClass.NuclearMedicineImageStorage;

			yield return SopClass.OphthalmicPhotography16BitImageStorage;
			yield return SopClass.OphthalmicPhotography8BitImageStorage;
			yield return SopClass.OphthalmicTomographyImageStorage;

			yield return SopClass.PositronEmissionTomographyImageStorage;

			yield return SopClass.RtImageStorage;

			yield return SopClass.SecondaryCaptureImageStorage;

			yield return SopClass.UltrasoundImageStorage;
			yield return SopClass.UltrasoundImageStorageRetired;
			yield return SopClass.UltrasoundMultiFrameImageStorage;
			yield return SopClass.UltrasoundMultiFrameImageStorageRetired;

			yield return SopClass.VideoEndoscopicImageStorage;
			yield return SopClass.VideoMicroscopicImageStorage;
			yield return SopClass.VideoPhotographicImageStorage;

			yield return SopClass.VlEndoscopicImageStorage;
			yield return SopClass.VlMicroscopicImageStorage;
			yield return SopClass.VlPhotographicImageStorage;
			yield return SopClass.VlSlideCoordinatesMicroscopicImageStorage;

			yield return SopClass.XRay3dAngiographicImageStorage;
			yield return SopClass.XRay3dCraniofacialImageStorage;

			yield return SopClass.XRayAngiographicBiPlaneImageStorageRetired;
			yield return SopClass.XRayAngiographicImageStorage;

			yield return SopClass.XRayRadiofluoroscopicImageStorage;
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			IDicomCodecFactory[] codecs = DicomCodecHelper.GetCodecs();

			foreach (SopClass sopClass in GetSupportedSopClasses())
			{
				SupportedSop sop = new SupportedSop();
				sop.SopClass = sopClass;
				sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
				sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);

				//TODO: should we maybe order them according to preference?
				foreach (IDicomCodecFactory codec in codecs)
				{
					if (codec.CodecTransferSyntax.UidString != TransferSyntax.ExplicitVrLittleEndian.UidString
						&& codec.CodecTransferSyntax.UidString != TransferSyntax.ImplicitVrLittleEndian.UidString)
					{
						sop.SyntaxList.Add(codec.CodecTransferSyntax);
					}
				}

				yield return sop;
			}
		}

		public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, 
			ClearCanvas.Dicom.Network.ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			string studyInstanceUid = null;
			string seriesInstanceUid = null;
			DicomUid sopInstanceUid;

			bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUid].TryGetString(0, out seriesInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid);

			if (!ok)
			{
				Platform.Log(LogLevel.Error, "Unable to retrieve UIDs from request message, sending failure status.");

				server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
					DicomStatuses.ProcessingFailure);

				return true;
			}

			string fileName = Path.GetRandomFileName();
			string path = String.Format("{0}\\{1}.dcm", Context.InterimStorageDirectory, fileName);

			try
			{
				DicomFile dicomFile = new DicomFile(message, path);

				if (message.TransferSyntax.Encapsulated)
					dicomFile.TransferSyntax = message.TransferSyntax;
				else
					dicomFile.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

				dicomFile.MediaStorageSopInstanceUid = sopInstanceUid.UID;
				dicomFile.ImplementationClassUid = DicomImplementation.ClassUID.UID;
				dicomFile.ImplementationVersionName = DicomImplementation.Version;
				dicomFile.SourceApplicationEntityTitle = association.CallingAE;
				dicomFile.MediaStorageSopClassUid = message.SopClass.Uid;

				dicomFile.Save(DicomWriteOptions.None);

				OnFileReceived(association.CallingAE, dicomFile.Filename);

				server.SendCStoreResponse(presentationID, message.MessageId,
				                          sopInstanceUid.UID, DicomStatuses.Success);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Failed to save file to interim directory ({0}).", path);

				server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
					DicomStatuses.ProcessingFailure);

				OnReceiveError(message, e.Message, association.CallingAE);
			}

			return true;
		}

		private static void OnFileReceived(string fromAE, string filename)
		{
			StoreScpReceivedFileInformation info = new StoreScpReceivedFileInformation();
			info.AETitle = fromAE;
			info.FileName = filename;
			LocalDataStoreEventPublisher.Instance.FileReceived(info);
		}

		private static void OnReceiveError(DicomMessage message, string error, string fromAE)
		{
			ReceiveErrorInformation info = new ReceiveErrorInformation();
			info.FromAETitle = fromAE;
			info.ErrorMessage = error;

			info.StudyInformation = new StudyInformation();
			info.StudyInformation.PatientId = message.DataSet[DicomTags.PatientId].GetString(0, "");
			info.StudyInformation.PatientsName = message.DataSet[DicomTags.PatientsName].GetString(0, "");
			info.StudyInformation.StudyDate = DateParser.Parse(message.DataSet[DicomTags.StudyDate].GetString(0, ""));
			info.StudyInformation.StudyDescription = message.DataSet[DicomTags.StudyDescription].GetString(0, "");
			info.StudyInformation.StudyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");

			LocalDataStoreEventPublisher.Instance.ReceiveError(info);
		}
	}
}
