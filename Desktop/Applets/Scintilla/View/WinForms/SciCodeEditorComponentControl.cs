using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ScintillaNet;
using ClearCanvas.Common;

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
            // when creating the control, need to specify the full path to the native DLL, otherwise it will not find it
            _editor = new ScintillaNet.Scintilla(System.IO.Path.Combine(Platform.CommonDirectory, ScintillaNet.Scintilla.DefaultDllName));
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
