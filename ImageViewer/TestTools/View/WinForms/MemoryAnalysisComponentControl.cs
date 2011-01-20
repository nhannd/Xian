#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="MemoryAnalysisComponent"/>.
    /// </summary>
    public partial class MemoryAnalysisComponentControl : ApplicationComponentUserControl
    {
        private MemoryAnalysisComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryAnalysisComponentControl(MemoryAnalysisComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

            BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

        	_memoryIncrement.DataBindings.Add("Value", _component, "MemoryIncrementKB", true,
        	                                   DataSourceUpdateMode.OnPropertyChanged);

        	_heapMemory.DataBindings.Add("Value", _component, "HeapMemoryKB", true, 
				DataSourceUpdateMode.OnPropertyChanged);

			_heldMemory.DataBindings.Add("Value", _component, "HeldMemoryKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_markedMemory.DataBindings.Add("Value", _component, "MemoryMarkKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_memoryDifference.DataBindings.Add("Value", _component, "MemoryDifferenceKB", true,
				DataSourceUpdateMode.OnPropertyChanged);

			_largeObjectMemory.DataBindings.Add("Value", _component, "TotalLargeObjectMemoryKB", true,
				DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _consumeMaxMemory_Click(object sender, EventArgs e)
		{
			_component.ConsumeMaximumMemory();
		}

		private void _consumeIncrement_Click(object sender, EventArgs e)
		{
			_component.AddHeldMemory();
		}

		private void _releaseHeldMemory_Click(object sender, EventArgs e)
		{
			_component.ReleaseHeldMemory();
		}

		private void _collect_Click(object sender, EventArgs e)
		{
			Cursor old = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			_component.Collect();
			Cursor.Current = old;
		}

		private void _markMemory_Click(object sender, EventArgs e)
		{
			_component.MarkMemory();
		}

		private void _unloadPixelData_Click(object sender, EventArgs e)
		{
			_component.UnloadPixelData();
		}
    }
}
