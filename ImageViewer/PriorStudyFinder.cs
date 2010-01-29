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

			public StudyItemList FindPriorStudies()
			{
				return new StudyItemList();
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
		public abstract StudyItemList FindPriorStudies();

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
