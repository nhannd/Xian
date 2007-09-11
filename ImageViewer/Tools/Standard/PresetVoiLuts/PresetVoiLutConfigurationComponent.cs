using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	/// <summary>
	/// Extension point for views onto <see cref="PresetVoiLutConfigurationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PresetVoiLutConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// VoiLutConfigurationComponent class
	/// </summary>
	[AssociateView(typeof(PresetVoiLutConfigurationComponentViewExtensionPoint))]
	public sealed class PresetVoiLutConfigurationComponent : ConfigurationApplicationComponent
	{
		public sealed class PresetFactoryDescriptor : IEquatable<PresetFactoryDescriptor>, IComparable<PresetFactoryDescriptor>
		{
			internal readonly IPresetVoiLutApplicatorFactory Factory;

			internal PresetFactoryDescriptor(IPresetVoiLutApplicatorFactory factory)
			{
				Factory = factory;
			}

			public string Name
			{
				get { return Factory.Name; }
			}

			public string Description
			{
				get { return Factory.Description; }
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is PresetFactoryDescriptor)
					return this.Equals((PresetFactoryDescriptor) obj);

				return false;
			}

			public override string ToString()
			{
				return this.Factory.Description;
			}

			#region IEquatable<PresetFactoryDescriptor> Members

			public bool Equals(PresetFactoryDescriptor other)
			{
				return this.Factory == other.Factory;
			}

			#endregion

			#region IComparable<PresetFactoryDescriptor> Members

			public int CompareTo(PresetFactoryDescriptor other)
			{
				return this.Factory.Description.CompareTo(other.Factory.Description);
			}

			#endregion
		}

		private List<PresetFactoryDescriptor> _availableAddFactories;
		private PresetFactoryDescriptor _selectedAddFactory;

		private List<string> _availableModalities;
		private PresetVoiLutGroupCollection _presetVoiLutGroups;
		private PresetVoiLutGroup _selectedPresetVoiLutGroup;

		private ITable _voiLutPresets;
		private ISelection _selection;

		private SimpleActionModel _toolbarModel;
		private SimpleActionModel _contextMenuModel;

		/// <summary>
		/// Constructor
		/// </summary>
		public PresetVoiLutConfigurationComponent()
		{
		}

		public IList<PresetFactoryDescriptor> AvailableAddFactories
		{
			get 
			{
				return _availableAddFactories;
			}	
		}

		public PresetFactoryDescriptor SelectedAddFactory
		{
			get { return _selectedAddFactory; }	
			set
			{
				Platform.CheckForNullReference(value, "value");

				if (value.Equals(_selectedAddFactory))
					return;

				_selectedAddFactory = value;
				UpdateButtonStates();
				NotifyPropertyChanged("SelectedAddFactory");
			}
		}

		public IList<string> Modalities
		{
			get { return _availableModalities; }
		}

		public string SelectedModality
		{
			get { return _selectedPresetVoiLutGroup.Modality; }
			set
			{
				foreach(PresetVoiLutGroup group in _presetVoiLutGroups)
				{
					if (group.Modality == value)
					{
						_selectedPresetVoiLutGroup = group;
						PopulateTable();
						base.NotifyPropertyChanged("SelectedModality");
						return;
					}
				}

				throw new ArgumentException("The input value does not match an existing Modality.");
			}
		}

		public ITable VoiLutPresets
		{
			get { return _voiLutPresets; }
		}

		public ISelection Selection
		{
			get { return _selection; }
			set
			{
				_selection = value;
				NotifyPropertyChanged("Selection");

				UpdateButtonStates();
			}
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				return _toolbarModel;
			}
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return _contextMenuModel;
			}
		}

		public bool HasMultipleFactories
		{
			get
			{
				return _availableAddFactories.Count > 1;
			}
		}

		public bool AddEnabled
		{
			get
			{
				return _toolbarModel["add"].Enabled;
			}
			private set
			{
				_toolbarModel["add"].Enabled = value;
				_contextMenuModel["add"].Enabled = value;
				NotifyPropertyChanged("AddEnabled");
			}
		}

		public bool EditEnabled
		{
			get { return _toolbarModel["edit"].Enabled; }
			private set
			{
				_toolbarModel["edit"].Enabled = value;
				_contextMenuModel["edit"].Enabled = value;
				NotifyPropertyChanged("EditEnabled");
			}
		}

		private bool DeleteEnabled
		{
			get { return _toolbarModel["delete"].Enabled; }
			set
			{
				_toolbarModel["delete"].Enabled = value;
				_contextMenuModel["delete"].Enabled = value;
				NotifyPropertyChanged("DeleteEnabled");
			}
		}

		private PresetVoiLut SelectedPresetVoiLut
		{
			get
			{
				if (this.Selection == null)
					return null;

				return (PresetVoiLut)this.Selection.Item;
			}
		}

		private IPresetVoiLutApplicator SelectedPresetApplicator
		{
			get
			{
				if (this.SelectedPresetVoiLut == null)
					return null;

				return this.SelectedPresetVoiLut.Applicator;
			}
		}

		public override void Start()
		{
			InitializeAddFactories();
			_selectedAddFactory = new PresetFactoryDescriptor(PresetVoiLutApplicatorFactories.GetFactory(LinearPresetVoiLutApplicatorFactory.FactoryName));

			_voiLutPresets = new Table<PresetVoiLut>();

			_availableModalities = new List<string>(StandardModalities.Modalities);
			if (!_availableModalities.Contains(""))
				_availableModalities.Add("");

			_presetVoiLutGroups = PresetVoiLutSettings.Default.GetPresetGroups().Clone();

			foreach (string modality in _availableModalities)
			{
				if (!_presetVoiLutGroups.Contains(new PresetVoiLutGroup(modality)))
					_presetVoiLutGroups.Add(new PresetVoiLutGroup(modality));
			}

			_presetVoiLutGroups.Sort(new PresetVoiLutGroupSortByModality());

			_selectedPresetVoiLutGroup = _presetVoiLutGroups[0];

			InitializeMenuAndToolbar();
			InitializeTable();
			PopulateTable();

			Selection = null;

			base.Start();
		}

		public void OnAdd()
		{
			if (!AddEnabled)
				return;

			IPresetVoiLutApplicatorComponent addComponent = SelectedAddFactory.Factory.GetEditComponent(null);
			addComponent.EditContext = EditContext.Add;
			PresetVoiLutApplicatorComponentContainer container = new PresetVoiLutApplicatorComponentContainer(GetUnusedKeyStrokes(null), addComponent);

			if (ApplicationComponentExitCode.Normal != ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, container, "Add Preset"))
				return;

			PresetVoiLut preset = container.GetPresetVoiLut();
			if (preset == null)
			{
				this.Host.DesktopWindow.ShowMessageBox("An error has occurred while adding the preset.", MessageBoxActions.Ok);
			}

			List<PresetVoiLut> conflictingItems = CollectionUtils.Select<PresetVoiLut>(_selectedPresetVoiLutGroup.Presets,
			                                                                           delegate(PresetVoiLut test) { return preset.Equals(test); });

			if (conflictingItems.Count == 0)
			{
				_selectedPresetVoiLutGroup.Presets.Add(preset);
				_voiLutPresets.Items.Add(preset);
				Selection = null;

				this.Modified = true;
			}
			else
			{
				this.Host.DesktopWindow.ShowMessageBox("The name and/or keystroke entered conflicts with at least one other existing preset.", MessageBoxActions.Ok);
			}
		}

		public void OnEditSelected()
		{
			if (!EditEnabled)
				return;

			if (this.SelectedPresetVoiLut == null)
			{
				this.Host.DesktopWindow.ShowMessageBox("Please Select an item to edit.", MessageBoxActions.Ok);
				return;
			}

			PresetVoiLutConfiguration configuration = this.SelectedPresetApplicator.GetConfiguration();
			IPresetVoiLutApplicatorComponent editComponent = this.SelectedPresetApplicator.SourceFactory.GetEditComponent(configuration);
			editComponent.EditContext = EditContext.Edit;
			PresetVoiLutApplicatorComponentContainer container = new PresetVoiLutApplicatorComponentContainer(GetUnusedKeyStrokes(this.SelectedPresetVoiLut), editComponent);
			container.SelectedKeyStroke = this.SelectedPresetVoiLut.KeyStroke;

			if (ApplicationComponentExitCode.Normal != ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, container, "Edit Preset"))
				return;

			PresetVoiLut preset = container.GetPresetVoiLut();
			if (preset == null)
			{
				this.Host.DesktopWindow.ShowMessageBox("An error has occurred while editing the preset.", MessageBoxActions.Ok);
			}

			List<PresetVoiLut> conflictingItems = CollectionUtils.Select<PresetVoiLut>(_selectedPresetVoiLutGroup.Presets,
			                                                                           delegate(PresetVoiLut test) { return preset.Equals(test); });

			if (conflictingItems.Count == 0 || (conflictingItems.Count == 1 && conflictingItems[0].Equals(this.SelectedPresetVoiLut)))
			{
				PresetVoiLut selected = this.SelectedPresetVoiLut;

				_selectedPresetVoiLutGroup.Presets.Remove(selected);
				_selectedPresetVoiLutGroup.Presets.Add(preset);

				int index = _voiLutPresets.Items.IndexOf(selected);
				_voiLutPresets.Items.Remove(selected);
				
				if (index < _voiLutPresets.Items.Count)
					_voiLutPresets.Items.Insert(index, preset);
				else
					_voiLutPresets.Items.Add(preset);

				Selection = null;

				this.Modified = true;
			}
			else
			{
				this.Host.DesktopWindow.ShowMessageBox("The name and/or keystroke entered conflicts with at least one other existing preset.", MessageBoxActions.Ok);
			}
		}

		public void OnDeleteSelected()
		{
			if (!DeleteEnabled)
				return;

			if (this.SelectedPresetVoiLut == null)
			{
				this.Host.DesktopWindow.ShowMessageBox("Please Select an item to delete.", MessageBoxActions.Ok);
				return;
			}

			_selectedPresetVoiLutGroup.Presets.Remove(this.SelectedPresetVoiLut);
			_voiLutPresets.Items.Remove(this.SelectedPresetVoiLut);
			this.Selection = null;

			this.Modified = true;
		}

		public override void Save()
		{
			try
			{
				PresetVoiLutSettings.Default.SetPresetGroups(_presetVoiLutGroups);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, "Failed to save Window/Level Preset changes.", this.Host.DesktopWindow);
			}
		}

		private void InitializeMenuAndToolbar()
		{
			ResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);

			_toolbarModel = new SimpleActionModel(resolver);
			_toolbarModel.AddAction("add", "Add", "AddToolSmall.png", OnAdd);
			_toolbarModel.AddAction("edit", "Edit", "EditToolSmall.png", OnEditSelected);
			_toolbarModel.AddAction("delete", "Delete", "DeleteToolSmall.png", OnDeleteSelected);

			_contextMenuModel = new SimpleActionModel(resolver);
			_contextMenuModel.AddAction("add", "Add", "AddToolSmall.png", OnAdd);
			_contextMenuModel.AddAction("edit", "Edit", "EditToolSmall.png", OnEditSelected);
			_contextMenuModel.AddAction("delete", "Delete", "DeleteToolSmall.png", OnDeleteSelected);

			_toolbarModel["add"].Visible = !HasMultipleFactories;
			_contextMenuModel["add"].Visible = !HasMultipleFactories;

			UpdateButtonStates();
		}

		private void InitializeTable()
		{
			TableColumn<PresetVoiLut, string> column;

			column = new TableColumn<PresetVoiLut, string>("Key", delegate(PresetVoiLut item) { return item.KeyStrokeDescriptor.ToString(); }, 0.2f);
			_voiLutPresets.Columns.Add(column);
	
			column = new TableColumn<PresetVoiLut, string>("Name", delegate(PresetVoiLut item) { return item.Applicator.Name; }, 0.2f);
			_voiLutPresets.Columns.Add(column);

			column = new TableColumn<PresetVoiLut, string>("Description", delegate(PresetVoiLut item) { return item.Applicator.Description; }, 0.6f);
			_voiLutPresets.Columns.Add(column);
		}

		private void PopulateTable()
		{
			this.Selection = null;

			_voiLutPresets.Items.Clear();

			foreach (PresetVoiLut preset in _selectedPresetVoiLutGroup.Presets)
				_voiLutPresets.Items.Add(preset);
		}
		
		private List<XKeys> GetUnusedKeyStrokes(PresetVoiLut include)
		{
			List<XKeys> keyStrokes = new List<XKeys>(AvailablePresetVoiLutKeyStrokeSettings.Default.GetAvailableKeyStrokes());

			foreach (PresetVoiLut presetVoiLut in this._selectedPresetVoiLutGroup.Presets)
			{
				if (include != null && include.KeyStroke == presetVoiLut.KeyStroke)
					continue;

				keyStrokes.Remove(presetVoiLut.KeyStroke);
			}

			if (!keyStrokes.Contains(XKeys.None))
				keyStrokes.Add(XKeys.None);

			//put 'None' at the top.
			keyStrokes.Sort();

			return keyStrokes;
		}

		private void InitializeAddFactories()
		{
			if (_availableAddFactories == null)
				_availableAddFactories = new List<PresetFactoryDescriptor>();

			foreach (IPresetVoiLutApplicatorFactory factory in PresetVoiLutApplicatorFactories.Factories)
			{
				_availableAddFactories.Add(new PresetFactoryDescriptor(factory));
			}

			_availableAddFactories.Sort();
		}

		private void UpdateButtonStates()
		{
			bool editDeleteEnabled = this.Selection != null && this.Selection.Item != null;
			//update the edit & delete toolbar and context menu buttons.
			EditEnabled = editDeleteEnabled;
			DeleteEnabled = editDeleteEnabled;

			bool addEnabled = true;
			if (!_selectedAddFactory.Factory.CanCreateMultiple)
			{
				foreach (PresetVoiLut preset in _selectedPresetVoiLutGroup.Presets)
				{
					if (preset.Applicator.SourceFactory == _selectedAddFactory.Factory)
					{
						addEnabled = false;
						break;
					}
				}
			}

			AddEnabled = addEnabled;
		}
	}
}