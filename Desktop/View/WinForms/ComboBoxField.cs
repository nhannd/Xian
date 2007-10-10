using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class ComboBoxField : UserControl
    {
        public ComboBoxField()
        {
            InitializeComponent();
        }

        public string LabelText
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public ComboBoxStyle DropDownStyle
        {
            get { return _comboBox.DropDownStyle; }
            set { _comboBox.DropDownStyle = value; }
        }

        public object Value
        {
            get { return _comboBox.SelectedItem; }
            set { _comboBox.SelectedItem = value; }
        }

        public event EventHandler ValueChanged
        {
            // use pass through event subscription
            add { _comboBox.SelectedIndexChanged += value; }
            remove { _comboBox.SelectedIndexChanged -= value; }
        }

        public object DataSource
        {
            get { return _comboBox.DataSource; }
            set { _comboBox.DataSource = value; }
        }

		public string DisplayMember
		{
			get { return _comboBox.DisplayMember; }
			set { _comboBox.DisplayMember = value; }
		}
    }
}
