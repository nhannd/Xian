#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Specifies window launch options for the <see cref="OpenStudyHelper"/>.
	/// </summary>
	public enum WindowBehaviour
	{
		/// <summary>
		/// Unused, currently.
		/// </summary>
		Auto,

		/// <summary>
		/// Specifies that the <see cref="ImageViewerComponent"/> should be launched
		/// in a single (e.g. active) desktop window.
		/// </summary>
		Single,

		/// <summary>
		/// Specifies that the <see cref="ImageViewerComponent"/> should be launched
		/// in a separate desktop window.
		/// </summary>
		Separate
	}

	/// <summary>
	/// Helper class to create, populate and launch an <see cref="ImageViewerComponent"/>.
	/// </summary>
	public static class OpenStudyHelper
	{
		private static IDesktopWindow DesktopWindow
		{
			get { return Application.ActiveDesktopWindow; }
		}

		/// <summary>
		/// Launches a new <see cref="ImageViewerComponent"/> with the specified local files.
		/// </summary>
		public static void OpenFiles(string[] localFileList, WindowBehaviour windowBehaviour)
		{
			Platform.CheckForNullReference(localFileList, "localFileList");
			if (localFileList.Length == 0)
				throw new ArgumentException("localFileList array cannot be empty.");

			ImageViewerComponent viewer = OpenFilesInternal(localFileList);
			if (viewer != null)
				Launch(viewer, windowBehaviour);
		}

		/// <summary>
		/// Launches a new <see cref="ImageViewerComponent"/> with the specified studies.
		/// </summary>
		public static void OpenStudies(string studyLoaderName, string[] studyInstanceUids, WindowBehaviour windowBehaviour)
		{
			Platform.CheckForNullReference(studyLoaderName, "studyLoaderName");
			Platform.CheckForNullReference(studyInstanceUids, "studyInstanceUids");
			if (studyInstanceUids.Length == 0)
				throw new ArgumentException("studyInstanceUids array cannot be empty.");

			ImageViewerComponent imageViewer = null;
			BlockingOperation.Run(delegate { imageViewer = OpenStudiesInternal(studyLoaderName, studyInstanceUids); });

			if (imageViewer != null)
				Launch(imageViewer, windowBehaviour);
		}

		private static ImageViewerComponent OpenStudiesInternal(string studyLoaderName, string[] studyInstanceUids)
		{
			if (studyInstanceUids.Length == 1)
				return OpenSingleStudyWithPriors(studyLoaderName, studyInstanceUids[0]);
			else
				return OpenMultipleStudiesInSingleWorkspace(studyLoaderName, studyInstanceUids);
		}

		private static ImageViewerComponent OpenFilesInternal(string[] localFileList)
		{
			ImageViewerComponent imageViewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);

			bool cancelled = false;
			bool anyFailures = false;
			int successfulImagesInLoadFailure = 0;

			try
			{
				imageViewer.LoadImages(localFileList, DesktopWindow, out cancelled);
			}
			catch (OpenStudyException e)
			{
				anyFailures = true;
				successfulImagesInLoadFailure = e.SuccessfulImages;
				ExceptionHandler.Report(e, DesktopWindow);
			}

			if (cancelled || (anyFailures && successfulImagesInLoadFailure == 0))
			{
				imageViewer.Dispose();
				imageViewer = null;
			}

			return imageViewer;
		}

		// Okay, the method name is deceptive--it doesn't actually
		// open priors yet
		private static ImageViewerComponent OpenSingleStudyWithPriors(string studyLoaderName, string studyInstanceUid)
		{
			ImageViewerComponent imageViewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);

			try
			{
				imageViewer.LoadStudy(studyInstanceUid, studyLoaderName);
			}
			catch (OpenStudyException e)
			{
				ExceptionHandler.Report(e, DesktopWindow);
				if (e.SuccessfulImages == 0)
				{
					imageViewer.Dispose();
					imageViewer = null;
				}
			}

			return imageViewer;
		}

		private static ImageViewerComponent OpenMultipleStudiesInSingleWorkspace(string studyLoaderName, string[] studyInstanceUids)
		{
			ImageViewerComponent imageViewer = new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			int completelySuccessfulStudies = 0;
			int successfulImagesInLoadFailure = 0;

			foreach (string studyInstanceUid in studyInstanceUids)
			{
				try
				{
					imageViewer.LoadStudy(studyInstanceUid, studyLoaderName);
					completelySuccessfulStudies++;
				}
				catch (OpenStudyException e)
				{
					// Study failed to load completely; keep track of how many
					// images in the study actually did load
					successfulImagesInLoadFailure += e.SuccessfulImages;
					ExceptionHandler.Report(e, DesktopWindow);
				}
			}

			// If nothing at all was able to load, then don't bother trying to
			// even open a workspace; just return
			if (completelySuccessfulStudies == 0 && successfulImagesInLoadFailure == 0)
			{
				imageViewer.Dispose();
				imageViewer = null;
			}

			return imageViewer;
		}

		private static void Launch(ImageViewerComponent imageViewer, WindowBehaviour windowBehaviour)
		{
			// Open the images in a separate window
			if (windowBehaviour == WindowBehaviour.Separate)
				ImageViewerComponent.LaunchInSeparateWindow(imageViewer);
			// Open the images in the same window
			else
				ImageViewerComponent.LaunchInActiveWindow(imageViewer);
		}
	}
}
