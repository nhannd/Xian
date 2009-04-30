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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class SearchComponentControl : CustomUserControl
    {
        private readonly SearchComponent _component;

        public SearchComponentControl(SearchComponent component)
        {
            InitializeComponent();
            _component = component;

            _accession.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _accession.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Value", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Value", _component, "HealthcardNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthcard.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _familyName.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _fromDate.DataBindings.Add("Value", _component, "FromDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _fromDate.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _untilDate.DataBindings.Add("Value", _component, "UntilDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _untilDate.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _orderingPractitioner.DataBindings.Add("Value", _component, "OrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitioner.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;

            _procedureType.DataBindings.Add("Value", _component, "ProcedureType", true, DataSourceUpdateMode.OnPropertyChanged);
            _procedureType.DataBindings.Add("Enabled", _component, "ComponentEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _procedureType.LookupHandler = _component.ProcedureTypeLookupHandler;

            _keepOpen.DataBindings.Add("Checked", _component, "KeepOpen", true, DataSourceUpdateMode.OnPropertyChanged);
            _searchButton.DataBindings.Add("Enabled", _component, "SearchEnabled");
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_component.Clear();
		}
    }
}
