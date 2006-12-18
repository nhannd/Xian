using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class ApplicationComponentUserControl : CustomUserControl
    {
        public ApplicationComponentUserControl()
        {
        }

        public ApplicationComponentUserControl(IApplicationComponent component)
        {
            InitializeComponent();

            _errorProvider.DataSource = component;
        }

        /// <summary>
        /// Gets the default <see cref="ErrorProvider"/> for this user control
        /// </summary>
        protected ValidationErrorProvider ErrorProvider
        {
            get { return _errorProvider; }
        }
    }
}
