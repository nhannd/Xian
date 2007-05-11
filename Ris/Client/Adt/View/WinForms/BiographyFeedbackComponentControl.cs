using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyFeedbackComponent"/>
    /// </summary>
    public partial class BiographyFeedbackComponentControl : ApplicationComponentUserControl
    {
        private BiographyFeedbackComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyFeedbackComponentControl(BiographyFeedbackComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

            this.Dock = DockStyle.Fill;

            _feedbackTable.Table = _component.Feedbacks;
            _feedbackTable.DataBindings.Add("Selection", _component, "SelectedFeedback", true, DataSourceUpdateMode.OnPropertyChanged);

            _subject.DataBindings.Add("Value", _component, "Subject", true, DataSourceUpdateMode.OnPropertyChanged);
            _comments.DataBindings.Add("Value", _component, "Comments", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
