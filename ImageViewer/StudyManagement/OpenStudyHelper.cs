#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	#region OpenStudyArgs

	/// <summary>
	/// Holds the parameters that specify the studies to be opened using the <see cref="OpenStudyHelper"/>
	/// </summary>
	[Obsolete("This class will be removed in a future version.  Please use an instance of OpenStudyHelper instead.")]
	public class OpenStudyArgs
	{
		private readonly string[] _studyInstanceUids;
		private readonly WindowBehaviour _windowBehaviour;
		private readonly object _server;
		private readonly string _studyLoaderName;

		/// <summary>
		/// Constructs a new <see cref="OpenStudyArgs"/> using the specified parameters.
		/// </summary>
		/// <param name="studyInstanceUids">The Study Instance UIDs of the studies to be opened.</param>
		/// <param name="server">An object specifying the server to open the study from, such as
		/// <code>null</code> for the local server or an <see cref="ApplicationEntity"/> object specifying the remote server.</param>
		/// <param name="studyLoaderName">The name of the <see cref="IStudyLoader"/> to use, which is specified by <see cref="IStudyLoader.Name"/>.</param>
		/// <param name="windowBehaviour">The window launch options.</param>
		public OpenStudyArgs(
			string[] studyInstanceUids, 
			object server, 
			string studyLoaderName,
			WindowBehaviour windowBehaviour)
		{
			Platform.CheckForNullReference(studyLoaderName, "studyLoaderName");
			Platform.CheckForNullReference(studyInstanceUids, "studyInstanceUids");

			if (studyInstanceUids.Length == 0)
				throw new ArgumentException("studyInstanceUids array cannot be empty.");

			_studyInstanceUids = studyInstanceUids;
			_server = server;
			_studyLoaderName = studyLoaderName;
			_windowBehaviour = windowBehaviour;
		}

		/// <summary>
		/// Gets the Study Instance UIDs of the studies to be opened.
		/// </summary>
		public string[] StudyInstanceUids
		{
			get { return _studyInstanceUids; }
		}

		/// <summary>
		/// Gets the server to open the study from, such as
		/// <code>null</code> for the local server or an <see cref="ApplicationEntity"/> object specifying the remote server.
		/// </summary>
		public object Server
		{
			get { return _server; }
		}

		/// <summary>
		/// Gets the name of the <see cref="IStudyLoader"/> to use, which is specified by <see cref="IStudyLoader.Name"/>.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
		}

		/// <summary>
		/// Gets the window launch options.
		/// </summary>
		public WindowBehaviour WindowBehaviour
		{
			get { return _windowBehaviour; }
		}
	}

	#endregion

	/// <summary>
	/// Helper class to create, populate and launch an <see cref="ImageViewerComponent"/>.
	/// </summary>
	public class OpenStudyHelper
	{
		#region Private Fields

		private readonly List<LoadStudyArgs> _studiesToOpen = new List<LoadStudyArgs>();

		#endregion

		/// <summary>
		/// Constructs a new instance of <see cref="OpenStudyHelper"/>.
		/// </summary>
		public OpenStudyHelper()
		{
			LoadPriors = true;
			WindowBehaviour = WindowBehaviour.Auto;
		}

		#region Launch Options

		/// <summary>
		/// Gets or sets the <see cref="WindowBehaviour"/> for launching the <see cref="ImageViewerComponent"/>.
		/// </summary>
		public WindowBehaviour WindowBehaviour { get; set; }

		/// <summary>
		/// Gets or sets the title that should be used for the workspace in which the <see cref="ImageViewerComponent"/>
		/// will be launced.
		/// </summary>
		/// <remarks>
		/// The default value is null, which means that the title will be automatically figured out.
		/// </remarks>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets whether or not the <see cref="ImageViewerComponent"/> should load any prior studies.
		/// </summary>
		public bool LoadPriors { get; set; }

		/// <summary>
		/// Gets or sets whether or not to allow an empty viewer to be opened (e.g. with no studies loaded).
		/// </summary>
		public bool AllowEmptyViewer { get; set; }
		
		#endregion

		#region Instance Methods

		#region Public

		/// <summary>
		/// Adds a study to the list of studies to be opened.
		/// </summary>
		public void AddStudy(string studyInstanceUid, object server, string studyLoaderName)
		{
			_studiesToOpen.Add(new LoadStudyArgs(studyInstanceUid, server, studyLoaderName));
		}

		/// <summary>
		/// Creates the <see cref="ImageViewerComponent"/>, loads the specified studies,
		/// and launches the <see cref="ImageViewerComponent"/>.
		/// </summary>
		public ImageViewerComponent OpenStudies()
		{
			ImageViewerComponent viewer = null;

			BlockingOperation.Run(delegate { viewer = LoadAndOpenStudies(); });

			return viewer;
		}

		#endregion

		#region Private

		private ImageViewerComponent LoadAndOpenStudies()
		{
			CodeClock codeClock = new CodeClock();
			codeClock.Start();

			ImageViewerComponent viewer = CreateViewer(LoadPriors);

			try
			{
				viewer.LoadStudies(_studiesToOpen);
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			if (!AnySopsLoaded(viewer) && !AllowEmptyViewer)
			{
				viewer.Dispose();
				return null;
			}

			LaunchImageViewerArgs args = new LaunchImageViewerArgs(WindowBehaviour);
			args.Title = Title;
			ImageViewerComponent.Launch(viewer, args);

			codeClock.Stop();
			string message = String.Format("TTFI: {0}", codeClock);
			Platform.Log(LogLevel.Debug, message);

			return viewer;
		}

		#endregion
		#endregion

		#region Static Helpers

		#region Public

		//TODO (later): create a class called OpenFilesHelper

		/// <summary>
		/// Launches a new <see cref="ImageViewerComponent"/> with the specified local files.
		/// </summary>
		public static IImageViewer OpenFiles(string[] localFileList, WindowBehaviour windowBehaviour)
		{
			Platform.CheckForNullReference(localFileList, "localFileList");
			if (localFileList.Length == 0)
				throw new ArgumentException("localFileList array cannot be empty.");

			ImageViewerComponent viewer = OpenFiles(localFileList);
			if (viewer != null)
				ImageViewerComponent.Launch(viewer, new LaunchImageViewerArgs(windowBehaviour));

			return viewer;
		}

		/// <summary>
		/// Launches a new <see cref="ImageViewerComponent"/> with the specified studies.
		/// </summary>
		/// <remarks>
		/// <para>This method has been deprecated and will be removed in the future. Use an instance of OpenStudyHelper instead.</para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version.  Please use an instance of OpenStudyHelper instead.")]
		public static IImageViewer OpenStudies(string studyLoaderName, string[] studyInstanceUids, WindowBehaviour windowBehaviour)
		{
			OpenStudyHelper helper = new OpenStudyHelper();
			helper.WindowBehaviour = windowBehaviour;
			foreach (string studyInstanceUid in studyInstanceUids)
				helper.AddStudy(studyInstanceUid, null, studyLoaderName);

			return helper.OpenStudies();
		}

		/// <summary>
		/// Launches a new <see cref="ImageViewerComponent"/> with the specified studies.
		/// </summary>
		/// <remarks>
		/// <para>This method has been deprecated and will be removed in the future. Use an instance of OpenStudyHelper instead.</para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version.  Please use an instance of OpenStudyHelper instead.")]
		public static IImageViewer OpenStudies(OpenStudyArgs openStudyArgs)
		{
			OpenStudyHelper helper = new OpenStudyHelper();
			helper.WindowBehaviour = openStudyArgs.WindowBehaviour;
			foreach (string studyInstanceUid in openStudyArgs.StudyInstanceUids)
				helper.AddStudy(studyInstanceUid, openStudyArgs.Server, openStudyArgs.StudyLoaderName);

			return helper.OpenStudies();
		}

		#endregion 

		#region Private

		private static ImageViewerComponent OpenFiles(string[] localFileList)
		{
			//don't find priors for files loaded off the local disk.
			ImageViewerComponent imageViewer = CreateViewer(false);

			bool cancelled = false;

			try
			{
				imageViewer.LoadImages(localFileList, Application.ActiveDesktopWindow, out cancelled);
			}
			catch(Exception e)
			{
				if (!cancelled) // silence any exceptions if the operation was cancelled
					ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			if (cancelled || !AnySopsLoaded(imageViewer))
			{
				imageViewer.Dispose();
				imageViewer = null;
			}

			return imageViewer;
		}

		private static ImageViewerComponent CreateViewer(bool loadPriors)
		{
			if (loadPriors)
				return new ImageViewerComponent(LayoutManagerCreationParameters.Extended);
			else
				return new ImageViewerComponent(LayoutManagerCreationParameters.Extended, PriorStudyFinder.Null);
		}

		private static bool AnySopsLoaded(IImageViewer imageViewer)
		{
			foreach (Patient patient in imageViewer.StudyTree.Patients)
			{
				foreach (Study study in patient.Studies)
				{
					foreach (Series series in study.Series)
					{
						foreach (Sop sop in series.Sops)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		#endregion
		#endregion
	}
}
