using System;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.Iod.ContextGroups
{
	public sealed class KeyObjectSelectionDocumentTitleContextGroup : ContextGroupBase<KeyObjectSelectionDocumentTitle>
	{
		private KeyObjectSelectionDocumentTitleContextGroup() : base(7010, "Key Object Selection Document Title", true, new DateTime(2004, 09, 20)) {}

		public static readonly KeyObjectSelectionDocumentTitle OfInterest = new KeyObjectSelectionDocumentTitle(113000, "Of Interest");
		public static readonly KeyObjectSelectionDocumentTitle RejectedForQualityReasons = new KeyObjectSelectionDocumentTitle(113001, "Rejected for Quality Reasons");
		public static readonly KeyObjectSelectionDocumentTitle ForReferringProvider = new KeyObjectSelectionDocumentTitle(113002, "For Referring Provider");
		public static readonly KeyObjectSelectionDocumentTitle ForSurgery = new KeyObjectSelectionDocumentTitle(113003, "For Surgery");
		public static readonly KeyObjectSelectionDocumentTitle ForTeaching = new KeyObjectSelectionDocumentTitle(113004, "For Teaching");
		public static readonly KeyObjectSelectionDocumentTitle ForConference = new KeyObjectSelectionDocumentTitle(113005, "For Conference");
		public static readonly KeyObjectSelectionDocumentTitle ForTherapy = new KeyObjectSelectionDocumentTitle(113006, "For Therapy");
		public static readonly KeyObjectSelectionDocumentTitle ForPatient = new KeyObjectSelectionDocumentTitle(113007, "For Patient");
		public static readonly KeyObjectSelectionDocumentTitle ForPeerReview = new KeyObjectSelectionDocumentTitle(113008, "For Peer Review");
		public static readonly KeyObjectSelectionDocumentTitle ForResearch = new KeyObjectSelectionDocumentTitle(113009, "For Research");
		public static readonly KeyObjectSelectionDocumentTitle QualityIssue = new KeyObjectSelectionDocumentTitle(113010, "Quality Issue");
		public static readonly KeyObjectSelectionDocumentTitle BestInSet = new KeyObjectSelectionDocumentTitle(113013, "Best In Set");
		public static readonly KeyObjectSelectionDocumentTitle ForPrinting = new KeyObjectSelectionDocumentTitle(113018, "For Printing");
		public static readonly KeyObjectSelectionDocumentTitle ForReportAttachment = new KeyObjectSelectionDocumentTitle(113020, "For Report Attachment");
		public static readonly KeyObjectSelectionDocumentTitle Manifest = new KeyObjectSelectionDocumentTitle(113030, "Manifest");
		public static readonly KeyObjectSelectionDocumentTitle SignedManifest = new KeyObjectSelectionDocumentTitle(113031, "Signed Manifest");
		public static readonly KeyObjectSelectionDocumentTitle CompleteStudyContent = new KeyObjectSelectionDocumentTitle(113032, "Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteStudyContent = new KeyObjectSelectionDocumentTitle(113033, "Signed Complete Study Content");
		public static readonly KeyObjectSelectionDocumentTitle CompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113034, "Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle SignedCompleteAcquisitionContent = new KeyObjectSelectionDocumentTitle(113035, "Signed Complete Acquisition Content");
		public static readonly KeyObjectSelectionDocumentTitle GroupOfFramesForDisplay = new KeyObjectSelectionDocumentTitle(113036, "Group of Frames for Display");

		#region Singleton Instancing

		private static readonly KeyObjectSelectionDocumentTitleContextGroup _contextGroup = new KeyObjectSelectionDocumentTitleContextGroup();

		public static KeyObjectSelectionDocumentTitleContextGroup GetInstance()
		{
			return _contextGroup;
		}

		#endregion

		#region Static Enumeration of Values

		private static readonly IList<KeyObjectSelectionDocumentTitle> _valueList;

		static KeyObjectSelectionDocumentTitleContextGroup()
		{
			List<KeyObjectSelectionDocumentTitle> valueList = new List<KeyObjectSelectionDocumentTitle>();
			valueList.Add(OfInterest);
			valueList.Add(RejectedForQualityReasons);
			valueList.Add(ForReferringProvider);
			valueList.Add(ForSurgery);
			valueList.Add(ForTeaching);
			valueList.Add(ForConference);
			valueList.Add(ForTherapy);
			valueList.Add(ForPatient);
			valueList.Add(ForPeerReview);
			valueList.Add(ForResearch);
			valueList.Add(QualityIssue);
			valueList.Add(BestInSet);
			valueList.Add(ForPrinting);
			valueList.Add(ForReportAttachment);
			valueList.Add(Manifest);
			valueList.Add(SignedManifest);
			valueList.Add(CompleteStudyContent);
			valueList.Add(SignedCompleteStudyContent);
			valueList.Add(CompleteAcquisitionContent);
			valueList.Add(SignedCompleteAcquisitionContent);
			valueList.Add(GroupOfFramesForDisplay);
			_valueList = valueList.AsReadOnly();
		}

		public static IList<KeyObjectSelectionDocumentTitle> Values
		{
			get { return _valueList; }
		}

		public override IEnumerator<KeyObjectSelectionDocumentTitle> GetEnumerator()
		{
			return _valueList.GetEnumerator();
		}

		#endregion
	}

	public sealed class KeyObjectSelectionDocumentTitle : KeyObjectSelectionDocumentTitleContextGroup.ContextGroupItemBase {
		internal KeyObjectSelectionDocumentTitle(int value, string meaning) : base("DCM", value.ToString(), meaning) { }

		public static KeyObjectSelectionDocumentTitleContextGroup ContextGroup {
			get { return KeyObjectSelectionDocumentTitleContextGroup.GetInstance(); }
		}
	}
}