#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Oto;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Threading;

namespace ClearCanvas.ImageViewer.Oto
{
	[ExtensionOf(typeof(OtoServiceExtensionPoint))]
	[OtoServiceBehaviour(UseSynchronizationContext = true)]
	public class ImageViewerService : OtoServiceBase
	{
		private static readonly Dictionary<int, IImageViewer> _viewers = new Dictionary<int, IImageViewer>();
		private static int _viewerId;


		#region StudyProperties

		[DataContract]
		public class StudyProperties
		{
			[DataMember]
			[Description("DICOM Patient ID")]
			public string PatientId;

			[DataMember]
			[Description("DICOM Accession Number")]
			public string AccessionNumber;

			[DataMember]
			[Description("DICOM Patients Name")]
			public string PatientsName;

			[DataMember]
			[Description("DICOM Study Date")]
			public DateTime? StudyDate;

			[DataMember]
			[Description("DICOM Study Description")]
			public string StudyDescription;

			[DataMember]
			[Description("DICOM Modalities in Study")]
			public string ModalitiesInStudy;

			[DataMember]
			[Description("DICOM Study Instance UID")]
			public string StudyInstanceUID;
		}

		#endregion


		[DataContract]
		public class QueryInput : StudyProperties
		{
		}

		[DataContract]
		public class QueryOutput
		{
			public QueryOutput(List<StudyProperties> results)
			{
				this.Results = results;
			}

			[DataMember]
			[Description("Results of the query.")]
			public List<StudyProperties> Results;
		}

		[Description("Query the local datastore for studies matching the specified properties.")]
		public QueryOutput QueryStudies(QueryInput input)
		{
			var criteria = new DicomAttributeCollection();
			criteria[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			criteria[DicomTags.PatientId].SetStringValue(input.PatientId);
			criteria[DicomTags.AccessionNumber].SetStringValue(input.AccessionNumber);
			criteria[DicomTags.PatientsName].SetStringValue(input.PatientsName);
			criteria[DicomTags.StudyDate].SetDateTime(0, input.StudyDate);
			criteria[DicomTags.StudyDescription].SetStringValue(input.StudyDescription);
			criteria[DicomTags.PatientsBirthDate].SetStringValue("");
			criteria[DicomTags.ModalitiesInStudy].SetStringValue(input.ModalitiesInStudy);
			criteria[DicomTags.SpecificCharacterSet].SetStringValue("");
			criteria[DicomTags.StudyInstanceUid].SetStringValue(input.StudyInstanceUID);

			var reader = DataAccessLayer.GetIDataStoreReader();
			var results = reader.Query(criteria);

			return new QueryOutput(
				CollectionUtils.Map(results,
					delegate(DicomAttributeCollection result)
					{
						var item = new StudyProperties();
						item.PatientId = result[DicomTags.PatientId].ToString();
						item.PatientsName = result[DicomTags.PatientsName].ToString();
						item.StudyDate = result[DicomTags.StudyDate].GetDateTime(0);
						item.StudyDescription = result[DicomTags.StudyDescription].ToString();
						item.ModalitiesInStudy = result[DicomTags.ModalitiesInStudy].ToString();
						item.AccessionNumber = result[DicomTags.AccessionNumber].ToString();
						item.StudyInstanceUID = result[DicomTags.StudyInstanceUid].ToString();
						return item;
					}));
		}

		[DataContract]
		public class OpenViewerInput
		{
			[Description("List of study instance UIDs for studies to be opened.")]
			[DataMember]
			public string[] StudyInstanceUids { get; set; }
		}

		[DataContract]
		public class OpenViewerOutput
		{

			[Description("An identifier that can be used to perform subsequent operations on the viewer.")]
			[DataMember]
			public object ViewerId { get; set; }
		}


		[Description("Opens the specified studies in a new viewer workspace.")]
		public OpenViewerOutput OpenViewer(OpenViewerInput input)
		{
			Platform.CheckForNullReference(input, "input");
			Platform.CheckMemberIsSet(input.StudyInstanceUids, "StudyInstanceUids");

			if (input.StudyInstanceUids.Length == 0)
				return new OpenViewerOutput();

			var helper = new OpenStudyHelper();
			foreach (var studyInstanceUid in input.StudyInstanceUids)
				helper.AddStudy(studyInstanceUid, null, "DICOM_LOCAL");

			var id = Interlocked.Increment(ref _viewerId);
			helper.Title = "imageviewer " + id;

			var viewer = helper.OpenStudies();

			var workspace = Application.ActiveDesktopWindow.Workspaces
				.First(w => ImageViewerComponent.GetAsImageViewer(w) == viewer);

			lock(_viewers)
			{
				_viewers.Add(id, viewer);
			}

			workspace.Closed += (sender, e) =>
									{
										lock (_viewers)
										{
											_viewers.Remove(id);
										}
									};

			return new OpenViewerOutput {ViewerId = id};
		}


		[DataContract]
		public class CloseViewerInput
		{
			[Description("Viewer to close.")]
			[DataMember]
			public object ViewerId { get; set; }
		}

		[DataContract]
		public class CloseViewerOutput
		{
		}


		[Description("Closes the specified viewer.")]
		public CloseViewerOutput CloseViewer(CloseViewerInput input)
		{
			Platform.CheckForNullReference(input, "input");
			Platform.CheckMemberIsSet(input.ViewerId, "ViewerId");

			var viewer = _viewers[(int)input.ViewerId];
			var workspace = Application.ActiveDesktopWindow.Workspaces
				.First(w => ImageViewerComponent.GetAsImageViewer(w) == viewer);

			workspace.Close(UserInteraction.NotAllowed);

			return new CloseViewerOutput();
		}

		[DataContract]
		public class QueryPhysicalWorkspaceInput
		{
			[Description("Viewer whose physical workspace is to be queried.")]
			[DataMember]
			public object ViewerId { get; set; }
		}
		[DataContract]
		public class QueryPhysicalWorkspaceOutput
		{
			[Description("Number of rows.")]
			[DataMember]
			public int Rows { get; set; }

			[Description("Number of columns.")]
			[DataMember]
			public int Columns { get; set; }
		}

		[Description("Queries the specified viewer for information about its physical workspace.")]
		public QueryPhysicalWorkspaceOutput QueryPhysicalWorkspace(QueryPhysicalWorkspaceInput input)
		{
			Platform.CheckForNullReference(input, "input");
			Platform.CheckMemberIsSet(input.ViewerId, "ViewerId");

			var viewer = _viewers[(int)input.ViewerId];
			var pw = viewer.PhysicalWorkspace;

			return new QueryPhysicalWorkspaceOutput {Rows = pw.Rows, Columns = pw.Columns};
		}
	}
}
