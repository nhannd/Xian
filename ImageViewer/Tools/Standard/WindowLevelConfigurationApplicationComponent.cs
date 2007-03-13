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
	public class WindowLevelConfigurationApplicationComponent : ConfigurationApplicationComponent
	{
		public class WindowLevelPreset
		{
			private string _name;
			private int _window;
			private int _level;

			public WindowLevelPreset()
			{

			}

			public WindowLevelPreset(string name, int window, int level)
			{
				_name = name;
				_window = window;
				_level = level;
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

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowLevelConfigurationApplicationComponent()
		{

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
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Save()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetSelection(ISelection selection)
		{
			_selectedPreset = selection.Item as WindowLevelPreset;
		}

		public void AddPreset()
		{
			WindowLevelPresetApplicationComponent presetComponent = 
				new WindowLevelPresetApplicationComponent("", 400, 200);
			DialogContent content = new DialogContent(presetComponent);
			DialogComponentContainer dialog = new DialogComponentContainer(content);
			ApplicationComponentExitCode code = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, dialog, "Add Window/Level Preset");

			if (code == ApplicationComponentExitCode.Normal)
			{
				WindowLevelPreset preset =
					new WindowLevelPreset(
						presetComponent.Name,
						presetComponent.Window,
						presetComponent.Level);

				this.SelectedPresetList.Items.Add(preset);
				this.Modified = true;
			}
		}

		public void EditPreset()
		{
			WindowLevelPresetApplicationComponent presetComponent =
				new WindowLevelPresetApplicationComponent(
				_selectedPreset.Name,
				_selectedPreset.Window,
				_selectedPreset.Level);
			DialogContent content = new DialogContent(presetComponent);
			DialogComponentContainer dialog = new DialogComponentContainer(content);
			ApplicationComponentExitCode code = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, dialog, "Edit Window/Level Preset");

			if (code == ApplicationComponentExitCode.Normal)
			{
				_selectedPreset.Name = presetComponent.Name;
				_selectedPreset.Window = presetComponent.Window;
				_selectedPreset.Level = presetComponent.Level;
				_selectedPresetList.Items.NotifyItemUpdated(_selectedPreset);
				this.Modified = true;
			}
		}

		public void DeletePreset()
		{
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
						"Name",
						delegate(WindowLevelPreset item) { return item.Name; },
						1.5f
						));
				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Window",
						delegate(WindowLevelPreset item) { return item.Window.ToString(); },
						1.5f
						));
				presetList.Columns.Add(
					new TableColumn<WindowLevelPreset, string>(
						"Level",
						delegate(WindowLevelPreset item) { return item.Level.ToString(); },
						1.5f
						));

				_presetLists.Add(modality, presetList);
			}
		}
	}
}
