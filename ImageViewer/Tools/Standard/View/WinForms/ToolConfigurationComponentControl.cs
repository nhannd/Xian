#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class ToolConfigurationComponentControl : UserControl
	{
		private readonly ToolModalityBehaviorCollection _collection;
		private readonly ToolConfigurationComponent _component;

		public ToolConfigurationComponentControl(ToolConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;

			_collection = new ToolModalityBehaviorCollection(component.ModalityBehavior);
			_collection.CollectionChanged += HandleCollectionChanged;

			var modalities = StandardModalities.Modalities.Union(new[] {string.Empty}).ToList();
			modalities.Sort(StringComparer.InvariantCultureIgnoreCase);
			var bindingSource = new BindingSource {DataSource = new BindingList<ToolModalityBehaviorSettings>(modalities.Select(s => new ToolModalityBehaviorSettings(_collection[s], string.IsNullOrEmpty(s) ? SR.LabelDefault : s)).ToList())};

			_cboModality.DataSource = bindingSource;
			_cboModality.DisplayMember = "Modality";

			_chkWindowLevel.DataBindings.Add("Checked", bindingSource, "SelectedOnlyWindowLevel", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkOrientation.DataBindings.Add("Checked", bindingSource, "SelectedOnlyOrientation", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkZoom.DataBindings.Add("Checked", bindingSource, "SelectedOnlyZoom", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkPan.DataBindings.Add("Checked", bindingSource, "SelectedOnlyPan", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkReset.DataBindings.Add("Checked", bindingSource, "SelectedOnlyReset", false, DataSourceUpdateMode.OnPropertyChanged);

			_chkInvertZoomDirection.DataBindings.Add("Checked", _component, "InvertZoomDirection", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void HandleCollectionChanged(object sender, EventArgs e)
		{
			_component.ModalityBehavior = new ToolModalityBehaviorCollection(_collection);
		}

		private class ToolModalityBehaviorSettings : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			private readonly ToolModalityBehavior _item;
			private readonly string _modality;

			private bool _selectedOnlyWindowLevel;
			private bool _selectedOnlyOrientation;
			private bool _selectedOnlyZoom;
			private bool _selectedOnlyPan;
			private bool _selectedOnlyReset;

			public ToolModalityBehaviorSettings(ToolModalityBehavior item, string modality)
			{
				_item = item;
				_modality = modality;
				_selectedOnlyWindowLevel = _item.SelectedImageWindowLevelTool || _item.SelectedImageWindowLevelPresetsTool || _item.SelectedImageInvertTool;
				_selectedOnlyOrientation = _item.SelectedImageFlipTool || _item.SelectedImageRotateTool;
				_selectedOnlyZoom = _item.SelectedImageZoomTool;
				_selectedOnlyPan = _item.SelectedImagePanTool;
				_selectedOnlyReset = _item.SelectedImageResetTool;
			}

			public string Modality
			{
				get { return _modality; }
			}

			public bool SelectedOnlyWindowLevel
			{
				get { return _selectedOnlyWindowLevel; }
				set
				{
					if (_selectedOnlyWindowLevel != value)
					{
						_selectedOnlyWindowLevel = value;
						NotifyPropertyChanged(@"SelectedOnlyWindowLevel");

						_item.SelectedImageWindowLevelTool = _item.SelectedImageWindowLevelPresetsTool = _item.SelectedImageInvertTool = value;
					}
				}
			}

			public bool SelectedOnlyOrientation
			{
				get { return _selectedOnlyOrientation; }
				set
				{
					if (_selectedOnlyOrientation != value)
					{
						_selectedOnlyOrientation = value;
						NotifyPropertyChanged(@"SelectedOnlyOrientation");

						_item.SelectedImageFlipTool = _item.SelectedImageRotateTool = value;
					}
				}
			}

			public bool SelectedOnlyZoom
			{
				get { return _selectedOnlyZoom; }
				set
				{
					if (_selectedOnlyZoom != value)
					{
						_selectedOnlyZoom = value;
						NotifyPropertyChanged(@"SelectedOnlyZoom");

						_item.SelectedImageZoomTool = value;
					}
				}
			}

			public bool SelectedOnlyPan
			{
				get { return _selectedOnlyPan; }
				set
				{
					if (_selectedOnlyPan != value)
					{
						_selectedOnlyPan = value;
						NotifyPropertyChanged(@"SelectedOnlyPan");

						_item.SelectedImagePanTool = value;
					}
				}
			}

			public bool SelectedOnlyReset
			{
				get { return _selectedOnlyReset; }
				set
				{
					if (_selectedOnlyReset != value)
					{
						_selectedOnlyReset = value;
						NotifyPropertyChanged(@"SelectedOnlyReset");

						_item.SelectedImageResetTool = value;
					}
				}
			}

			private void NotifyPropertyChanged(string propertyName)
			{
				EventsHelper.Fire(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}