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

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="StaffSelectionComponent"/>.
	/// </summary>
	public partial class StaffSelectionComponentControl : ApplicationComponentUserControl
	{
		private readonly StaffSelectionComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public StaffSelectionComponentControl(StaffSelectionComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_staff.LookupHandler = _component.StaffLookupHandler;
			_staff.DataBindings.Add("Value", _component, "Staff", true, DataSourceUpdateMode.OnPropertyChanged);
			_staff.DataBindings.Add("LabelText", _component, "LabelText", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, System.EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}
