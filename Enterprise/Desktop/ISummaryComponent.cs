#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Enterprise.Desktop
{
	/// <summary>
	/// Represents admin summary components.
	/// </summary>
	public interface ISummaryComponent : IApplicationComponent
	{
		/// <summary>
		/// Gets the summary table action model.
		/// </summary>
		ActionModelNode SummaryTableActionModel { get; }

		/// <summary>
		/// Gets the summary table <see cref="ITable"/>.
		/// </summary>
		ITable SummaryTable { get; }

		/// <summary>
		/// Gets the summary table selection as an <see cref="ISelection"/>.
		/// </summary>
		ISelection SummarySelection { get; set; }

		/// <summary>
		/// Gets a value indicating whether dialog accept/cancel buttons are visible.
		/// </summary>
		bool ShowAcceptCancelButtons { get; }

		/// <summary>
		/// Gets a value indicating whether accept button is enabled.
		/// </summary>
		bool AcceptEnabled { get; }

		/// <summary>
		/// Handles the "search" action if supported.
		/// </summary>
		void Search();

		/// <summary>
		/// Handles the "add" action.
		/// </summary>
		void AddItems();

		/// <summary>
		/// Handles the "edit" action.
		/// </summary>
		void EditSelectedItems();

		/// <summary>
		/// Handles the "delete" action.
		/// </summary>
		void DeleteSelectedItems();

		/// <summary>
		/// Handles the "toggle activation" action.
		/// </summary>
		void ToggleSelectedItemsActivation();

		/// <summary>
		/// Handles double-clicking on a list item.
		/// </summary>
		void DoubleClickSelectedItem();

		/// <summary>
		/// Handles the accept button.
		/// </summary>
		void Accept();

		/// <summary>
		/// Handles the cancel button.
		/// </summary>
		void Cancel();
	}

	/// <summary>
	/// Represents admin summary components.
	/// </summary>
	public interface ISummaryComponent<TSummary, TTable> : ISummaryComponent
		where TSummary : class
		where TTable : Table<TSummary>, new()
	{
		/// <summary>
		/// Gets or sets a value indicating whether this component will set <see cref="ApplicationComponent.Modified"/> to true
		/// when the summary list is changed.
		/// </summary>
		bool SetModifiedOnListChange { get; set; }

		/// <summary>
		/// Gets or sets whether the component is being hosted by another component.  Hosted mode overrides dialog mode.
		/// When being hosted, the Accept/Cancel and double click actions are disabled.  It is up to the hosting component
		/// to decide what to do.
		/// </summary>
		bool HostedMode { get; set; }

		/// <summary>
		/// Gets or sets whether this component is in a read-only mode.  If true, the defaul add, edit, delete, and toggle activation
		/// actions will be hidden.
		/// </summary>
		bool ReadOnly { get; set; }

		/// <summary>
		/// Gets or sets whether this component includes de-activated items in the list.
		/// </summary>
		bool IncludeDeactivatedItems { get; set; }

		event EventHandler SummarySelectionChanged;
		event EventHandler ItemDoubleClicked;
	}
}