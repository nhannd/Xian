using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StackComponentContainer"/>
    /// </summary>
    public partial class StackComponentContainerControl : CustomUserControl
    {
        private StackComponentContainer _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StackComponentContainerControl(StackComponentContainer component)
        {
            InitializeComponent();

            _component = component;
            _component.TopmostChanged += new EventHandler(TopmostChangedEventHandler);

            UpdateVisibleControl();
        }

        private void TopmostChangedEventHandler(object sender, EventArgs e)
        {
            UpdateVisibleControl();
        }

        private void UpdateVisibleControl()
        {
            Control control = null;
            if (_component.Topmost != null)
            {
                control = (Control)_component.Topmost.ComponentView.GuiElement;
                control.Dock = DockStyle.Fill;
                if (!this.Controls.Contains(control))
                {
                    this.Controls.Add(control);
                }
                control.Visible = true;
            }

            // hide all the other controls
            foreach (Control c in this.Controls)
            {
                if (c != control)
                    c.Visible = false;
            }
        }
    }
}
