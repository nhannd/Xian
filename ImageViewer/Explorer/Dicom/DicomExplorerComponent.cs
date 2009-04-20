using System;
using System.Collections.Generic;
using System.Security.Policy;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	internal class DicomExplorerComponent : SplitComponentContainer
	{
		private static readonly object _syncLock = new object();
		private static readonly List<DicomExplorerComponent> _activeComponents = new List<DicomExplorerComponent>();

		private ServerTreeComponent _serverTreeComponent;
		private StudyBrowserComponent _studyBrowserComponent;

		private DicomExplorerComponent(SplitPane pane1, SplitPane pane2)
			: base(pane1, pane2, Desktop.SplitOrientation.Horizontal)
		{
		}

		public ServerTreeComponent ServerTreeComponent
		{
			get { return _serverTreeComponent; }
		}

		public StudyBrowserComponent StudyBrowserComponent
		{
			get { return _studyBrowserComponent; }
		}

		public SearchPanelComponent SearchPanelComponent
		{
			get { return _studyBrowserComponent.SearchPanelComponent; }
		}

		public override void Start()
		{
			base.Start();

			lock (_syncLock)
			{
				_activeComponents.Add(this);
			}
		}

		public override void Stop()
		{
			lock (_syncLock)
			{
				_activeComponents.Remove(this);
			}

			base.Stop();
		}

		public static List<DicomExplorerComponent> GetActiveComponents()
		{
			lock (_syncLock)
			{
				return new List<DicomExplorerComponent>(_activeComponents);
			}
		}

		public static DicomExplorerComponent Create()
		{
			ServerTreeComponent serverTreeComponent = new ServerTreeComponent();
			serverTreeComponent.ShowLocalDataStoreNode = HasLocalDatastoreSupport();

			StudyBrowserComponent studyBrowserComponent = new StudyBrowserComponent();

			serverTreeComponent.SelectedServerChanged +=
				delegate { studyBrowserComponent.SelectServerGroup(serverTreeComponent.SelectedServers); };

			SearchPanelComponent searchPanel = new SearchPanelComponent(studyBrowserComponent);

			studyBrowserComponent.SelectServerGroup(serverTreeComponent.SelectedServers);

			try
			{
				//explicitly check and make sure we're querying local only.
				if (serverTreeComponent.ShowLocalDataStoreNode && serverTreeComponent.SelectedServers.IsLocalDatastore)
					studyBrowserComponent.Search();
			}
			catch (PolicyException)
			{
				//TODO: ignore this on startup or show message?
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Application.ActiveDesktopWindow);
			}

			SplitPane leftPane = new SplitPane(SR.TitleServerTreePane, serverTreeComponent, 0.25f);
			SplitPane rightPane = new SplitPane(SR.TitleStudyBrowserPane, studyBrowserComponent, 0.75f);

			SplitComponentContainer bottomContainer =
				new SplitComponentContainer(
				leftPane,
				rightPane,
				SplitOrientation.Vertical);

			SplitPane topPane = new SplitPane(SR.TitleSearchPanelPane, searchPanel, true);
			SplitPane bottomPane = new SplitPane(SR.TitleStudyNavigatorPane, bottomContainer, false);

			DicomExplorerComponent component = new DicomExplorerComponent(topPane, bottomPane);
			component._studyBrowserComponent = studyBrowserComponent;
			component._serverTreeComponent = serverTreeComponent;
			return component;
		}

		internal static bool HasLocalDatastoreSupport()
		{
			try
			{
				StudyFinderExtensionPoint finders = new StudyFinderExtensionPoint();
				return null != CollectionUtils.SelectFirst(finders.CreateExtensions(),
								delegate(object extension) { return ((IStudyFinder)extension).Name == "DICOM_LOCAL"; });
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Warn, "Local data store study finder not found.");
				return false;
			}
		}
	}
}
