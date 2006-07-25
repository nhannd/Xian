using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DialogBoxForm : Form
    {
        public DialogBoxForm(string title, IView view)
        {
            InitializeComponent();
            this.Text = title;

            Control content = (Control)view.GuiElement;
            content.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(content);
        }
    }
}