#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="TranscriptionEditorComponent"/>.
	/// </summary>
	public partial class RichTextReportEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly IReportEditorComponent _component;
		private readonly CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor.
		/// </summary>
		public RichTextReportEditorComponentControl(IReportEditorComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_richText.DataBindings.Add("Text", _component, "EditorText", true, DataSourceUpdateMode.OnPropertyChanged);
			_cannedTextSupport = new CannedTextSupport(_richText, _component.CannedTextLookupHandler);

			Control reportPreview = (Control)_component.ReportPreviewHost.ComponentView.GuiElement;
			reportPreview.Dock = DockStyle.Fill;
			_splitContainer.Panel1.Controls.Add(reportPreview);
			UpdatePreviewVisibility();

			((INotifyPropertyChanged)_component).PropertyChanged += _component_PropertyChanged;
		}

		void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EditorText")
			{
				_richText.Text = _component.EditorText;
			}
			else if (e.PropertyName == "PreviewVisible")
			{
				UpdatePreviewVisibility();
			}
		}

		private void UpdatePreviewVisibility()
		{
			_splitContainer.Panel1Collapsed = !_component.PreviewVisible;
		}
	}
}
