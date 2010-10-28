#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SelectorEditorComponent"/>
	/// </summary>
	public partial class SelectorEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly SelectorEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public SelectorEditorComponentControl(SelectorEditorComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			_usersSelector.AvailableItemsTable = _component.AvailableItemsTable;
			_usersSelector.SelectedItemsTable = _component.SelectedItemsTable;
			_usersSelector.ItemAdded += OnItemsAdded;
			_usersSelector.ItemRemoved += OnItemsRemoved;
			_usersSelector.ReadOnly = _component.IsReadOnly;

		}

		private void OnItemsAdded(object sender, EventArgs args)
		{
			_component.NotifyItemsAdded();
		}

		private void OnItemsRemoved(object sender, EventArgs args)
		{
			_component.NotifyItemsRemoved();
		}
	}
}
