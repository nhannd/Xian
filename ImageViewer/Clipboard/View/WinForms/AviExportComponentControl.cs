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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Clipboard.ImageExport;
using ClearCanvas.ImageViewer.Clipboard.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AviExportComponent"/>.
    /// </summary>
    public partial class AviExportComponentControl : ApplicationComponentUserControl, INotifyPropertyChanged
    {
        private AviExportComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AviExportComponentControl(AviExportComponent component)
            :base(component)
        {
            InitializeComponent();
			_component = component;
			_component.PropertyChanged += OnComponentPropertyChanged;

        	base.AcceptButton = _buttonOk;
        	base.CancelButton = _buttonCancel;

			_trackBarFrameRate.DataBindings.Add("Minimum", _component, "MinFrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarFrameRate.DataBindings.Add("Maximum", _component, "MaxFrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarFrameRate.DataBindings.Add("Value", _component, "FrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding binding = new Binding("Text", _component, "FrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += new ConvertEventHandler(OnFormatFrameRate);
			_frameRate.DataBindings.Add(binding);
			
			binding = new Binding("Text", _component, "DurationSeconds", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += new ConvertEventHandler(OnFormatDuration);
        	_duration.DataBindings.Add(binding);

			_checkOptionWysiwyg.DataBindings.Add("Checked", this, "ExportOptionWysiwyg", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionCompleteImage.DataBindings.Add("Checked", this, "ExportOptionCompleteImage", true, DataSourceUpdateMode.OnPropertyChanged);

			_checkOptionScale.DataBindings.Add("Checked", this, "SizeModeScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionFixed.DataBindings.Add("Checked", this, "SizeModeFixed", true, DataSourceUpdateMode.OnPropertyChanged);
			_pnlScale.DataBindings.Add("Enabled", this, "SizeModeScale", true, DataSourceUpdateMode.Never);
			_pnlFixedSize.DataBindings.Add("Enabled", this, "SizeModeFixed", true, DataSourceUpdateMode.Never);

			_scale.DataBindings.Add("Maximum", this, "MaximumScalePercent", true, DataSourceUpdateMode.Never);
			_scale.DataBindings.Add("Minimum", this, "MinimumScalePercent", true, DataSourceUpdateMode.Never);
			_scale.DataBindings.Add("Value", this, "ScalePercent", true, DataSourceUpdateMode.OnPropertyChanged);

			_imageWidth.DataBindings.Add("Maximum", _component, "MaximumDimension", true, DataSourceUpdateMode.Never);
			_imageWidth.DataBindings.Add("Minimum", _component, "MinimumDimension", true, DataSourceUpdateMode.Never);
			_imageWidth.DataBindings.Add("Value", _component, "Width", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageHeight.DataBindings.Add("Maximum", _component, "MaximumDimension", true, DataSourceUpdateMode.Never);
			_imageHeight.DataBindings.Add("Minimum", _component, "MinimumDimension", true, DataSourceUpdateMode.Never);
			_imageHeight.DataBindings.Add("Value", _component, "Height", true, DataSourceUpdateMode.OnPropertyChanged);
			_backgroundColorSwatch.DataBindings.Add("BackColor", _component, "BackgroundColor", true, DataSourceUpdateMode.Never);
        	_chkShowTextOverlay.DataBindings.Add("Checked", _component, "ShowTextOverlay", true, DataSourceUpdateMode.OnPropertyChanged);
		}

    	private void DoDispose(bool disposing)
    	{
    		if (disposing)
    		{
    			if (_component != null)
    			{
    				_component.PropertyChanged -= OnComponentPropertyChanged;
    				_component = null;
    			}
    		}
    	}

    	#region INotifyPropertyChanged Members

    	private event PropertyChangedEventHandler _propertyChanged;

    	public event PropertyChangedEventHandler PropertyChanged
    	{
    		add { _propertyChanged += value; }
    		remove { _propertyChanged -= value; }
    	}

    	protected virtual void NotifyPropertyChanged(string propertyName)
    	{
    		EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
    	}

    	#endregion

		private void OnFormatFrameRate(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			e.Value = String.Format("({0})", (int)e.Value);
		}

		private void OnFormatDuration(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			e.Value = String.Format(((float) e.Value).ToString("F2"));
		}

		private void OnAdvanced(object sender, EventArgs e)
		{
			_component.ShowAdvanced();
		}

		private void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void OnOk(object sender, EventArgs e)
		{
			using (SaveFileDialog dialog = new SaveFileDialog())
			{
				dialog.Filter = _component.FileExtensionFilter;
				dialog.DefaultExt = _component.DefaultFileExtension;
				dialog.AddExtension = true;
				dialog.RestoreDirectory = true;

				if (DialogResult.OK == dialog.ShowDialog())
				{
					_component.FilePath = dialog.FileName;
					_component.Accept();
				}
			}
		}

    	private void OnBackgroundColorSwatchClick(object sender, EventArgs e)
    	{
    		Settings settings = Settings.Default;
    		using (ColorDialog dlg = new ColorDialog())
    		{
    			dlg.AllowFullOpen = true;
    			dlg.AnyColor = true;
    			dlg.Color = _component.BackgroundColor;
    			dlg.CustomColors = settings.CustomColorsArray;
    			dlg.FullOpen = settings.ExpandColorDialog;
    			dlg.ShowHelp = false;
    			dlg.SolidColorOnly = false;

    			if (dlg.ShowDialog(this) == DialogResult.OK)
    			{
    				_component.BackgroundColor = dlg.Color;
    				settings.CustomColorsArray = dlg.CustomColors;
    				settings.Save();
    			}
    		}
		}

    	private void OnShowTextOverlayCheckedChanged(object sender, EventArgs e)
    	{
    		_pnlWarningPatientPrivacy.Visible = _chkShowTextOverlay.Checked;
    	}

    	private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
    	{
    		switch (e.PropertyName)
    		{
    			case "ExportOption":
    				this.ExportOptionWysiwyg = (_component.ExportOption == ExportOption.Wysiwyg);
    				this.ExportOptionCompleteImage = (_component.ExportOption == ExportOption.CompleteImage);
    				break;
    			case "SizeMode":
    				this.SizeModeScale = (_component.SizeMode == SizeMode.Scale);
    				this.SizeModeFixed = (_component.SizeMode == SizeMode.Fixed);
    				break;
    		}
    	}

    	public float ScalePercent
    	{
    		get { return 100*_component.Scale; }
    		set
    		{
    			value = value/100;
    			if (value != _component.Scale)
    			{
    				_component.Scale = value;
    				this.NotifyPropertyChanged("ScalePercent");
    			}
    		}
    	}

    	public float MaximumScalePercent
    	{
    		get { return 100*_component.MaximumScale; }
    	}

    	public float MinimumScalePercent
    	{
    		get { return 100*_component.MinimumScale; }
    	}

    	public bool ExportOptionWysiwyg
    	{
    		get { return _component.ExportOption == ExportOption.Wysiwyg; }
    		set
    		{
    			if (_component.ExportOption == ExportOption.Wysiwyg && !value)
    			{
    				_component.ExportOption = ExportOption.CompleteImage;
    				this.NotifyPropertyChanged("ExportOptionWysiwyg");
    			}
    		}
    	}

    	public bool ExportOptionCompleteImage
    	{
    		get { return _component.ExportOption == ExportOption.CompleteImage; }
    		set
    		{
    			if (_component.ExportOption == ExportOption.CompleteImage && !value)
    			{
    				_component.ExportOption = ExportOption.Wysiwyg;
    				this.NotifyPropertyChanged("ExportOptionCompleteImage");
    			}
    		}
    	}

    	public bool SizeModeScale
    	{
    		get { return _component.SizeMode == SizeMode.Scale; }
    		set
    		{
    			if (_component.SizeMode == SizeMode.Scale && !value)
    			{
    				_component.SizeMode = SizeMode.Fixed;
    				this.NotifyPropertyChanged("SizeModeScale");
    			}
    		}
    	}

    	public bool SizeModeFixed
    	{
    		get { return _component.SizeMode == SizeMode.Fixed; }
    		set
    		{
    			if (_component.SizeMode == SizeMode.Fixed && !value)
    			{
    				_component.SizeMode = SizeMode.Scale;
    				this.NotifyPropertyChanged("SizeModeFixed");
    			}
    		}
    	}
	}
}
