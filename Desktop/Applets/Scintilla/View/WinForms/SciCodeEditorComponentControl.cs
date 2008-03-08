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
        private ScintillaNet.Scintilla _scintilla;

        /// <summary>
        /// Constructor
        /// </summary>
        public SciCodeEditorComponentControl(SciCodeEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component.PropertyChanged += new PropertyChangedEventHandler(_component_PropertyChanged);
            _component.InsertTextRequested += new EventHandler<SciCodeEditorComponent.InsertTextEventArgs>(_component_InsertTextRequested);

            // Scintilla control does not seem to get along with the designer very well,
            // so do everything manually
            // when creating the control, need to specify the full path to the native DLL, otherwise it will not find it
            _scintilla = new ScintillaNet.Scintilla(System.IO.Path.Combine(Platform.CommonDirectory, ScintillaNet.Scintilla.DefaultDllName));
            _scintilla.Dock = DockStyle.Fill;
            this.Controls.Add(_scintilla);

            // set the margin wide enough to display line numbers (set it to zero to hide line numbers)
            _scintilla.Margins.Margin0.Width = 35;

            // scintilla control "Text" property does not work with data-binding, because it does not fire the TextChanged event
            // therefore need to subscribe manually to these two events
            _scintilla.TextInserted += new EventHandler<TextModifiedEventArgs>(_scintilla_TextInserted);
            _scintilla.TextDeleted += new EventHandler<TextModifiedEventArgs>(_scintilla_TextDeleted);

            if (_component.Language != null)
            {
                _scintilla.ConfigurationManager.Language = _component.Language;
            }
            _scintilla.Text = _component.Text;
        }

        private void _scintilla_TextDeleted(object sender, TextModifiedEventArgs e)
        {
            _component.Text = _scintilla.Text;
        }

        private void _scintilla_TextInserted(object sender, TextModifiedEventArgs e)
        {
            _component.Text = _scintilla.Text;
        }

        private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                _scintilla.ConfigurationManager.Language = _component.Language;
            }
            else if (e.PropertyName == "Text")
            {
                // setting _scintilla.Text causes its cursor to move to the beginning of the document
                // therefore only do it if the text in the _component was changed programatically
                if (_scintilla.Text != _component.Text)
                {
                    _scintilla.Text = _component.Text;
                }
            }
        }

        private void _component_InsertTextRequested(object sender, SciCodeEditorComponent.InsertTextEventArgs e)
        {
            _scintilla.Selection.Text = e.Text;
        }

    }
}
