using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ScintillaNet;

namespace ClearCanvas.Desktop.Applets.Scintilla.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SciCodeEditorComponent"/>
    /// </summary>
    public partial class SciCodeEditorComponentControl : ApplicationComponentUserControl
    {
        private SciCodeEditorComponent _component;
        private ScintillaNet.Scintilla _editor;

        /// <summary>
        /// Constructor
        /// </summary>
        public SciCodeEditorComponentControl(SciCodeEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component.PropertyChanged += new PropertyChangedEventHandler(_component_PropertyChanged);

            // Scintilla control does not seem to get along with the designer very well,
            // so do everything manually

            _editor = new ScintillaNet.Scintilla();
            _editor.Dock = DockStyle.Fill;
            this.Controls.Add(_editor);
            _editor.ConfigurationManager.Language = _component.Language;
        }

        private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                _editor.ConfigurationManager.Language = _component.Language;
            }
        }

        public ScintillaNet.Scintilla Editor
        {
            get { return _editor; }
        }
    }
}
