#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class StudyBrowserComponentExtensionPoint : ExtensionPoint<IStudyBrowserComponent>
	{
	}

	public interface IStudyBrowserComponent : IApplicationComponent
	{
		DicomServiceNodeList SelectedServers { get; set; }
		void Search(StudyRootStudyIdentifier queryCriteria);
		void CancelSearch();

		event EventHandler SearchStarted;
		event EventHandler SearchEnded;
	}
}
