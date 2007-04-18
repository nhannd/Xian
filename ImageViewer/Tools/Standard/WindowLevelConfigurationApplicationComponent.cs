using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using System.Collections;
using ClearCanvas.ImageViewer.Tools.Standard.LutPresets;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Extension point for views onto <see cref="WindowLevelConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class WindowLevelConfigurationApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WindowLevelConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(WindowLevelConfigurationApplicationComponentViewExtensionPoint))]
	public partial class WindowLevelConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		public class WindowLevelPreset
		{
			private XKeys _key;
			private string _name;
			private int _window;
			private int _level;

			public WindowLevelPreset()
			{

			}

			public WindowLevelPreset(XKeys key, string name, int window, int level)
			{
				_key = key;
				_name = name;
				_window = window;
				_level = level;
			}

			public XKeys Key
			{
				get { return _key; }
				set { _key = value; }
			}

			public string Name
			{
				get { return _name; }
				set { _name = value; }
			}

			public int Window
			{
				get { return _window; }
				set { _window = value; }
			}

			public int Level
			{
				get { return _level; }
				set { _level = value; }
			}
		}

		#region Private fields

		private Dictionary<string, Table<WindowLevelPreset>> _presetLists;
		private Table<WindowLevelPreset> _selectedPresetList;
		private string _selectedModality;
		private WindowLevelPreset _selectedPreset;
		private List<XKeys> _availableKeyStrokes;
		private VoiLutPresetConfigurationCollection _currentConfiguration;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowLevelConfigurationApplicationComponent()
		{
			_availableKeyStrokes = new List<XKeys>();
			_availableKeyStrokes.Add(XKeys.None);
			foreach (XKeys availableKeyStroke in AvailableLutPresetKeyStrokes.GetAvailableKeyStrokes())
			{
				if (availableKeyStroke != XKeys.None)
					_availableKeyStrokes.Add(availableKeyStroke);
			}
		}

		public ICollection<string> ModalityList
		{
			get { return StandardModalities.Modalities; }
		}

		public string SelectedModality
		{
			get 
			{
				return _selectedModality;
			}
			set 
			{ 
				_selectedModality = value;
				this.SelectedPresetList = _presetLists[_selectedModality];
			}
		}

		public Table<WindowLevelPreset> SelectedPresetList
		{
			get { return _selectedPresetList; }
			set 
			{
				_selectedPresetList = value;
				this.NotifyPropertyChanged("SelectedPresetList");
			}
		}

		public override void Start()
		{
			base.Start();

			CreatePresetLists();

			// Move to the beginning of the modality list.
			this.ModalityList.GetEnumerator().Reset();
			IEnumerator<string> enumerator = this.ModalityList.GetEnumerator();
			enumerator.MoveNext();

			this.SelectedModality = enumerator.Current;

			this.ModalityList.GetEnumerator().Reset();

			Load();
		}

		public override void Stop()
		{
			base.Stop();
		}

		private VoiLutPresetConfiguration ConvertToLutPresetConfiguration(WindowLevelPreset preset)
		{ 
			return new VoiLutPresetConfiguration(_selectedModality, preset.Key, preset.Name,
				VoiLutLinearPresetApplicatorFactory.CreateVoiLutLinearApplicatorConfiguration(preset.Window, preset.Level));
		}

		private WindowLevelPreset ConvertToWindowLevelPreset(VoiLutPresetConfiguration presetConfiguration)
		{
			WindowLevelPreset preset = new WindowLevelPreset();
			preset.Key = presetConfiguration.KeyStroke;
			preset.Name = presetConfiguration.Name;
			preset.Window = int.Parse(presetConfiguration.VoiLutPresetApplicatorConfiguration.ConfigurationValues["WindowWidth"]);
			preset.Level = int.Parse(presetConfiguration.VoiLutPresetApplicatorConfiguration.ConfigurationValues["WindowCenter"]);

			return preset;
		}

		private void Load()
		{
			VoiLutPresetConfigurationCollection configuration = VoiLutPresetSettings.Default.GetVoiLutPresetConfigurations();
			_currentConfiguration = configuration;

			foreach (KeyValuePair<string, IEnumerable<VoiLutPresetConfiguration>> modalityConfiguration in
				VoiLutPresetConfigurations.GetConfigurationsByModality(VoiLutLinearPresetApplicatorFactory.InternalFactoryKey))
			{
				string modality = modalityConfiguration.Key;
				if (!_presetLists.ContainsKey(modality))
					continue;

				foreach (VoiLutPresetConfiguration presetConfiguration in modalityConfiguration.Value)
					_presetLists[modality].Items.Add(this.ConvertToWindowLevelPreset(presetConfiguration));
			}
		}

		public override void Save()
		{
			VoiLutPresetSettings.Default.SetVoiLutPresetGroupConfigurations(_currentConfiguration);
		}

		public void SetSelection(ISelection selection)
		{
			_selectedPreset = selection.Item as WindowLevelPreset;
		}

		public void AddPreset()
		{
			WindowLevelPresetApplicationComponent presetComponent =
				new WindowLevelPresetApplicationComponent(_availableKeyStrokes, _availableKeyStrokes[0], "", 400, 200);
			DialogContent content = new DialogContent(presetComponent);
			DialogComponentContainer dialog = new DialogComponentContainer(content);
			ApplicationComponentExitCode code = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, dialog, SR.TitleAddWindowLevelPreset);

			if (code == ApplicationComponentExitCode.Normal)
			{
				WindowLevelPreset preset =
					new WindowLevelPreset(presetComponent.SelectedKey,
						presetComponent.Name,
						presetComponent.Window,
						presetComponent.Level);

				VoiLutPresetConfiguration newConfiguration = this.ConvertToLutPresetConfiguration(preset);

				try
				{
					_currentConfiguration.Add(newConfiguration);

					this.SelectedPresetList.Items.Add(preset);
					this.Modified = true;
				}
				catch (Exception e)
				{ 
					//the exception thrown is a user message.
					Platform.ShowMessageBox(e.Message);
				}
			}
		}

		public void EditPreset()
		{
			WindowLevelPresetApplicationComponent presetComponent =
				new WindowLevelPresetApplicationComponent(
				_availableKeyStrokes, 
				_selectedPreset.Key,
				_selectedPreset.Name,
				_selectedPreset.Window,
				_selectedPreset.Level);
			DialogContent content = new DialogContent(presetComponent);
			DialogComponentContainer dialog = new DialogComponentContainer(content);
			ApplicationComponentExitCode code = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, dialog, "Edit Window/Level Preset");

			if (code == ApplicationComponentExitCode.Normal)
			{
				WindowLevelPreset editPreset = new WindowLevelPreset(presetComponent.SelectedKey, presetComponent.Name, presetComponent.Window, presetComponent.Level);

				VoiLutPresetConfiguration existingConfiguration = this.ConvertToLutPresetConfiguration(_selectedPreset);
				VoiLutPresetConfiguration editConfiguration = this.ConvertToLutPresetConfiguration(editPreset);

				try
				{
					_currentConfiguration.Replace(existingConfiguration, editConfiguration);

					_selectedPreset.Key = presetComponent.SelectedKey;
					_selectedPreset.Name = presetComponent.Name;
					_selectedPreset.Window = presetComponent.Window;
					_selectedPreset.Level = presetComponent.Level;
					_selectedPresetList.Items.NotifyItemUpdated(_selectedPreset);
					
					this.Modified = true;
				}
				catch (Exception e)
				{
					//the exception thrown is a user message.
					Platform.ShowMessageBox(e.Message);
				}
			}
		}

		public void DeletePreset()
		{
			VoiLutPresetConfiguration existingConfiguration = this.ConvertToLutPresetConfiguration(_selectedPreset);

			_currentConfiguration.Remove(existingConfiguration);
			this.SelectedPresetList.Items.Remove(_selectedPreset);
			this.Modified = true;
		}

		private void CreatePresetLists()
		{
			_presetLists = new Dictionary<string, Table<WindowLevelPreset>>(); 
			
			foreach (string modality in this.ModalityList)
			{
				Table<WindowLevelPreset> presetList = new Table<WindowLevelPreset>();

				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Key",
						delegate(WindowLevelPreset item) { return item.Key.ToString(); },
						1f
						));

				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Name",
						delegate(WindowLevelPreset item) { return item.Name; },
						1.5f
						));
				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Window",
						delegate(WindowLevelPreset item) { return item.Window.ToString(); },
						1f
						));
				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Level",
						delegate(WindowLevelPreset item) { return item.Level.ToString(); },
						1f
						));

				_presetLists.Add(modality, presetList);
			}
		}
	}
}
