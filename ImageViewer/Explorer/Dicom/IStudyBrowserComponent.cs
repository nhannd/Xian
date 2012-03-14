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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class StudyBrowserComponentExtensionPoint : ExtensionPoint<IStudyBrowserComponent>
	{
	}

	public interface IStudyBrowserComponent : IApplicationComponent
	{
		QueryParameters CreateOpenSearchQueryParams();
		SearchResult CreateSearchResult();
		AEServerGroup SelectedServerGroup { get; set; }
		void Search(List<QueryParameters> queryParameters);
		void CancelSearch();

		event EventHandler SearchStarted;
		event EventHandler SearchEnded;
	}
}
