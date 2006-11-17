using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    public partial class DicomEditorCreateToolComponentControl : CustomUserControl
    {
        private DicomEditorCreateToolComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomEditorCreateToolComponentControl(DicomEditorCreateToolComponent component)
        {
            InitializeComponent();

            _component = component;

            _group.DataBindings.Add("Text", _component, "Group", true, DataSourceUpdateMode.OnPropertyChanged);
            _element.DataBindings.Add("Text", _component, "Element", true, DataSourceUpdateMode.OnPropertyChanged);
            _tagName.DataBindings.Add("Text", _component, "TagName", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataSource = _component.VrList;
            _vr.DataBindings.Add("Value", _component, "Vr", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Enabled", _component, "VrEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _value.DataBindings.Add("value", _component, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
