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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public sealed class PriorStudyFinderExtensionPoint : ExtensionPoint<IPriorStudyFinder>
	{
	}

	/// <summary>
	/// Abstract base class for an <see cref="IPriorStudyFinder"/>.
	/// </summary>
	public abstract class PriorStudyFinder : IPriorStudyFinder
	{
		private class NullPriorStudyFinder : IPriorStudyFinder
		{
			public NullPriorStudyFinder()
			{
			}

			public PriorStudyFinderResult FindPriorStudies()
			{
                return new PriorStudyFinderResult(new StudyItemList(), true);
			}

			#region IPriorStudyFinder Members

			public void SetImageViewer(IImageViewer viewer)
			{
			}

			public void Cancel()
			{
			}

			#endregion
		}

		/// <summary>
		/// Convenient static property for an <see cref="IPriorStudyFinder"/> that does nothing.
		/// </summary>
		public static readonly IPriorStudyFinder Null = new NullPriorStudyFinder();

		private IImageViewer _viewer;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected PriorStudyFinder()
		{
		}

		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		protected IImageViewer Viewer
		{
			get { return _viewer; }	
		}

		#region IPriorStudyFinder Members

		/// <summary>
		/// Sets the <see cref="IImageViewer"/> for which prior studies are to found (and added/loaded).
		/// </summary>
		public void SetImageViewer(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		/// <summary>
		/// Gets the list of prior studies.
		/// </summary>
        public abstract PriorStudyFinderResult FindPriorStudies();

		/// <summary>
		/// Cancels the search for prior studies.
		/// </summary>
		public abstract void Cancel();

		#endregion

		public static IPriorStudyFinder Create()
		{
			try
			{
				return (IPriorStudyFinder)new PriorStudyFinderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}

			return Null;
		}
	}
}
