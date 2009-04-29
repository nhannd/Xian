using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class ImageStorageScpExtension : StoreScpExtension
	{
		public ImageStorageScpExtension()
			: base(GetSupportedSops())
		{}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			foreach (SopClass sopClass in GetSopClasses(DicomServerSettings.Instance.ImageStorageSopClasses))
			{
				SupportedSop supportedSop = new SupportedSop();
				supportedSop.SopClass = sopClass;

				supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);

				foreach (TransferSyntax transferSyntax in GetTransferSyntaxes(DicomServerSettings.Instance.StorageTransferSyntaxes))
				{
					if (transferSyntax.DicomUid.UID != TransferSyntax.ExplicitVrLittleEndianUid &&
						transferSyntax.DicomUid.UID != TransferSyntax.ImplicitVrLittleEndianUid)
					{
						supportedSop.AddSyntax(transferSyntax);
					}
				}

				yield return supportedSop;
			}
		}
	}

	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class NonImageStorageScpExtension : StoreScpExtension
	{
		public NonImageStorageScpExtension()
			: base(GetSupportedSops())
		{ }

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			foreach (SopClass sopClass in GetSopClasses(DicomServerSettings.Instance.NonImageStorageSopClasses))
			{
				SupportedSop supportedSop = new SupportedSop();
				supportedSop.SopClass = sopClass;
				supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);
				yield return supportedSop;
			}
		}
	}

	public abstract class StoreScpExtension : ScpExtension, IDicomScp<IDicomServerContext>
	{
		protected StoreScpExtension(IEnumerable<SupportedSop> supportedSops)
			: base(supportedSops)
		{
		}

		protected static IEnumerable<SopClass> GetSopClasses(SopClassConfigurationElementCollection config)
		{
			foreach (SopClassConfigurationElement element in config)
			{
				if (!String.IsNullOrEmpty(element.Uid))
				{
					SopClass sopClass = SopClass.GetSopClass(element.Uid);
					if (sopClass != null)
						yield return sopClass;
				}
			}
		}

		protected static IEnumerable<TransferSyntax> GetTransferSyntaxes(TransferSyntaxConfigurationElementCollection config)
		{
			foreach (TransferSyntaxConfigurationElement element in config)
			{
				if (!String.IsNullOrEmpty(element.Uid))
				{
					TransferSyntax syntax = TransferSyntax.GetTransferSyntax(element.Uid);
					if (syntax != null)
					{
						//at least for now, restrict to available codecs for compressed syntaxes.
						if (!syntax.Encapsulated || ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodec(syntax) != null)
							yield return syntax;
					}
				}
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
