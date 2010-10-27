#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	[ExtensionPoint]
	public class ViewerContextMenuFilterExtensionPoint : ExtensionPoint<IViewerActionFilter>
	{}

	//TODO (CR Mar 2010): Predicate!
	public interface IViewerActionFilter
	{
		void SetImageViewer(IImageViewer imageViewer);
        
		bool Evaluate(IAction action);
	}

	//TODO (CR Sept 2010): remove this stuff
	public abstract class ViewerActionFilter : IViewerActionFilter
	{
		private class AlwaysTrueFilter : IViewerActionFilter
		{
			#region IViewerActionFilter Members

			public void SetImageViewer(IImageViewer imageViewer)
			{
			}

			public bool Evaluate(IAction action)
			{
				return true;
			}

			#endregion
		}

		public static readonly IViewerActionFilter Null = new AlwaysTrueFilter();

		protected ViewerActionFilter()
		{}

		protected IImageViewer ImageViewer { get; private set; }

		#region IViewerActionFilter Members

		void IViewerActionFilter.SetImageViewer(IImageViewer imageViewer)
		{
			ImageViewer = imageViewer;
		}

		public abstract bool Evaluate(IAction action);

		#endregion

		public static IViewerActionFilter CreateContextMenuFilter()
		{
			try
			{
				return (IViewerActionFilter)new ViewerContextMenuFilterExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return Null;
			}
		}
	}
}
