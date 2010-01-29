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

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer
{
	public interface IViewerSetupHelper
	{
		void SetImageViewer(IImageViewer viewer);

		ILayoutManager GetLayoutManager(); 
		
		ITool[] GetTools();
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