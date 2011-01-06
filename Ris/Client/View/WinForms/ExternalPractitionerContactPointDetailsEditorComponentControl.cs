#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerContactPointDetailsEditorComponent"/>
	/// </summary>
	public partial class ExternalPractitionerContactPointDetailsEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerContactPointDetailsEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointDetailsEditorComponentControl(ExternalPractitionerContactPointDetailsEditorComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			_component.PropertyChanged += _component_PropertyChanged;

			_name.DataBindings.Add("Value", _component, "ContactPointName", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "ContactPointDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_isDefaultContactPoint.DataBindings.Add("Checked", _component, "IsDefaultContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_resultCommunicationMode.DataBindings.Add("Value", _component, "SelectedResultCommunicationMode", true, DataSourceUpdateMode.OnPropertyChanged);
			_resultCommunicationMode.DataSource = _component.ResultCommunicationModeChoices;
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsDefaultContactPoint")
			{
				if (_isDefaultContactPoint.Checked != _component.IsDefaultContactPoint)
				{
					_isDefaultContactPoint.Checked = _component.IsDefaultContactPoint;
				}
			}
		}
	}
}
