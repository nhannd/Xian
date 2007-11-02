using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// A type of field that allows the user to select from a potentially large list of items.
    /// </summary>
    /// <remarks>
    /// This type of field is useful when the list of items to choose from is too large to fit into a
    /// regular drop-down.  This type of field allows the user to choose an item by entering a text query,
    /// such as the first few letters of a name, and then clicking a button to launch an auxilliary dialog
    /// to resolve the query.  Alternatively, the user may resolve the query from a dynamically generated
    /// list of suggested items (this field makes use of the <see cref="SuggestComboField"/>).  The
    /// <see cref="LookupHandler"/> property must be set, as all user-interaction with the control will
    /// be directed to an underlying <see cref="ILookupHandler"/>.
    /// </remarks>
    public partial class LookupField : UserControl
    {
        private ILookupHandler _lookupHandler;

        public LookupField()
        {
            InitializeComponent();
        }

        #region Design-time properties

        public string LabelText
        {
            get { return _inputField.LabelText; }
            set { _inputField.LabelText = value; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        [Browsable(false)]
        public object Value
        {
            get { return _inputField.Value; }
            set { _inputField.Value = value; }
        }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property changes.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            // use pass through event subscription
            add { _inputField.ValueChanged += value; }
            remove { _inputField.ValueChanged -= value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ILookupHandler"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILookupHandler LookupHandler
        {
            get { return _lookupHandler; }
            set
            {
                _lookupHandler = value;
                _inputField.SuggestionProvider = _lookupHandler.SuggestionProvider;
            }
        }

        private void _findButton_Click(object sender, EventArgs e)
        {
            object result;
            bool resolved = _lookupHandler.Resolve(_inputField.QueryText, true, out result);
            if(resolved)
            {
                this.Value = result;
            }
        }

        private void _inputField_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = _lookupHandler.FormatItem(e.ListItem);
        }
    }
}
