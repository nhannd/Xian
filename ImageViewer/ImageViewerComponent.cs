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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines an extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public sealed class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

	/// <summary>
	/// An <see cref="ExtensionPoint"/> for views on to the 
	/// <see cref="ImageViewerComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ImageViewerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// An <see cref="ExtensionPoint"/> for image viewer tools.
	/// </summary>
	/// <remarks>
	/// This <see cref="ExtensionPoint"/> is the means by which tools
	/// are added to the <see cref="ImageViewerComponent"/>.
	/// </remarks>
	[ExtensionPoint]
	public sealed class ImageViewerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// Specifies how the <see cref="ImageViewerComponent"/>'s <see cref="ILayoutManager"/>
	/// should be created.
	/// </summary>
	public enum LayoutManagerCreationParameters
	{
		/// <summary>
		/// Use a simple layout manager that initializes the layout strictly
		/// based on the number of <see cref="IDisplaySet"/>s available.
		/// </summary>
		Simple,

		/// <summary>
		/// Use the <see cref="LayoutManagerExtensionPoint"/> to create
		/// the <see cref="ImageViewerComponent"/>'s <see cref="ILayoutManager"/>.
		/// </summary>
		Extended
	}

	/// <summary>
	/// An <see cref="ApplicationComponent"/> capable of image display.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> (IVC) is an <see cref="ApplicationComponent"/> 
	/// whose purpose is to display images and to allow users to interact with those images.
	/// It provides a number of core services, such as image loading, layout, selection,
	/// rendering, etc.  An API and a number of extension points allow plugin developers
	/// to extend the functionality of the IVC.
	/// </remarks>
	[AssociateView(typeof(ImageViewerComponentViewExtensionPoint))]
	public class ImageViewerComponent : ApplicationComponent, IImageViewer, IContextMenuProvider
	{
		internal class ImageViewerToolContext : ToolContext, IImageViewerToolContext
		{
			private ImageViewerComponent _component;

			internal ImageViewerToolContext(ImageViewerComponent component)
			{
				_component = component;
			}

			#region IImageViewerToolContext Members

			public IImageViewer Viewer
			{
				get { return _component; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			#endregion
		}

		#region Private fields

		private StudyTree _studyTree;
		private ILogicalWorkspace _logicalWorkspace;
		private IPhysicalWorkspace _physicalWorkspace;
		private EventBroker _eventBroker;
		private ViewerShortcutManager _shortcutManager;
		private ToolSet _toolSet;
		private ILayoutManager _layoutManager;

		private static readonly StudyFinderMap _studyFinders = new StudyFinderMap();
		private readonly StudyLoaderMap _studyLoaders = new StudyLoaderMap();

		private event EventHandler _closingEvent;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="ImageViewerComponent"/>.
		/// </summary>
		/// This constructor creates an <see cref="ImageViewerComponent"/>
		/// that automatically lays out images according to how many 
		/// <see cref="DisplaySet"/>s there are.  Use this constructor in 
		/// simple display scenarios where you don't require control over 
		/// how images are initially laid out.
		/// If you do require control over the initial layout, use
		/// <see cref="ImageViewerComponent(ILayoutManager)"/> instead.
		public ImageViewerComponent(): this(LayoutManagerCreationParameters.Simple)
		{
		}

		/// <summary>
		/// Instantiates a new instance of <see cref="ImageViewerComponent"/>
		/// taking a <see cref="LayoutManagerCreationParameters"/> to determine
		/// how the <see cref="LayoutManager"/> should be created.
		/// </summary>
		/// <remarks>
		///	If <paramref name="creationParameters"/> is <see cref="LayoutManagerCreationParameters.Extended"/>,
		/// then the <see cref="LayoutManagerExtensionPoint"/> is used to create the <see cref="LayoutManager"/>.  If
		/// no extension exists, a simple layout manager is used.
		/// </remarks>
		public ImageViewerComponent(LayoutManagerCreationParameters creationParameters)
			: this(CreateLayoutManager(creationParameters))
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageViewerComponent"/>
		/// with the specified <see cref="ILayoutManager"/>.
		/// </summary>
		public ImageViewerComponent(ILayoutManager layoutManager)
		{
			Platform.CheckForNullReference(layoutManager, "layoutManager");
			_layoutManager = layoutManager;
			_layoutManager.SetImageViewer(this);
		}

		private static ILayoutManager CreateLayoutManager(LayoutManagerCreationParameters creationParameters)
		{
			ILayoutManager layoutManager = null;
			
			if (creationParameters == LayoutManagerCreationParameters.Extended)
			{
				try
				{
					layoutManager = new LayoutManagerExtensionPoint().CreateExtension() as ILayoutManager;
				}
				catch(NotSupportedException e)
				{
					Platform.Log(LogLevel.Info, e);
				}
			}

			return layoutManager ?? new LayoutManager();
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new ImageViewerToolExtensionPoint(), new ImageViewerToolContext(this));

			_shortcutManager = new ViewerShortcutManager();

			foreach (ITool tool in _toolSet.Tools)
				_shortcutManager.RegisterImageViewerTool(tool);
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Stop()
		{
			EventsHelper.Fire(_closingEvent, this, EventArgs.Empty);

			base.Stop();
		}

		#region Public Properties

		/// <summary>
		/// Gets the host <see cref="IDesktopWindow"/>.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get { return this.Host.DesktopWindow; }
		}

		/// <summary>
		/// Gets actions associated with the <see cref="ImageViewerComponent"/>'s
		/// <see cref="ToolSet"/>.
		/// </summary>
		public override IActionSet ExportedActions
		{
			get
			{
				// we should technically only export the actions that target the global menus
				// or toolbars, but it doesn't matter - the desktop will sort it out
				return _toolSet.Actions;
			}
		}

		/// <summary>
		/// Gets the command history.
		/// </summary>
		/// <remarks>
		/// Each <see cref="ImageViewerComponent"/> (IVC) maintains its own 
		/// <see cref="CommandHistory"/>.
		/// </remarks>
		public CommandHistory CommandHistory
		{
			get { return this.Host.CommandHistory; }
		}

		/// <summary>
		/// Gets the <see cref="IPhysicalWorkspace"/>.
		/// </summary>
		public IPhysicalWorkspace PhysicalWorkspace
		{
			get
			{
				if (_physicalWorkspace == null)
					_physicalWorkspace = new PhysicalWorkspace(this);

				return _physicalWorkspace;
			}
		}

		/// <summary>
		/// Gets the <see cref="ILogicalWorkspace"/>.
		/// </summary>
		public ILogicalWorkspace LogicalWorkspace
		{
			get
			{
				if (_logicalWorkspace == null)
					_logicalWorkspace = new LogicalWorkspace(this);

				return _logicalWorkspace;
			}
		}

		/// <summary>
		/// Gets the <see cref="EventBroker"/>.
		/// </summary>
		public EventBroker EventBroker
		{
			get
			{
				if (_eventBroker == null)
					_eventBroker = new EventBroker();

				return _eventBroker;
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="IImageBox"/>
		/// </summary>
		/// <value>The currently selected <see cref="IImageBox"/>, or <b>null</b> if
		/// no <see cref="IImageBox"/> is currently selected.</value>
		public IImageBox SelectedImageBox
		{
			get
			{
				if (this.PhysicalWorkspace == null)
					return null;
				else
					return this.PhysicalWorkspace.SelectedImageBox;
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="Tile"/>
		/// </summary>
		/// <value>The currently selected <see cref="ITile"/>, or <b>null</b> if
		/// no <see cref="ITile"/> is currently selected.</value>
		public ITile SelectedTile
		{
			get
			{
				if (this.SelectedImageBox == null)
					return null;
				else
					return this.SelectedImageBox.SelectedTile;
			}
		}

		/// <summary>
		/// Gets the currently selected <see cref="PresentationImage"/>
		/// </summary>
		/// <value>The currently selected <see cref="IPresentationImage"/>, or <b>null</b> if
		/// no <see cref="IPresentationImage"/> is currently selected.</value>
		public IPresentationImage SelectedPresentationImage
		{
			get
			{
				if (this.SelectedTile == null)
					return null;
				else
					return this.SelectedTile.PresentationImage;
			}
		}

		/// <summary>
		/// Gets the <see cref="IViewerShortcutManager"/>.
		/// </summary>
		/// <remarks>
		/// This property is intended for internal framework use only.
		/// </remarks>
		public IViewerShortcutManager ShortcutManager
		{
			get
			{
				return _shortcutManager;
			}
		}

		/// <summary>
		/// Gets the <see cref="StudyTree"/>.
		/// </summary>
		/// <remarks>
		/// Although each <see cref="ImageViewerComponent"/> (IVC) maintains its own
		/// <see cref="StudyTree"/>, actual <see cref="ImageSop"/> objects are shared
		/// between IVCs for efficient memory usage.
		/// </remarks>
		public StudyTree StudyTree
		{
			get
			{
				if (_studyTree == null)
					_studyTree = new StudyTree();

				return _studyTree;
			}
		}


		/// <summary>
		/// Gets a string containing the patients currently loaded in this
		/// <see cref="ImageViewerComponent"/>.
		/// </summary>
		public string PatientsLoadedLabel
		{
			get
			{
				return StringUtilities.Combine<Patient>(this.StudyTree.Patients, String.Format(" {0} ", SR.SeparatorPatientsLoaded),
					delegate(Patient patient)
					{
						PersonName name = patient.PatientsName;
						string formattedName = name.FormattedName;
						return StringUtilities.Combine<string>(new string[] { formattedName, patient.PatientId } , String.Format(" {0} ", SR.SeparatorPatientDescription));
					});
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the <see cref="ImageViewerComponent"/> is about to close.
		/// </summary>
		public event EventHandler Closing
		{
			add { _closingEvent += value; }
			remove { _closingEvent -= value; }
		}

		#endregion

		#region Protected properties

		/// <summary>
		/// Gets the <see cref="ToolSet"/>.
		/// </summary>
		protected ToolSet ToolSet
		{
			get { return _toolSet; }
		}

		#endregion

		#region Private/internal properties

		private static StudyFinderMap StudyFinders
		{
			get { return _studyFinders; }
		}

		private StudyLoaderMap StudyLoaders
		{
			get { return _studyLoaders; }
		}

		private ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-contextmenu", _toolSet.Actions);
			}
		}

		private ActionModelNode KeyboardModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "imageviewer-keyboard", _toolSet.Actions);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ILayoutManager"/>
		/// </summary>
		protected ILayoutManager LayoutManager
		{
			get { return _layoutManager; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Queries for studies matching a specified set of query parameters.
		/// </summary>
		/// <param name="queryParameters">The search criteria.</param>
		/// <param name="targetServer">The server to query. Can be null
		/// if the <paramref name="studyFinderName"/> does not support the querying
		/// of different servers.</param>
		/// <param name="studyFinderName">The name of the <see cref="IStudyFinder"/> to use, which is specified
		/// by <see cref="IStudyFinder.Name"/>.</param>
		/// <returns></returns>
		public static StudyItemList FindStudy(
			QueryParameters queryParameters,
			object targetServer,
			string studyFinderName)
		{
			Platform.CheckForNullReference(queryParameters, "queryParameters");
			Platform.CheckForEmptyString(studyFinderName, "studyFinderName");

			return StudyFinders[studyFinderName].Query(queryParameters, targetServer);
		}

		/// <summary>
		/// Loads a study with a specific Study Instance UID from a specific source.
		/// </summary>
		/// <param name="studyInstanceUID">The Study Instance UID of the study to be loaded.</param>
		/// <param name="studyLoaderName">The name of the <see cref="IStudyLoader"/> to use, which is specified
		/// by <see cref="IStudyLoader.Name"/>.</param>
		/// <remarks>
		/// <para>This method has been deprecated and will be removed in the future. Use the <see cref="LoadStudy(LoadStudyArgs)"/> overload instead.</para>
		/// <para>After this method is executed, the image viewer's <see cref="StudyTree"/>
		/// will be populated with the appropriate <see cref="Study"/>, <see cref="Series"/> 
		/// and <see cref="ImageSop"/> objects.</para>
		/// <para>By default, the Framework provides an implementation of 
		/// <see cref="IStudyLoader"/> called <b>LocalDataStoreStudyLoader</b> which loads
		/// studies from the local database.  If you have implemented your own 
		/// <see cref="IStudyLoader"/> and want to load a study using that implementation,
		/// just pass in the name provided by <see cref="IStudyLoader.Name"/> as the source.</para>
		/// </remarks>
		/// <exception cref="OpenStudyException">The study could not be opened.</exception>
		/// <seealso cref="LoadStudy(LoadStudyArgs)"/>
		[Obsolete("This method has been deprecated and will be removed in the future. Use the LoadStudy(LoadStudyArgs) overload instead.")]
		public void LoadStudy(string studyInstanceUID, string studyLoaderName)
		{
			LoadStudy(new LoadStudyArgs(studyInstanceUID, null, studyLoaderName));
		}

		/// <summary>
		/// Loads a study using the specified parameters.
		/// </summary>
		/// <remarks>After this method is executed, the image viewer's <see cref="StudyTree"/>
		/// will be populated with the appropriate <see cref="Study"/>, <see cref="Series"/> 
		/// and <see cref="ImageSop"/> objects.
		/// 
		/// By default, the Framework provides an implementation of 
		/// <see cref="IStudyLoader"/> called <b>LocalDataStoreStudyLoader</b> which loads
		/// studies from the local database.  If you have implemented your own 
		/// <see cref="IStudyLoader"/> and want to load a study using that implementation,
		/// just pass in the name provided by <see cref="IStudyLoader.Name"/> as the source.
		/// </remarks>
		/// <param name="loadStudyArgs">A <see cref="LoadStudyArgs"/> object containing information about the study to be loaded.</param>
		/// <exception cref="OpenStudyException">The study could not be opened.</exception>
		public void LoadStudy(LoadStudyArgs loadStudyArgs)
		{
			IStudyLoader studyLoader = this.StudyLoaders[loadStudyArgs.StudyLoaderName];
			int totalImages = 0;

			try
			{
				totalImages = studyLoader.Start(new StudyLoaderArgs(loadStudyArgs.StudyInstanceUid, loadStudyArgs.Server));
			}
			catch (Exception e)
			{
				OpenStudyException ex = new OpenStudyException("Failed to load any of the requested images.", e);
				throw ex;
			}

			int numberOfImages = 0;
			int failedImages = 0;

			while (true)
			{
				ImageSop image = studyLoader.LoadNextImage();
				if (image == null)
					break;

				try
				{
					this.StudyTree.AddImage(image);
				}
				catch (Exception e)
				{
					failedImages++;
					Platform.Log(LogLevel.Error, e);
				}

				numberOfImages++;
			}

			int successfulImages = numberOfImages - failedImages;

			// Only bother to tell someone if at least one image loaded
			if (successfulImages > 0)
			{
				this.EventBroker.OnStudyLoaded(new ItemEventArgs<Study>(this.StudyTree.GetStudy(loadStudyArgs.StudyInstanceUid)));
			}

			VerifyLoad(numberOfImages, failedImages);

			if (studyLoader.PrefetchingStrategy != null)
				studyLoader.PrefetchingStrategy.Start(this);
		}

		/// <summary>
		/// Loads images from the specified file paths.
		/// </summary>
		/// <param name="files">An array of file paths.</param>
		/// <exception cref="OpenStudyException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		public void LoadImages(string[] files)
		{
			// Dummy variable; this overload can't be cancelled
			bool cancelled;
			LoadImages(files, null, out cancelled);
		}

		/// <summary>
		/// Loads images from the specified file paths and displays a progress bar.
		/// </summary>
		/// <param name="files">A list of file paths.</param>
		/// <param name="desktop">The desktop window.  This is necessary for
		/// a progress bar to be shown.  If <b>null</b>, no progress bar
		/// will be shown.</param>
		/// <param name="cancelled">A value that indicates whether the operation
		/// was cancelled.</param>
		/// <exception cref="OpenStudyException">One or more images could not be opened.</exception>
		/// <exception cref="ArgumentNullException">A parameter is <b>null</b>.</exception>
		public void LoadImages(string[] files, IDesktopWindow desktop, out bool cancelled)
		{
			Platform.CheckForNullReference(files, "files");

			LocalImageLoader loader = new LocalImageLoader(this);
			loader.Load(files, desktop, out cancelled);

			VerifyLoad(loader.TotalImages, loader.FailedImages);
		}

		/// <summary>
		/// Lays out the images in the <see cref="ImageViewerComponent"/> using
		/// the current layout manager.
		/// </summary>
		public void Layout()
		{
			this.LayoutManager.Layout();
		}

		/// <summary>
		/// Tries to get a reference to an <see cref="IImageViewer"/> hosted by a workspace.
		/// </summary>
		/// <returns>The active <see cref="IImageViewer"/> or <b>null</b> if 
		/// the workspace does not host an <see cref="IImageViewer"/>.</returns>
		public static IImageViewer GetAsImageViewer(Workspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			// return the hosted IImageViewer, or null if the hosted component is not an IImageViewer
			return workspace.Component as IImageViewer;
		}

		/// <summary>
		/// Launches an <see cref="ImageViewerComponent"/> in the active desktop window.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Subsequent <see cref="ImageViewerComponent"/>s will also be launched in workspaces in
		/// the same window.
		/// </remarks>
		public static void LaunchInActiveWindow(ImageViewerComponent imageViewer)
		{
			LaunchInWindow(imageViewer, Application.ActiveDesktopWindow);
		}

		/// <summary>
		/// Launches an <see cref="ImageViewerComponent"/> in a separate desktop window
		/// devoted strictly to image display.
		/// </summary>
		/// <param name="imageViewer"></param>
		/// <remarks>
		/// Subsequent <see cref="ImageViewerComponent"/>s will also be launched in workspaces in
		/// the same window.
		/// </remarks>
		public static void LaunchInSeparateWindow(ImageViewerComponent imageViewer)
		{
			IDesktopWindow window;
			string imageViewerWindow = "ImageViewer";

			// If an image viewer desktop window already exists, use it
			if (Application.DesktopWindows.Contains(imageViewerWindow))
			{
				window = Application.DesktopWindows[imageViewerWindow];
			}
			// If not, create one
			else
			{
				DesktopWindowCreationArgs args = new DesktopWindowCreationArgs("", imageViewerWindow);
				window = Application.DesktopWindows.AddNew(args);
			}

			LaunchInWindow(imageViewer, window);
		}

		private static void LaunchInWindow(ImageViewerComponent imageViewer, IDesktopWindow desktopWindow)
		{
			IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
				desktopWindow,
				imageViewer,
				imageViewer.PatientsLoadedLabel);

			workspace.Closed += delegate(object sender, ClosedEventArgs e)
			                    	{
			                    		imageViewer.Dispose();
			                    	};
			imageViewer.Layout();
			imageViewer.PhysicalWorkspace.SelectDefaultImageBox();
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by this <see cref="ImageViewerComponent"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				StopPrefetching();

				if (_physicalWorkspace != null)
				{
					_physicalWorkspace.Dispose();
					_physicalWorkspace = null;
				}

				if (_logicalWorkspace != null)
				{
					_logicalWorkspace.Dispose();
					_logicalWorkspace = null;
				}

				if (_toolSet != null)
				{
					_toolSet.Dispose();
					_toolSet = null;
				}

				if (_studyTree != null)
				{
					_studyTree.Dispose();
					_studyTree = null;
				}

				if (_layoutManager != null)
				{
					_layoutManager.Dispose();
					_layoutManager = null;
				}
			}
		}

		#endregion

		#region Private methods

		private void VerifyLoad(int totalImages, int failedImages)
		{
			if (failedImages == 0)
				return;

			string message = String.Format("{0} of {1} images have failed to load.", failedImages, totalImages);
			OpenStudyException ex = new OpenStudyException(message);
			ex.TotalImages = totalImages;
			ex.FailedImages = failedImages;
			throw ex;
		}

		private void StopPrefetching()
		{
			foreach (IStudyLoader loader in _studyLoaders)
			{
				if (loader.PrefetchingStrategy != null)
					loader.PrefetchingStrategy.Stop();
			}
		}

		#endregion

		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu model for the <see cref="ImageViewerComponent"/>.
		/// </summary>
		/// <param name="mouseInformation"></param>
		/// <returns>An <see cref="ActionModelNode"/></returns>
		/// <remarks>
		/// This method is used by the tile's view class to generate the 
		/// <see cref="ImageViewerComponent"/> context menu when a user right-clicks
		/// on a tile.  It is unlikely that you will ever need to use this method.
		/// </remarks>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			return this.ContextMenuModel;
		}

		#endregion
	}
}
