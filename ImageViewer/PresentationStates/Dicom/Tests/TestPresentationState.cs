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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom.Tests
{
	internal class TestPresentationState : DicomSoftcopyPresentationStateBase<TestPresentationImage>
	{
		public TestPresentationState() : base(SopClass.RawDataStorage) {}

		public SpatialTransformModuleIod SerializeSpatialTransform(TestPresentationImage image)
		{
			SpatialTransformModuleIod dataset = new SpatialTransformModuleIod();
			base.SerializeSpatialTransform(dataset, CreateImageCollection(image));
			return dataset;
		}

		public TestPresentationImage DeserializeSpatialTransform(SpatialTransformModuleIod dataset)
		{
			TestPresentationImage image = new TestPresentationImage();
			base.DeserializeSpatialTransform(dataset, image);
			return image;
		}

		protected override void PerformTypeSpecificSerialization(DicomPresentationImageCollection<TestPresentationImage> images)
		{
			throw new NotImplementedException();
		}

		protected override void PerformTypeSpecificDeserialization(DicomPresentationImageCollection<TestPresentationImage> images)
		{
			throw new NotImplementedException();
		}

		private static DicomPresentationImageCollection<TestPresentationImage> CreateImageCollection(params TestPresentationImage[] images)
		{
			return new DicomPresentationImageCollection<TestPresentationImage>(images);
		}
	}
}

#endif