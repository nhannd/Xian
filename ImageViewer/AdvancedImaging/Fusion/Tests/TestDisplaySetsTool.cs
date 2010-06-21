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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[MenuAction("setBase", "global-menus/MenuDebug/MenuFusion/Base Display Set", "SetBaseDisplaySet")]
	[MenuAction("setOverlay", "global-menus/MenuDebug/MenuFusion/Overlay Display Set", "SetOverlayDisplaySet")]
	[MenuAction("setFusion", "global-menus/MenuDebug/MenuFusion/Fusion Display Set", "SetFusionDisplaySet")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class TestDisplaySetsTool : ImageViewerTool
	{
		private TestDisplaySetGenerator _testDisplaySetGenerator;
		private IList<ISopDataSource> _baseSopDataSources;
		private IList<ISopDataSource> _overlaySopDataSources;

		private TestDisplaySetGenerator TestDisplaySetGenerator
		{
			get
			{
				if (_testDisplaySetGenerator == null)
				{
					_baseSopDataSources = TestDataFunction.Threed.CreateSops(true, Modality.CT, new Vector3D(0.3f, 0.3f, 0.3f), Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit);
					_overlaySopDataSources = TestDataFunction.Threed.CreateSops(true, Modality.PT, new Vector3D(0.8f, 0.8f, 0.8f), Vector3D.zUnit, Vector3D.xUnit, Vector3D.yUnit);
					_testDisplaySetGenerator = new TestDisplaySetGenerator(_baseSopDataSources, _overlaySopDataSources);
				}
				return _testDisplaySetGenerator;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_testDisplaySetGenerator != null)
				{
					_testDisplaySetGenerator.Dispose();
					_testDisplaySetGenerator = null;
				}

				if (_baseSopDataSources != null)
				{
					DisposeAll(_baseSopDataSources);
					_baseSopDataSources = null;
				}

				if (_overlaySopDataSources != null)
				{
					DisposeAll(_overlaySopDataSources);
					_overlaySopDataSources = null;
				}
			}
			base.Dispose(disposing);
		}

		public void SetBaseDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateBaseDisplaySet());
		}

		public void SetOverlayDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateOverlayDisplaySet());
		}

		public void SetFusionDisplaySet()
		{
			SetDisplaySet(TestDisplaySetGenerator.CreateFusionDisplaySet());
		}

		private void SetDisplaySet(IDisplaySet displaySet)
		{
			if (ImageViewer == null || ImageViewer.PhysicalWorkspace.SelectedImageBox == null)
				return;
			if (ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySetLocked)
				return;
			var oldDisplaySet = ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySet;
			ImageViewer.PhysicalWorkspace.SelectedImageBox.DisplaySet = displaySet;
			if (oldDisplaySet != null)
				oldDisplaySet.Dispose();
			ImageViewer.PhysicalWorkspace.SelectedImageBox.Draw();
		}

		private static void DisposeAll<T>(IEnumerable<T> list) where T : IDisposable
		{
			foreach (var item in list)
				item.Dispose();
		}
	}
}

#endif