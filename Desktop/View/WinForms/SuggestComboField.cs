#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using System.ComponentModel;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// A type of field that allows the user to select an item from a list of suggested items
    /// that is provided dynamically from a <see cref="ISuggestionProvider"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="SuggestionProvider"/> property must be set.  Also, the <see cref="Format"/> event
    /// should be handled to correctly format items for display.
    /// </remarks>
    public partial class SuggestComboField : UserControl
    {
        public SuggestComboField()
        {
            InitializeComponent();
        }

        #region Design-time properties and events

        /// <summary>
        /// Gets or sets the associated label text.
        /// </summary>
        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        /// <summary>
        /// Occurs to allow formatting of the item for display in the user-interface.
        /// </summary>
        public event ListControlConvertEventHandler Format
        {
            add { _comboBox.Format += value; }
            remove { _comboBox.Format -= value; }
        }

        #endregion

        [Browsable(false)]
        public object Value
        {
            get { return _comboBox.Value; }
            set { _comboBox.Value = value; }
        }

        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            // use pass through event subscription
            add { _comboBox.ValueChanged += value; }
            remove { _comboBox.ValueChanged -= value; }
        }

        /// <summary>
        /// Gets the current query text (the text that is in the edit portion of the control).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string QueryText
        {
            get { return _comboBox.Text; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISuggestionProvider"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISuggestionProvider SuggestionProvider
        {
            get { return _comboBox.SuggestionProvider; }
            set { _comboBox.SuggestionProvider = value; }
        }

    }
}
