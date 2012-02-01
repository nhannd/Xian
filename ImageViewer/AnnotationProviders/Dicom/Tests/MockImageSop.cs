#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Tests;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Tests
{
	internal class MockImageSop : ImageSop
	{
		private ISopDataSource _sopDataSource;

		public MockImageSop()
			: this(CreateSopDataSource()) {}

		private MockImageSop(ISopDataSource sopDataSource)
			: base(sopDataSource)
		{
			_sopDataSource = sopDataSource;
		}

		public override int NumberOfFrames
		{
			get { return 1; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sopDataSource != null)
				{
					_sopDataSource.Dispose();
					_sopDataSource = null;
				}
			}
			base.Dispose(disposing);
		}

		private static ISopDataSource CreateSopDataSource()
		{
			var uid = DicomUid.GenerateUid().UID;
			var dcf = new DicomFile();
			dcf.MediaStorageSopInstanceUid = uid;
			dcf.MediaStorageSopClassUid = DicomUids.SecondaryCaptureImageStorage.UID;
			dcf.DataSet[DicomTags.SopInstanceUid].SetStringValue(uid);
			dcf.DataSet[DicomTags.SopClassUid].SetStringValue(DicomUids.SecondaryCaptureImageStorage.UID);
			return new TestDataSource(dcf);
		}
	}
}

#endif