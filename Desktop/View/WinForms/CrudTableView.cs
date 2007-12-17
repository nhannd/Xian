#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A <see cref="TableView"/> that has Create, Edit and Delete buttons
	/// as part of the toolstrip and context menu.
	/// </summary>
	public class CrudTableView : TableView
	{
		#region Private fields

		private ToolStripButton _addButton;
		private ToolStripButton _editButton;
		private ToolStripButton _deleteButton;

		private ToolStripMenuItem _addMenuItem;
		private ToolStripMenuItem _editMenuItem;
		private ToolStripMenuItem _deleteMenuItem;

		private event EventHandler _addClickedEvent;
		private event EventHandler _editClickedEvent;
		private event EventHandler _deleteClickedEvent;

		#endregion

		public CrudTableView()
		{
			AddToolStripButtons();
			AddContextMenuItems();
			this.AddEnabled = true;
			this.EditEnabled = false;
			this.DeleteEnabled = false;
			this.ItemDoubleClicked += new EventHandler(OnItemDoubleClicked);
			this.SelectionChanged += new EventHandler(OnSelectionChanged);
		}

		public event EventHandler AddClicked
		{
			add { _addClickedEvent += value; }
			remove { _addClickedEvent -= value; }
		}

		public event EventHandler EditClicked
		{
			add { _editClickedEvent += value; }
			remove { _editClickedEvent -= value; }
		}

		public event EventHandler DeleteClicked
		{
			add { _deleteClickedEvent += value; }
			remove { _deleteClickedEvent -= value; }
		}

		protected bool AddEnabled
		{
			get { return _addButton.Enabled; }
			set
			{
				_addButton.Enabled = value;
				_addMenuItem.Enabled = value;
			}
		}

		protected bool EditEnabled
		{
			get { return _editButton.Enabled; }
			set
			{
				_editButton.Enabled = value;
				_editMenuItem.Enabled = value;
			}
		}

		protected bool DeleteEnabled
		{
			get { return _deleteButton.Enabled; }
			set
			{
				_deleteButton.Enabled = value;
				_deleteMenuItem.Enabled = value;
			}
		}

		private void AddToolStripButtons()
		{
			_addButton = new ToolStripButton("Add");
			_addButton.Click += new EventHandler(OnAddClicked);
			this.ToolStrip.Items.Add(_addButton);
			_editButton = new ToolStripButton("Edit");
			_editButton.Click += new EventHandler(OnEditClicked);
			this.ToolStrip.Items.Add(_editButton);
			_deleteButton = new ToolStripButton("Delete");
			_deleteButton.Click += new EventHandler(OnDeleteClicked);
			this.ToolStrip.Items.Add(_deleteButton);
		}

		private void AddContextMenuItems()
		{
			_addMenuItem = new ToolStripMenuItem("Add");
			_addMenuItem.Click += new EventHandler(OnAddClicked);
			this.ContextMenuStrip.Items.Add(_addMenuItem);
			_editMenuItem = new ToolStripMenuItem("Edit");
			_editMenuItem.Click += new EventHandler(OnEditClicked);
			this.ContextMenuStrip.Items.Add(_editMenuItem);
			_deleteMenuItem = new ToolStripMenuItem("Delete");
			_deleteMenuItem.Click += new EventHandler(OnDeleteClicked);
			this.ContextMenuStrip.Items.Add(_deleteMenuItem);
		}

		void OnSelectionChanged(object sender, EventArgs e)
		{
			if (this.Selection.Item != null)
			{
				this.EditEnabled = true;
				this.DeleteEnabled = true;
			}
			else
			{
				this.EditEnabled = false;
				this.DeleteEnabled = false;
			}
		}

		void OnAddClicked(object sender, EventArgs e)
		{
			EventsHelper.Fire(_addClickedEvent, this, EventArgs.Empty);
		}
		
		void OnEditClicked(object sender, EventArgs e)
		{
			EventsHelper.Fire(_editClickedEvent, this, EventArgs.Empty);
		}

		void OnDeleteClicked(object sender, EventArgs e)
		{
			EventsHelper.Fire(_deleteClickedEvent, this, EventArgs.Empty);
		}

		void OnItemDoubleClicked(object sender, EventArgs e)
		{
			OnEditClicked(sender, e);
		}
	}
}
