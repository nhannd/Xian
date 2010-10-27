#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
{
	//TODO (CR Mar 2010): name, method names?
	//It's a factory, so Create methods?
	//IImageViewerSetupHelper?
	public interface IViewerSetupHelper
	{
		void SetImageViewer(IImageViewer viewer);

		ILayoutManager GetLayoutManager(); 
		
		ITool[] GetTools();
		//TODO (CR Sept 2010): remove this stuff
		IViewerActionFilter GetContextMenuFilter();

		IPriorStudyFinder GetPriorStudyFinder();
	}

	public class ViewerSetupHelper : IViewerSetupHelper
	{
		public ViewerSetupHelper()
		{
		}

		internal ViewerSetupHelper(ILayoutManager layoutManager, IPriorStudyFinder priorStudyFinder)
		{
			LayoutManager = layoutManager;
			PriorStudyFinder = priorStudyFinder;
		}

		protected IImageViewer ImageViewer { get; private set; }
		
		public ILayoutManager LayoutManager { get; set; }
		public ITool[] Tools { get; set; }
		public IViewerActionFilter ContextMenuFilter { get; set; }
		public IPriorStudyFinder PriorStudyFinder { get; set; }

		protected virtual ITool[] GetTools()
		{
			if (Tools != null)
				return Tools;

			try
			{
				object[] extensions = new ImageViewerToolExtensionPoint().CreateExtensions();
				return CollectionUtils.Map(extensions, (object tool) => (ITool) tool).ToArray();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No viewer tool extensions found.");
				return new ITool[0];
			}
		}

		protected virtual IViewerActionFilter GetContextMenuFilter()
		{
			return ContextMenuFilter ?? ViewerActionFilter.CreateContextMenuFilter();
		}

		protected virtual ILayoutManager GetLayoutManager()
		{
			return LayoutManager ?? ClearCanvas.ImageViewer.LayoutManager.Create();
		}

		protected virtual IPriorStudyFinder GetPriorStudyFinder()
		{
			return PriorStudyFinder ?? ClearCanvas.ImageViewer.PriorStudyFinder.Create();
		}

		#region IViewerSetupHelper Members

		void IViewerSetupHelper.SetImageViewer(IImageViewer viewer)
		{
			ImageViewer = viewer;
		}

		ITool[] IViewerSetupHelper.GetTools()
		{
			return GetTools();
		}

		IViewerActionFilter IViewerSetupHelper.GetContextMenuFilter()
		{
			return GetContextMenuFilter();
		}

		ILayoutManager IViewerSetupHelper.GetLayoutManager()
		{
			return GetLayoutManager();
		}

		IPriorStudyFinder IViewerSetupHelper.GetPriorStudyFinder()
		{
			return GetPriorStudyFinder();
		}

		#endregion
	}
}