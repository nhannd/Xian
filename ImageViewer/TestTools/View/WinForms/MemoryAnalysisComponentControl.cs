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
