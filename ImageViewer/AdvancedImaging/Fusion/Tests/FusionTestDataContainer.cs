#region License

// Copyright (c) 2010, ClearCanvas Inc.
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