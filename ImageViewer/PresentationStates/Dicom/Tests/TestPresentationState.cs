#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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