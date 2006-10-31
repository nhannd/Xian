using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DialogBoxForm : DotNetMagicForm
    {
        public DialogBoxForm(string title, IView view)
        {
            InitializeComponent();
            this.Text = title;

			this.Style = WinFormsView.VisualStyle;

            Control content = (Control)view.GuiElement;

            // important - if we do not set a minimum size, the full content may not be displayed
            content.MinimumSize = content.Size;
            content.Dock = DockStyle.Fill;

            // force the dialog client size to the size of the content
            this.ClientSize = content.Size;
            _contentPanel.Controls.Add(content);
        }
    }
}