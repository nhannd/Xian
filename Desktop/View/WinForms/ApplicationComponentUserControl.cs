using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Base class for user controls that are created by an Application Component View.
    /// </summary>
    public partial class ApplicationComponentUserControl : CustomUserControl
    {
        /// <summary>
        /// Constructor required for Designer support.  Do not use this constructor in application code.
        /// </summary>
        public ApplicationComponentUserControl()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        public ApplicationComponentUserControl(IApplicationComponent component)
        {
            InitializeComponent();

            _errorProvider.DataSource = component;
            component.ValidationVisibleChanged += ValidationVisibleChangedEventHandler;
        }

        /// <summary>
        /// Gets the default <see cref="System.Windows.Forms.ErrorProvider"/> for this user control
        /// </summary>
        public ErrorProvider ErrorProvider
        {
            get { return _errorProvider; }
        }

        private void ValidationVisibleChangedEventHandler(object sender, EventArgs e)
        {
            _errorProvider.UpdateBinding();
        }
    }
}
