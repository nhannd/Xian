using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Dicom.Services.View.WinForms
{
    public partial class QueueDisplayControl : UserControl
    {
        #region Handcoded Members
        #region Private Members
        private QueueDisplayComponent _queueDisplay;
        #endregion
        #endregion
        public QueueDisplayControl()
        {
            InitializeComponent();
        }

        public QueueDisplayControl(QueueDisplayComponent component) : this()
        {
            _queueDisplay = component;

            _parcelTableView.DataSource = _queueDisplay.Parcels;
            _parcelTableView.SelectionChanged += new EventHandler(OnParcelTableViewSelectionChanged);
            // event handlers
            _abortButton.Click += delegate(object sender, EventArgs args)
            {
                _queueDisplay.AbortSendSelectedParcel();
            };

            _pauseButton.Click += delegate(object sender, EventArgs args)
            {
                _queueDisplay.PauseSendSelectedParcel();
            };

            _removeButton.Click += delegate(object sender, EventArgs args)
            {
                _queueDisplay.RemoveSelectedParcel();
            };

            _refreshButton.Click += delegate(object sender, EventArgs args)
            {
                _queueDisplay.Refresh();
            };
        }

        void OnParcelTableViewSelectionChanged(object source, EventArgs args)
        {
            _queueDisplay.SetSelection(_parcelTableView.CurrentSelection);
        }
    }
}
