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
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	public class FusionTestDataContainer : IDisposable
	{
		public delegate IEnumerable<ISopDataSource> CreateSopsDelegate();

		private IEnumerable<ISopDataSource> _baseSops;
		private IEnumerable<ISopDataSource> _overlaySops;
		private TestDisplaySetGenerator _testDisplaySetGenerator;

		public FusionTestDataContainer(TestDataFunction function, Vector3D spacing, Vector3D orientationX, Vector3D orientationY, Vector3D orientationZ)
			: this(function, spacing, spacing, orientationX, orientationY, orientationZ) {}

		public FusionTestDataContainer(TestDataFunction function, Vector3D baseSpacing, Vector3D overlaySpacing,
		                               Vector3D orientationX, Vector3D orientationY, Vector3D orientationZ)
			: this(function,
			       true, baseSpacing, orientationX, orientationY, orientationZ,
			       true, overlaySpacing, orientationX, orientationY, orientationZ) {}

		public FusionTestDataContainer(TestDataFunction function,
		                               Vector3D baseSpacing, Vector3D baseOrientationX, Vector3D baseOrientationY, Vector3D baseOrientationZ,
		                               Vector3D overlaySpacing, Vector3D overlayOrientationX, Vector3D overlayOrientationY, Vector3D overlayOrientationZ)
			: this(function,
			       true, baseSpacing, baseOrientationX, baseOrientationY, baseOrientationZ,
			       true, overlaySpacing, overlayOrientationX, overlayOrientationY, overlayOrientationZ) {}

		public FusionTestDataContainer(TestDataFunction function,
		                               bool baseIsSigned, Vector3D baseSpacing, Vector3D baseOrientationX, Vector3D baseOrientationY, Vector3D baseOrientationZ,
		                               bool overlayIsSigned, Vector3D overlaySpacing, Vector3D overlayOrientationX, Vector3D overlayOrientationY, Vector3D overlayOrientationZ)
		{
			_baseSops = function.CreateSops(baseIsSigned, Modality.CT, baseSpacing, baseOrientationX, baseOrientationY, baseOrientationZ);
			_overlaySops = function.CreateSops(overlayIsSigned, Modality.PT, overlaySpacing, overlayOrientationX, overlayOrientationY, overlayOrientationZ);
			_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSops, _overlaySops);
		}

		public FusionTestDataContainer(CreateSopsDelegate baseSopCreatorDelegate, CreateSopsDelegate overlaySopCreatorDelegate)
		{
			_baseSops = baseSopCreatorDelegate.Invoke();
			_overlaySops = overlaySopCreatorDelegate.Invoke();
			_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSops, _overlaySops);
		}

		public IList<ImageSop> BaseSops
		{
			get { return _testDisplaySetGenerator.BaseSops; }
		}

		public IList<ImageSop> OverlaySops
		{
			get { return _testDisplaySetGenerator.OverlaySops; }
		}

		public IDisplaySet CreateBaseDisplaySet()
		{
			return _testDisplaySetGenerator.CreateBaseDisplaySet();
		}

		public IDisplaySet CreateOverlayDisplaySet()
		{
			return _testDisplaySetGenerator.CreateOverlayDisplaySet();
		}

		public IDisplaySet CreateFusionDisplaySet()
		{
			return _testDisplaySetGenerator.CreateFusionDisplaySet();
		}

		public void Dispose()
		{
			if (_testDisplaySetGenerator != null)
			{
				_testDisplaySetGenerator.Dispose();
				_testDisplaySetGenerator = null;
			}
			if (_overlaySops != null)
			{
				Dispose(_overlaySops);
				_overlaySops = null;
			}
			if (_baseSops != null)
			{
				Dispose(_baseSops);
				_baseSops = null;
			}
		}

		private static void Dispose<T>(IEnumerable<T> list) where T : IDisposable
		{
			foreach (var item in list)
				item.Dispose();
		}
	}
}

#endif