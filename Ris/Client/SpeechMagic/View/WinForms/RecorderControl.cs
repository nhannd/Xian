using System;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public partial class RecorderControl : UserControl
    {
        private event EventHandler _recordButtonClicked;
        private event EventHandler _playButtonClicked;
        private event EventHandler _stopButtonClicked;
        private event EventHandler _rewindButtonClicked;
        private event EventHandler _forwardButtonClicked;

        public RecorderControl()
        {
            InitializeComponent();

            _recordButton.Enabled = true;
            _stopButton.Enabled = false;
        }

        #region Public Events

        public event EventHandler RecordButtonClicked
        {
            add { _recordButtonClicked += value; }
            remove { _recordButtonClicked -= value; }
        }

        public event EventHandler PlayButtonClicked
        {
            add { _playButtonClicked += value; }
            remove { _playButtonClicked -= value; }
        }

        public event EventHandler StopButtonClicked
        {
            add { _stopButtonClicked += value; }
            remove { _stopButtonClicked -= value; }
        }

        public event EventHandler RewindButtonClicked
        {
            add { _rewindButtonClicked += value; }
            remove { _rewindButtonClicked -= value; }
        }

        public event EventHandler ForwardButtonClicked
        {
            add { _forwardButtonClicked += value; }
            remove { _forwardButtonClicked -= value; }
        }

        #endregion

        private void _recordButton_Click(object sender, EventArgs e)
        {
            _recordButton.Enabled = false;
            _stopButton.Enabled = true;
            EventsHelper.Fire(_recordButtonClicked, this, EventArgs.Empty);
        }

        private void _playButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_playButtonClicked, this, EventArgs.Empty);
        }

        private void _stopButton_Click(object sender, EventArgs e)
        {
            _recordButton.Enabled = true;
            _stopButton.Enabled = false;
            EventsHelper.Fire(_stopButtonClicked, this, EventArgs.Empty);
        }

        private void _rewindButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_rewindButtonClicked, this, EventArgs.Empty);
        }

        private void _forwardButton_Click(object sender, EventArgs e)
        {
            EventsHelper.Fire(_forwardButtonClicked, this, EventArgs.Empty);
        }
    }
}
