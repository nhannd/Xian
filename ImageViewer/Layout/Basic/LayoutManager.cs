#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
    [ExtensionOf(typeof(LayoutManagerExtensionPoint))]
	public class LayoutManager : ClearCanvas.ImageViewer.LayoutManager
	{
		private bool _physicalWorkspaceLayoutSet = false;
		private readonly DefaultPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();

		public LayoutManager()
		{	
		}

		protected override IImageSet CreateImageSet(Study study)
		{
			PatientInformation info = new PatientInformation(study.ParentPatient);
			PatientInformation reconciled = reconciliationStrategy.ReconcilePatient(info);

			ImageSet imageSet = (ImageSet)base.CreateImageSet(study);
			imageSet.PatientInfo = String.Format("{0} · {1}",
				study.ParentPatient.PatientsName.FormattedName, reconciled.PatientId);

			return imageSet;
		}

		protected override void LayoutPhysicalWorkspace()
		{
			if (_physicalWorkspaceLayoutSet)
				return;

			_physicalWorkspaceLayoutSet = true;

			StoredLayoutConfiguration configuration = null;

			//take the first opened study, enumerate the modalities and compute the union of the layout configuration (in case there are multiple modalities in the study).
			if (LogicalWorkspace.ImageSets.Count > 0)
			{
				IImageSet firstImageSet = LogicalWorkspace.ImageSets[0];
				foreach(IDisplaySet displaySet in firstImageSet.DisplaySets)
				{
					if (displaySet.PresentationImages.Count <= 0)
						continue;

					if (configuration == null)
						configuration = LayoutConfigurationSettings.GetMinimumConfiguration();

					StoredLayoutConfiguration storedConfiguration = LayoutConfigurationSettings.Default.GetLayoutConfiguration(displaySet.PresentationImages[0] as IImageSopProvider);
					configuration.ImageBoxRows = Math.Max(configuration.ImageBoxRows, storedConfiguration.ImageBoxRows);
					configuration.ImageBoxColumns = Math.Max(configuration.ImageBoxColumns, storedConfiguration.ImageBoxColumns);
					configuration.TileRows = Math.Max(configuration.TileRows, storedConfiguration.TileRows);
					configuration.TileColumns = Math.Max(configuration.TileColumns, storedConfiguration.TileColumns);
				}
			}

			if (configuration == null)
				configuration = LayoutConfigurationSettings.Default.DefaultConfiguration;

			PhysicalWorkspace.SetImageBoxGrid(configuration.ImageBoxRows, configuration.ImageBoxColumns);
			for (int i = 0; i < PhysicalWorkspace.ImageBoxes.Count; ++i)
				PhysicalWorkspace.ImageBoxes[i].SetTileGrid(configuration.TileRows, configuration.TileColumns);
		}
	}
}
