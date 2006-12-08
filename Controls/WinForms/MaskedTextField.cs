using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
    public partial class MaskedTextField : UserControl
    {
        public MaskedTextField()
        {
            InitializeComponent();
        }

        [Category("Masked Text Field")]
        public string Value
        {
            get 
            {
                return _nullableMaskedEdit.IsNull == false ? 
                    (string)_nullableMaskedEdit.Value : 
                    _nullableMaskedEdit.NullTextReturnValue; 
            } 
            set 
            { 
                _nullableMaskedEdit.Value = 
                    value == string.Empty ? null : value; 
            }
        }

        [Category("Masked Text Field")]
        public event EventHandler ValueChanged
        {
            add { _nullableMaskedEdit.TextChanged += value; }
            remove { _nullableMaskedEdit.TextChanged -= value; }
        }

        [Category("Masked Text Field")]
        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        /// <summary>
        /// Set/Get the text field mask.   See System.Windows.Forms.MaskedTextBox.Mask for details on setting the Mask value
        /// </summary>
        /// <seealso cref="System.Windows.Forms.MaskedTextBox.Mask"/>
        [Category("Masked Text Field")]
        [Description("See System.Windows.Forms.MaskedTextBox.Mask Property")]
        public string Mask
        {
            get { return _nullableMaskedEdit.EditMask; }
            set { _nullableMaskedEdit.EditMask = value; }
        }
    }
}
