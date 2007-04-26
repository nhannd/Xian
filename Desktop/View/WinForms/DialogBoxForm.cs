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
        private Control _content;

        public DialogBoxForm(string title, IView view)
        {
            InitializeComponent();
            this.Text = title;

            _content = (Control)view.GuiElement;

            // important - if we do not set a minimum size, the full content may not be displayed
            _content.MinimumSize = _content.Size;
            _content.Dock = DockStyle.Fill;

            // force the dialog client size to the size of the content
            this.ClientSize = _content.Size;
            _contentPanel.Controls.Add(_content);

            // Resize the dialog if size of the underlying content changed
            _content.SizeChanged += new EventHandler(OnContentSizeChanged);
        }

        private void OnContentSizeChanged(object sender, EventArgs e)
        {
            this.ClientSize = _content.Size;
        }
    }
}