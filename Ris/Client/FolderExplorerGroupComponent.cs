using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="FolderExplorerGroupComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class FolderExplorerGroupComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FolderExplorerGroupComponent class
	/// </summary>
	[AssociateView(typeof(FolderExplorerGroupComponentViewExtensionPoint))]
	public class FolderExplorerGroupComponent : ApplicationComponent, ISearchDataHandler
	{
		#region ISearchDataHandler implementation

		public SearchData SearchData
		{
			set { Search(value); }
		}

		#endregion

		#region IFolderSystemContext implementation

		class FolderSystemContext : ToolContext, IFolderSystemContext
		{
			private readonly FolderExplorerGroupComponent _owner;
			private readonly FolderExplorerComponent _explorerComponent;
			private readonly FolderContentsComponent _contentsComponent;


			public FolderSystemContext(FolderExplorerGroupComponent owner, FolderExplorerComponent explorerComponent, FolderContentsComponent contentsComponent)
			{
				_owner = owner;
				_explorerComponent = explorerComponent;
				_contentsComponent = contentsComponent;
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Host.DesktopWindow; }
			}

			public IFolder SelectedFolder
			{
				get { return _explorerComponent.SelectedFolder; }
				set { _explorerComponent.SelectedFolder = value; }
			}

			public event EventHandler SelectedFolderChanged
			{
				add { _explorerComponent.SelectedFolderChanged += value; }
				remove { _explorerComponent.SelectedFolderChanged -= value; }
			}

			public ISelection SelectedItems
			{
				get { return _contentsComponent.SelectedItems; }
			}

			public event EventHandler SelectedItemsChanged
			{
				add { _contentsComponent.SelectedItemsChanged += value; }
				remove { _contentsComponent.SelectedItemsChanged -= value; }
			}

			public event EventHandler SelectedItemDoubleClicked
			{
				add { _contentsComponent.SelectedItemDoubleClicked += value; }
				remove { _contentsComponent.SelectedItemDoubleClicked -= value; }
			}

			public void InvalidateFolders()
			{
				_explorerComponent.InvalidateFolders();
			}

			public void InvalidateFolders(Type folderClass)
			{
				_explorerComponent.InvalidateFolders(folderClass);
			}

			public void InvalidateFolder(IFolder folder)
			{
				_explorerComponent.InvalidateFolder(folder);
			}
		}

		#endregion

		private readonly StackTabComponentContainer _stackTabComponent;
		private ChildComponentHost _stackTabComponentContainerHost;

		private readonly FolderContentsComponent _contentComponent;
		private readonly Dictionary<IFolderSystem, FolderExplorerComponent> _folderExplorerComponents;
		private FolderExplorerComponent _selectedFolderExplorer;
		private IFolder _selectedFolder;

		private event EventHandler _selectedFolderSystemChanged;
		private event EventHandler _selectedFolderChanged;

		/// <summary>
		/// Constructor
		/// </summary>
		public FolderExplorerGroupComponent(ExtensionPoint<IFolderSystem> folderSystemExtensionPoint, FolderContentsComponent contentComponent)
		{
			_contentComponent = contentComponent;
			_stackTabComponent = new StackTabComponentContainer(StackStyle.ShowOneOnly, false);

			_folderExplorerComponents = new Dictionary<IFolderSystem, FolderExplorerComponent>();

			// Find all the folder systems
			List<IFolderSystem> folderSystems =
				CollectionUtils.Cast<IFolderSystem>(folderSystemExtensionPoint.CreateExtensions());

			// Order the Folder Systems
			List<IFolderSystem> remainder;
			FolderExplorerComponentSettings.Default.OrderFolderSystems(folderSystems, out folderSystems, out remainder);

			// add the remainder to the end of the ordered list
			folderSystems.AddRange(remainder);

			// create a folder explorer component and a tab page for each folder system
			CollectionUtils.ForEach(folderSystems,
				delegate(IFolderSystem folderSystem)
				{
					FolderExplorerComponent explorer = new FolderExplorerComponent(folderSystem);
					folderSystem.SetContext(new FolderSystemContext(this, explorer, _contentComponent));
					_folderExplorerComponents.Add(folderSystem, explorer);

					StackTabPage thisPage = new StackTabPage(
						folderSystem.Title,
						explorer,
						string.Empty,
						folderSystem.Title,
						string.Empty,
						folderSystem.TitleIcon,
						folderSystem.ResourceResolver);

					_stackTabComponent.Pages.Add(thisPage);

					folderSystem.TitleChanged += delegate { thisPage.SetTitle(string.Empty, folderSystem.Title, string.Empty); };
					folderSystem.TitleIconChanged += delegate { thisPage.IconSet = folderSystem.TitleIcon; };
				});
		}

		#region Application Overrides

		public override void Start()
		{
			// Subscribe to page changed event before starting component, so that when component start the event will fire
			// for the initial page that open
			_stackTabComponent.CurrentPageChanged += OnCurrentPageChanged;
			_stackTabComponentContainerHost = new ChildComponentHost(this.Host, _stackTabComponent);
			_stackTabComponentContainerHost.StartComponent();

			CollectionUtils.ForEach(_folderExplorerComponents.Keys,
				delegate(IFolderSystem folderSystem)
				{
					DocumentManager.RegisterFolderSystem(folderSystem);
					_folderExplorerComponents[folderSystem].SelectedFolderChanged += OnSelectedFolderChanged;
				});

			base.Start();
		}

		public override void Stop()
		{
			_stackTabComponent.CurrentPageChanged -= OnCurrentPageChanged;
			_stackTabComponentContainerHost.StopComponent();

			CollectionUtils.ForEach(_folderExplorerComponents.Keys,
				delegate(IFolderSystem folderSystem)
				{
					_folderExplorerComponents[folderSystem].SelectedFolderChanged -= OnSelectedFolderChanged;
					DocumentManager.UnregisterFolderSystem(folderSystem);
				});

			base.Stop();
		}

		#endregion

		#region Properties

		public ApplicationComponentHost StackTabComponentContainerHost
		{
			get { return _stackTabComponentContainerHost; }
		}

		public ActionModelNode ToolbarModel
		{
			get { return _selectedFolderExplorer.FoldersToolbarModel; }
		}

		public event EventHandler SelectedFolderSystemChanged
		{
			add { _selectedFolderSystemChanged += value; }
			remove { _selectedFolderSystemChanged -= value; }
		}

		public event EventHandler SelectedFolderChanged
		{
			add { _selectedFolderChanged += value; }
			remove { _selectedFolderChanged -= value; }
		}

		public event EventHandler SearchEnabledChanged
		{
			add { this.SelectedFolderSystemChanged += value; }
			remove { this.SelectedFolderSystemChanged -= value; }
		}

		public FolderExplorerComponent SelectedFolderExplorer
		{
			get { return _selectedFolderExplorer; }
			set
			{
				if (_selectedFolderExplorer != value)
				{
					// Must set the previous folder explorer folder selection to null before changing folder exploer
					this.SelectedFolder = null;

					_selectedFolderExplorer = value;
					EventsHelper.Fire(_selectedFolderSystemChanged, this, EventArgs.Empty);
				}
			}
		}

		public IFolderSystem SelectedFolderSystem
		{
			get { return _selectedFolderExplorer == null ? null : _selectedFolderExplorer.FolderSystem; }
		}

		public IFolder SelectedFolder
		{
			get { return _selectedFolder; }
			set
			{
				if (_selectedFolder != value)
				{
					if (_selectedFolderExplorer != null)
						_selectedFolderExplorer.SelectedFolder = value;

					_selectedFolder = value;
					EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool SearchEnabled
		{
			get { return this.SelectedFolderSystem.SearchEnabled; }
		}

		public string SearchMessage
		{
			get { return SR.MessageSearchMessage; }
		}

		#endregion

		#region Event Handlers

		private void OnCurrentPageChanged(object sender, EventArgs e)
		{
			this.SelectedFolderExplorer = (FolderExplorerComponent)_stackTabComponent.CurrentPage.Component;
		}

		private void OnSelectedFolderChanged(object sender, EventArgs e)
		{
			FolderExplorerComponent selectedFolderExplorer = (FolderExplorerComponent)sender;
			this.SelectedFolder = selectedFolderExplorer.SelectedFolder;
		}

		#endregion

		public void Search(SearchData searchData)
		{
			if (this.SearchEnabled)
				this.SelectedFolderSystem.ExecuteSearch(searchData);
		}

	}
}
