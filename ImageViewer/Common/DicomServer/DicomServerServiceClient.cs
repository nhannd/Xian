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
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    // TODO (Marmot): remove this stuff.
    
	public partial class DicomServerServiceClient : ClientBase<IDicomServerService>, IDicomServerService
	{
		public DicomServerServiceClient()
		{
		}

		[Obsolete("Use the SendStudies method instead.")]
        public void Send(ApplicationEntity destinationAEInformation, IEnumerable<string> studyInstanceUids)
		{
			base.Channel.Send(destinationAEInformation, studyInstanceUids);
		}

        public void RetrieveStudies(ApplicationEntity sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			base.Channel.RetrieveStudies(sourceAEInformation, studiesToRetrieve);
		}

        public void RetrieveSeries(ApplicationEntity sourceAEInformation, StudyInformation studyInformation, IEnumerable<string> seriesInstanceUids)
		{
			base.Channel.RetrieveSeries(sourceAEInformation, studyInformation, seriesInstanceUids);
		}

		public DicomServerConfiguration GetServerConfiguration()
		{
			return base.Channel.GetServerConfiguration();
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			base.Channel.UpdateServerConfiguration(newConfiguration);
		}
	}
}
