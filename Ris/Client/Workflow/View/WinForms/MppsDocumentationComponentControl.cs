#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="CheckInOrderComponent"/>
	/// </summary>
	public partial class MppsDocumentationComponentControl : ApplicationComponentUserControl
	{
		private readonly MppsDocumentationComponent _component;
		private CannedTextSupport _cannedTextSupport;

		/// <summary>
		/// Constructor
		/// </summary>
		public MppsDocumentationComponentControl(MppsDocumentationComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_label.DataBindings.Add("Text", _component, "CommentsLabel", true, DataSourceUpdateMode.OnPropertyChanged);
			_comments.DataBindings.Add("Text", _component, "Comments", true, DataSourceUpdateMode.OnPropertyChanged);
			_comments.DataBindings.Add("Enabled", _component, "CommentsEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_cannedTextSupport = new CannedTextSupport(_comments, _component.CannedTextLookupHandler);
		}
	}
}
