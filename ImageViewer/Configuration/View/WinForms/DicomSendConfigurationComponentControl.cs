#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="DicomSendConfigurationComponent"/>
	/// </summary>
	public partial class DicomSendConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly DicomSendConfigurationComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public DicomSendConfigurationComponentControl(DicomSendConfigurationComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_maxNumberOfRetries.DataBindings.Add("Value", _component, "MaxNumberOfRetries", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayValue.DataBindings.Add("Value", _component, "RetryDelayValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayValue.DataBindings.Add("Maximum", _component, "MaxRetryDelayValue");

			_retryDelayUnits.Items.AddRange(_component.RetryDelayUnits.Cast<object>().ToArray());
			_retryDelayUnits.DataBindings.Add("SelectedItem", _component, "RetryDelayUnit", true, DataSourceUpdateMode.OnPropertyChanged);
			_retryDelayUnits.Format += (sender, e) => { e.Value = _component.FormatRetryDelayUnit(e.ListItem); };
			
			// bug #10076: combobox databinding doesn't apply change until it loses focus, so we do it manually
			_retryDelayUnits.SelectedIndexChanged += (sender, args) =>
			{
				_component.RetryDelayUnit = (RetryDelayTimeUnit)_retryDelayUnits.SelectedItem;
			};
		}
	}
}
