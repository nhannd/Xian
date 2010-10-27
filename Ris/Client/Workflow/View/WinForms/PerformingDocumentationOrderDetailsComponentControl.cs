#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PerformingDocumentationOrderDetailsComponent"/>
	/// </summary>
	public partial class PerformingDocumentationOrderDetailsComponentControl : ApplicationComponentUserControl
	{
		private readonly PerformingDocumentationOrderDetailsComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PerformingDocumentationOrderDetailsComponentControl(PerformingDocumentationOrderDetailsComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			var protocols = (Control)_component.ProtocolHost.ComponentView.GuiElement;
			protocols.Dock = DockStyle.Fill;
			_protocolsPanel.Controls.Add(protocols);

			var notes = (Control)_component.NotesHost.ComponentView.GuiElement;
			notes.Dock = DockStyle.Fill;
			_orderNotesGroupBox.Controls.Add(notes);

			var rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);
		}
	}
}
