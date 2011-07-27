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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class SearchPanelComponentExtensionPoint : ExtensionPoint<ISearchPanelComponent>
	{
	}

	public class SearchRequestEventArgs : EventArgs
	{
		public SearchRequestEventArgs(List<QueryParameters> queryParametersList)
		{
			this.QueryParametersList = queryParametersList;
		}

		public List<QueryParameters> QueryParametersList { get; private set; }
	}

	public interface ISearchPanelComponent : IApplicationComponent
	{
		event EventHandler<SearchRequestEventArgs> SearchRequestEvent;
	}
}
