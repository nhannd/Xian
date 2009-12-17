#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Externals.General.Tests
{
	public class MockDicomPresentationImage : BasicPresentationImage, IDicomPresentationImage
	{
		private DicomFile _dicomFile;
		private LocalSopDataSource _sopDataSource;
		private ImageSop _imageSop;
		private Frame _frame;
		private string _filename;

		public MockDicomPresentationImage() : this(string.Format("{0}.dcm", Guid.NewGuid())) {}

		public MockDicomPresentationImage(string filename) : base(new GrayscaleImageGraphic(10, 10))
		{
			if (Path.IsPathRooted(filename))
				_filename = filename;
			else
				_filename = Path.Combine(Environment.CurrentDirectory, filename);

			_dicomFile = new DicomFile();
			_dicomFile.DataSet[DicomTags.SopClassUid].SetStringValue(SopClass.SecondaryCaptureImageStorageUid);
			_dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			_dicomFile.MetaInfo[DicomTags.MediaStorageSopClassUid].SetStringValue(_dicomFile.DataSet[DicomTags.SopClassUid].ToString());
			_dicomFile.MetaInfo[DicomTags.MediaStorageSopInstanceUid].SetStringValue(_dicomFile.DataSet[DicomTags.SopInstanceUid].ToString());
			_dicomFile.Save(_filename);
			_sopDataSource = new LocalSopDataSource(_dicomFile);
			_imageSop = new ImageSop(_sopDataSource);
			_frame = new MockFrame(_imageSop, 1);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_frame = null;
				_dicomFile = null;

				if (_imageSop != null)
				{
					_imageSop.Dispose();
					_imageSop = null;
				}

				if (_sopDataSource != null)
				{
					_sopDataSource.Dispose();
					_sopDataSource = null;
				}

				if (_filename != null)
				{
					if (File.Exists(_filename))
						File.Delete(_filename);
					_filename = null;
				}
			}
			base.Dispose(disposing);
		}

		public string Filename
		{
			get { return _filename; }
		}

		public DicomAttribute this[uint dicomTag]
		{
			get { return _dicomFile.DataSet[dicomTag]; }
		}

		public override IPresentationImage CreateFreshCopy()
		{
			throw new NotImplementedException();
		}

		#region IDicomPresentationImage Members

		GraphicCollection IDicomPresentationImage.DicomGraphics
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IImageSopProvider Members

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		public Frame Frame
		{
			get { return _frame; }
		}

		#endregion

		#region ISopProvider Members

		public Sop Sop
		{
			get { return _imageSop; }
		}

		#endregion

		#region IDicomSoftcopyPresentationStateProvider Members

		DicomSoftcopyPresentationState IDicomSoftcopyPresentationStateProvider.PresentationState
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		#endregion

		private class MockFrame : Frame
		{
			public MockFrame(ImageSop parent, int number) : base(parent, number) {}
		}
	}
}

#endif