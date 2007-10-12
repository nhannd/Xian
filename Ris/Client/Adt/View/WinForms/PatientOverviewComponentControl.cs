#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientOverviewComponent"/>
    /// </summary>
    public partial class PatientOverviewComponentControl : ApplicationComponentUserControl
    {
        private PatientOverviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientOverviewComponentControl(PatientOverviewComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;
            
            _name.DataBindings.Add("Text", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _mrn.DataBindings.Add("Text", _component, "Mrn", true, DataSourceUpdateMode.OnPropertyChanged);
            _healthCard.DataBindings.Add("Text", _component, "HealthCard", true, DataSourceUpdateMode.OnPropertyChanged);
            _ageSex.DataBindings.Add("Text", _component, "AgeSex", true, DataSourceUpdateMode.OnPropertyChanged);
            _dateOfBirth.DataBindings.Add("Text", _component, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged);

            _picture.Image = IconFactory.CreateIcon(_component.PatientImage, _component.ResourceResolver);

            foreach (AlertListItem item in _component.AlertList)
            {
                _alertIcons.Images.Add(item.Icon, IconFactory.CreateIcon(item.Icon, _component.ResourceResolver));

                ListViewItem listItem = new ListViewItem();
                listItem.Name = item.Name;
                listItem.Text = item.Name;
                listItem.ImageKey = item.Icon;
                listItem.ToolTipText = item.Message;

                _alertList.Items.Add(listItem);
            }

        }
    }
}
