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
