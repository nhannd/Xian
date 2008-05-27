using System;
using System.Collections;
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

	[ExtensionPoint]
	public class FolderExplorerGroupToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IFolderExplorerGroupToolContext : IToolContext
	{
		IEnumerable FolderSystems { get; }
		IFolderSystem SelectedFolderSystem { get; }

		IEnumerable Folders { get; }
		IFolder SelectedFolder { get; }

		IDesktopWindow DesktopWindow { get; }
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

		#region IFolderExplorerGroupToolContext implementation

		class FolderExplorerGroupToolContext : ToolContext, IFolderExplorerGroupToolContext
		{
			private readonly FolderExplorerGroupComponent _component;

			public FolderExplorerGroupToolContext(FolderExplorerGroupComponent component)
			{
				_component = component;
			}

			#region IFolderExplorerGroupToolContext Members

			public IEnumerable FolderSystems
			{
				get { return _component._folderExplorerComponents.Values; }
			}

			public IFolderSystem SelectedFolderSystem
			{
				get { return _component.SelectedFolderSystem; }
			}

			public IEnumerable Folders
			{
				get { return _component.SelectedFolderSystem.Folders; }
			}

			public IFolder SelectedFolder
			{
				get { return _component.SelectedFolder; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			#endregion
		}

		#endregion

		#region IFolderExplorerToolContext implementation

		class FolderExplorerToolContext : ToolContext, IFolderExplorerToolContext
		{
			private readonly FolderExplorerGroupComponent _component;

			public FolderExplorerToolContext(FolderExplorerGroupComponent component)
			{
				_component = component;
			}

			#region IFolderExplorerToolContext Members

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IFolder SelectedFolder
			{
				get { return _component.SelectedFolder; }
				set { _component.SelectedFolder = value; }
			}

			public ISelection SelectedItems
			{
				get { return _component._contentComponent.SelectedItems; }
			}

			#endregion
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

		private readonly ToolSet _toolSet;

		/// <summary>
		/// Constructor
		/// </summary>
		public FolderExplorerGroupComponent(IExtensionPoint folderExplorerExtensionPoint, FolderContentsComponent contentComponent)
		{
			_contentComponent = contentComponent;
			_stackTabComponent = new StackTabComponentContainer(
				HomePageSettings.Default.ShowMultipleFolderSystems ? StackStyle.ShowMultiple : StackStyle.ShowOneOnly,
				HomePageSettings.Default.OpenAllFolderSystemsInitially);
			_stackTabComponent.CurrentPageChanged += OnCurrentPageChanged;

			_folderExplorerComponents = new Dictionary<IFolderSystem, FolderExplorerComponent>();

			// Find all the folder systems
			ToolSet folderExplorers = new ToolSet(folderExplorerExtensionPoint, new FolderExplorerToolContext(this));
			List<IFolderSystem> folderSystems = CollectionUtils.Map<ITool, IFolderSystem, List<IFolderSystem>>(folderExplorers.Tools,
				delegate(ITool tool)
				{
					FolderExplorerToolBase folderExplorerTool = (FolderExplorerToolBase)tool;
					return folderExplorerTool.FolderSystem;
				});

			// Order the Folder Systems
			folderSystems = FolderExplorerComponentSettings.Default.OrderFolderSystems(folderSystems);

			// create a folder explorer component and a tab page for each folder system
			CollectionUtils.ForEach(folderSystems,
				delegate(IFolderSystem folderSystem)
				{
					FolderExplorerComponent component = new FolderExplorerComponent(folderSystem);
					component.SelectedFolderChanged += OnSelectedFolderChanged;

					_folderExplorerComponents.Add(folderSystem, component);

					StackTabPage thisPage = new StackTabPage(
						folderSystem.Title,
						component,
						string.Empty,
						folderSystem.Title,
						string.Empty,
						folderSystem.TitleIcon,
						folderSystem.ResourceResolver);

					_stackTabComponent.Pages.Add(thisPage);

					folderSystem.TextChanged += delegate { thisPage.SetTitle(string.Empty, folderSystem.Title, string.Empty); };
					folderSystem.IconChanged += delegate { thisPage.IconSet = folderSystem.TitleIcon; };
				});

			// Find all the folder explorer group tools
			_toolSet = new ToolSet(new FolderExplorerGroupToolExtensionPoint(), new FolderExplorerGroupToolContext(this));
		}

		#region Application Overrides

		public override void Start()
		{
			base.Start();

			CollectionUtils.ForEach(_folderExplorerComponents.Keys,
				delegate(IFolderSystem folderSystem)
				{
					DocumentManager.RegisterFolderSystem(folderSystem);
				});

			_stackTabComponentContainerHost = new ChildComponentHost(this.Host, _stackTabComponent);
			_stackTabComponentContainerHost.StartComponent();

		}

		public override void Stop()
		{
			_stackTabComponentContainerHost.StopComponent();

			CollectionUtils.ForEach(_folderExplorerComponents.Keys,
				delegate(IFolderSystem folderSystem)
				{
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

		public ActionModelRoot ContextMenuModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-group-contextmenu", _toolSet.Actions); }
		}

		public ActionModelRoot ToolbarModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-group-toolbar", _toolSet.Actions); }
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
					_selectedFolderExplorer = value;

					EventsHelper.Fire(_selectedFolderSystemChanged, this, EventArgs.Empty);

					IFolder newSelectedFolder = (IFolder)_selectedFolderExplorer.SelectedFolder.Item;
					if (newSelectedFolder == null)
						newSelectedFolder = (IFolder)CollectionUtils.FirstElement(_selectedFolderExplorer.FolderTree.Items);

					this.SelectedFolder = newSelectedFolder;
				}
			}
		}

		public IFolderSystem SelectedFolderSystem
		{
			get { return _selectedFolderExplorer == null ? null : _selectedFolderExplorer.FolderSystem; }
		}

		public IFolder SelectedFolder
		{
			get  { return _selectedFolder; }
			set
			{
				if (_selectedFolder != value)
				{
					_selectedFolder = value;

					if (_selectedFolderExplorer != null)
						_selectedFolderExplorer.SelectedFolder = new Selection(value);

					EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool SearchEnabled
		{
			get { return this.SelectedFolderSystem is ISearchDataHandler; }
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

			IFolder newSelectedFolder = (IFolder)selectedFolderExplorer.SelectedFolder.Item;
			if (newSelectedFolder == null)
				newSelectedFolder = (IFolder)CollectionUtils.FirstElement(_selectedFolderExplorer.FolderTree.Items);

			this.SelectedFolder = newSelectedFolder;
		}

		#endregion

		public void Search(SearchData searchData)
		{
			if (this.SearchEnabled)
				((ISearchDataHandler)this.SelectedFolderSystem).SearchData = searchData;
		}

	}
}
