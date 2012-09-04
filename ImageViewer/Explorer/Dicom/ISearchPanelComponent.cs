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

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class SearchPanelComponentExtensionPoint : ExtensionPoint<ISearchPanelComponent>
	{
	}

	public class SearchRequestedEventArgs : EventArgs
	{
		public SearchRequestedEventArgs(StudyRootStudyIdentifier queryCriteria)
		{
		    this.QueryCriteria = queryCriteria;
		}

		public StudyRootStudyIdentifier QueryCriteria { get; private set; }
	}

	public interface ISearchPanelComponent : IApplicationComponent
	{
		void Search();
		void Clear();
		bool SearchInProgress { get; set; }

		event EventHandler<SearchRequestedEventArgs> SearchRequested;
		event EventHandler SearchCancelled;
	}
}
