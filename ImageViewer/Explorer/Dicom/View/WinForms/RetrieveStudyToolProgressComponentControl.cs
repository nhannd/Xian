using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RetrieveStudyToolProgressComponent"/>
    /// </summary>
    public partial class RetrieveStudyToolProgressComponentControl : UserControl
    {
        private RetrieveStudyToolProgressComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RetrieveStudyToolProgressComponentControl(RetrieveStudyToolProgressComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            BindingSource bs = new BindingSource();
            bs.DataSource = _component;

            _aeTitle.DataBindings.Add("Text", bs, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Text", bs, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            _host.DataBindings.Add("Text", bs, "Host", true, DataSourceUpdateMode.OnPropertyChanged);
            _patient.DataBindings.Add("Text", bs, "Patient", true, DataSourceUpdateMode.OnPropertyChanged);
            _port.DataBindings.Add("Text", bs, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            _progressGroupBox.DataBindings.Add("Text", bs, "ProgressGroupBox", true, DataSourceUpdateMode.OnPropertyChanged);
            _retrieveProgressBar.DataBindings.Add("Value", bs, "ProgressBar", true, DataSourceUpdateMode.OnPropertyChanged);
            _retrieveProgressBar.DataBindings.Add("Minimum", bs, "ProgressBarMinimum", true, DataSourceUpdateMode.OnPropertyChanged);
            _retrieveProgressBar.DataBindings.Add("Maximum", bs, "ProgressBarMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
            _studyDate.DataBindings.Add("Text", bs, "StudyDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _retrieveSourceGroupBox.DataBindings.Add("Text", bs, "RetrieveSourceGroupBox", true, DataSourceUpdateMode.OnPropertyChanged);
            _studyDescriptionGroupBox.DataBindings.Add("Text", bs, "StudyDescriptionGroupBox", true, DataSourceUpdateMode.OnPropertyChanged);
            _progressToolTip.SetToolTip(_progressGroupBox, " ");
            _progressToolTip.SetToolTip(_retrieveProgressBar, " ");
            _progressToolTip.ShowAlways = true;
            _progressToolTip.ReshowDelay = 1;
            _progressToolTip.Popup += 
                delegate(object source, PopupEventArgs e)
                {
                    _progressToolTip.ToolTipTitle = _component.ProgressDetails;
                };            
        }

    }
}
