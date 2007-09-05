using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;
using ClearCanvas.Desktop.Configuration;
using System;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="VoiLutConfigurationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class VoiLutConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// VoiLutConfigurationComponent class
	/// </summary>
	[AssociateView(typeof(VoiLutConfigurationComponentViewExtensionPoint))]
	public sealed class VoiLutConfigurationComponent : ConfigurationApplicationComponent
	{
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
		public VoiLutConfigurationComponent()
		{
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
				NotifyPropertyChanged("SelectedVoiLutPreset");

				bool editDeleteEnabled = _selection != null && _selection.Item != null;
				EditEnabled = editDeleteEnabled;
				DeleteEnabled = editDeleteEnabled;
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

		private bool EditEnabled
		{
			get { return _toolbarModel["edit"].Enabled; }
			set
			{
				_toolbarModel["edit"].Enabled = value;
				_contextMenuModel["edit"].Enabled = value;
			}
		}

		private bool DeleteEnabled
		{
			get { return _toolbarModel["delete"].Enabled; }
			set
			{
				 _toolbarModel["delete"].Enabled = value;
				 _contextMenuModel["delete"].Enabled = value;
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

		private IPresetVoiLutApplicatorFactory CurrentAddPresetApplicatorFactory
		{
			get
			{
				//TODO: later, we will add support for adding from different factories, but right now it is disabled.
				return PresetVoiLutApplicatorFactories.GetFactory(PresetVoiLutLinearApplicatorFactory.FactoryName);
			}
		}

		public override void Start()
		{
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
			IEditPresetVoiLutApplicationComponent addComponent = CurrentAddPresetApplicatorFactory.GetEditComponent(null);
			EditPresetVoiLutComponentContainer container = new EditPresetVoiLutComponentContainer(GetUnusedKeyStrokes(null), addComponent);

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
			if (this.SelectedPresetVoiLut == null)
			{
				this.Host.DesktopWindow.ShowMessageBox("Please Select an item to edit.", MessageBoxActions.Ok);
				return;
			}

			PresetVoiLutConfiguration configuration = this.SelectedPresetApplicator.GetConfiguration();
			IEditPresetVoiLutApplicationComponent editComponent = this.SelectedPresetApplicator.SourceFactory.GetEditComponent(configuration);
			EditPresetVoiLutComponentContainer container = new EditPresetVoiLutComponentContainer(GetUnusedKeyStrokes(this.SelectedPresetVoiLut), editComponent);
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
			PresetVoiLut selected = this.SelectedPresetVoiLut;
			if (selected == null)	
				return;

			_selectedPresetVoiLutGroup.Presets.Remove(selected);
			_voiLutPresets.Items.Remove(selected);
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
		}

		private void InitializeTable()
		{
			TableColumn<PresetVoiLut, string> column;

			column = new TableColumn<PresetVoiLut, string>("Key",delegate(PresetVoiLut item) { return item.KeyStroke.ToString(); }, 0.2f);
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
			List<XKeys> keyStrokes = new List<XKeys>(AvailableLutKeyStrokeSettings.Default.GetAvailableKeyStrokes());

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
	}
}
