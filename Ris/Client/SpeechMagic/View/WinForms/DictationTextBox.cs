using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using SmIa = Philips.PSP.SmIa;
using SmXAudioLib = Philips.PSP.SpeechMagic.SmXAudioLib;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public partial class DictationTextBox : UserControl
    {
        private bool _sessionStarted;
        private bool _controlDisposed;

        public DictationTextBox()
        {
            InitializeComponent();
        }

        private void SpeechMagicControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            UpdateApplicationState(State.Uninitialized);
            UpdateAudioState(SmIa.AudioState.astStop);
            UpdateRecognizerMode(RecognizerType.None);
            UpdateRecordedTime(0, 0);
            UpdatePreview("");

            SmIaManager.Instance.SetActiveDocument(_richTextBox.Handle);

            SmIaManager.Instance.SessionWarning += OnSessionWarning;
            SmIaManager.Instance.SessionError += OnSessionError;
            SmIaManager.Instance.StateChanged += OnStateChanged;
            SmIaManager.Instance.AudioStateChanged += OnAudioStateChanged;
            SmIaManager.Instance.RecognizerModeChanged += OnRecognizerModeChanged;
            SmIaManager.Instance.PreviewReceived += OnPreviewReceived;
            SmIaManager.Instance.SpeechMikeButtonPressed += OnSpeechMikeButtonPressed;
            SmIaManager.Instance.ProtectDocumentRequested += OnProtectDocumentRequested;
            SmIaManager.Instance.CommandRecognized += OnCommandRecognized;

            this.Disposed += DictationTextBox_Disposed;

            // Start the session asynchronously
            BackgroundTask startSessionTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        OpenSpeechMagicSession(true);
                        _sessionStarted = true;

                        // User closes the control while the session is being started
                        if (_controlDisposed)
                            CloseSpeechMagicSession();
                        
                        taskContext.Complete();
                    }
                    catch (Exception ex)
                    {
                        taskContext.Error(ex);
                    }
                }, false);

            startSessionTask.Terminated +=
                delegate
                {
                    // dispose of the task
                    startSessionTask.Dispose();
                    startSessionTask = null;
                };

            startSessionTask.Run();
        }

        private void DictationTextBox_Disposed(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            _controlDisposed = true;

            if (_sessionStarted)
                CloseSpeechMagicSession();

            SmIaManager.Instance.SessionWarning -= OnSessionWarning;
            SmIaManager.Instance.SessionError -= OnSessionError;
            SmIaManager.Instance.StateChanged -= OnStateChanged;
            SmIaManager.Instance.AudioStateChanged -= OnAudioStateChanged;
            SmIaManager.Instance.RecognizerModeChanged -= OnRecognizerModeChanged;
            SmIaManager.Instance.PreviewReceived -= OnPreviewReceived;
            SmIaManager.Instance.SpeechMikeButtonPressed -= OnSpeechMikeButtonPressed;
            SmIaManager.Instance.ProtectDocumentRequested -= OnProtectDocumentRequested;
            SmIaManager.Instance.CommandRecognized -= OnCommandRecognized;
        }

        public string Value
        {
            get { return _richTextBox.Text; }
            set { _richTextBox.Text = value; }
        }

        public event EventHandler ValueChanged
        {
            add { _richTextBox.TextChanged += value; }
            remove { _richTextBox.TextChanged -= value; }
        }

        #region SmIaManager Events
        private void OnSessionWarning(int errorCode)
        {
            // TODO: Handles warning
        }
        private void OnSessionError(int errorCode)
        {
            // TODO: Handles Error
        }
        private void OnStateChanged(State state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StateChangedCallback(OnStateChanged), new object[] { state });
                return;
            }

            UpdateApplicationState(state);
        }
        private void OnAudioStateChanged(SmIa.AudioState state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AudioStateChangedCallback(OnAudioStateChanged), new object[] { state });
                return;
            }

            UpdateAudioState(state);
        }
        private void OnRecognizerModeChanged(RecognizerType type)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RecognizerModeChangedCallback(OnRecognizerModeChanged), new object[] { type });
                return;
            }

            UpdateRecognizerMode(type);
        }
        private void OnPreviewReceived(string preview)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PreviewReceivedCallback(OnPreviewReceived), new object[] { preview });
                return;
            }

            UpdatePreview(preview);
        }
        private void OnSpeechMikeButtonPressed(SmXAudioLib.SmXAudioControlDeviceEvent controlDeviceEvent)
        {
            switch (controlDeviceEvent)
            {
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastForwardPressed:
                    Forward();
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastRewindPressed:
                    Rewind();
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayPressed:
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayStopTogglePressed:
                    Play();
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudRecordPressed:
                    Record();
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastForwardReleased:
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastRewindReleased:
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudStopPressed:
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayStopToggleReleased:
                    Stop();
                    break;
            }
        }
        private void OnProtectDocumentRequested(bool protect)
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new ProtectDocumentRequestedCallback(Instance_ProtectDocumentRequested), new object[] { protect });
            //    return;
            //}

            // TODO: Protect Document
            // Whenever Protect document is requested, the component should not be closed
        }
        private void OnCommandRecognized(
            string grammar,
            string symbol,
            string commandText,
            double confidence,
            ref SmIa.SSemanticAttribute[] semanticAttributes,
            ref string[] textNonterminalTexts,
            SmIa.ICommandManipulation commandManipulator)
        {
            // TODO: Handle Commands recognized
        }
        #endregion SmIaManager Events

        #region Timer Events
        private void _trackBarTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(_trackBarTimer_Tick), null);
                return;
            }

            UpdateTrackBar(SmIaManager.Instance.AudioPosition, SmIaManager.Instance.AudioFileLength);
        }
        private void _volumeTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(_volumeTimer_Tick), null);
                return;
            }

            UpdateVolumeMeter(SmIaManager.Instance.AudioSignalLevel);
        }
        #endregion Timer Events

        #region Control & Button Events
        private void _recordButton_Click(object sender, EventArgs e)
        {
            Record();
        }
        private void _playButton_Click(object sender, EventArgs e)
        {
            Play();
        }
        private void _stopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void _rewindButton_Click(object sender, EventArgs e)
        {
            Rewind();
        }
        private void _forwardButton_Click(object sender, EventArgs e)
        {
            Forward();
        }
        private void _trackBar_Scroll(object sender, EventArgs e)
        {
            SmIaManager.Instance.AudioPosition = _trackBar.Value;
            UpdateRecordedTime(_trackBar.Value, _trackBar.Maximum);
        }
        #endregion Control & Button Events

        private void OpenSpeechMagicSession(bool openAsAuthor)
        {
            if (openAsAuthor)
                SmIaManager.Instance.OpenAsAuthor();
            else
                SmIaManager.Instance.OpenAsCorrectionist();

            UpdateDocumentEditMode(openAsAuthor);
        }
        private void CloseSpeechMagicSession()
        {
            SmIaManager.Instance.Close();
        }
        private void Record()
        {
            _volumeTimer.Start();
            _trackBarTimer.Start();
            SmIaManager.Instance.Record();
        }
        private void Play()
        {
            _volumeTimer.Start();
            _trackBarTimer.Start();
            SmIaManager.Instance.Play();
        }
        private void Stop()
        {
            SmIaManager.Instance.Stop();
            _volumeTimer.Stop();
            _trackBarTimer.Stop();
        }
        private void Rewind()
        {
            _trackBarTimer.Start();
            SmIaManager.Instance.Rewind();
        }
        private void Forward()
        {
            _trackBarTimer.Start();
            SmIaManager.Instance.Forward();
        }

        private void UpdateApplicationState(State state)
        {
            _appState.Text = state.ToString();
            _appState.ToolTipText = "Application State";

            _recorderPanel.Enabled = state != State.Uninitialized;
            _trackBar.Enabled = state == State.Idle || state == State.Initialized;

            switch (state)
            {
                case State.Uninitialized:
                    break;
                case State.Idle:
                case State.Initialized:
                    _recordButton.Enabled = true;
                    _playButton.Enabled = true;
                    _stopButton.Enabled = false;
                    _rewindButton.Enabled = true;
                    _forwardButton.Enabled = true;
                    break;
                case State.Recording:
                    _recordButton.Enabled = false;
                    _playButton.Enabled = false;
                    _stopButton.Enabled = true;
                    _rewindButton.Enabled = false;
                    _forwardButton.Enabled = false;
                    break;
                case State.Playing:
                    _recordButton.Enabled = false;
                    _playButton.Enabled = false;
                    _stopButton.Enabled = true;
                    _rewindButton.Enabled = false;
                    _forwardButton.Enabled = false;
                    break;
                case State.Stopping:
                    _recordButton.Enabled = false;
                    _playButton.Enabled = false;
                    _stopButton.Enabled = false;
                    _rewindButton.Enabled = false;
                    _forwardButton.Enabled = false;
                    break;
                case State.Winding:
                    _recordButton.Enabled = false;
                    _playButton.Enabled = false;
                    _stopButton.Enabled = true;
                    _rewindButton.Enabled = false;
                    _forwardButton.Enabled = false;
                    break;
                default:
                    break;
            }
        }
        private void UpdateTrackBar(int value, int maximum)
        {
            _trackBar.Maximum = maximum;
            _trackBar.Value = value;
            UpdateRecordedTime(value, maximum);
        }
        private void UpdateRecordedTime(int value, int maximum)
        {
            _recordedTime.Text = String.Format("{0} of {1} s", value/1000.0, maximum/1000.0);
        }
        private void UpdateVolumeMeter(int vol)
        {
            vol = vol - SmIaManager.Instance.SettingVorLevel;
            if (vol < 0)
                vol = 0;

            _volumeMeter.Value = vol;
        }
        private void UpdateDocumentEditMode(bool openAsAuthor)
        {
            if (openAsAuthor)
            {
                _documentEditMode.Image = SR.Author.ToBitmap();
                _documentEditMode.Text = "";
                _documentEditMode.ToolTipText = "Author Mode";
            }
            else
            {
                _documentEditMode.Image = SR.Typist.ToBitmap();
                _documentEditMode.Text = "";
                _documentEditMode.ToolTipText = "Revision Mode";
            }
        }
        private void UpdateAudioState(SmIa.AudioState state)
        {
            Icon audioIcon;
            string audioState;
            switch (state)
            {
                case SmIa.AudioState.astVORStop:
                    audioIcon = SR.Paused;
                    audioState = "Paused";
                    break;
                case SmIa.AudioState.astStop:
                    audioIcon = SR.Stopped;
                    audioState = "Stopped";
                    UpdateVolumeMeter(0);
                    break;
                case SmIa.AudioState.astRecord:
                    audioIcon = SR.Recording;
                    audioState = "Recording";
                    break;
                case SmIa.AudioState.astPlayback:
                    audioIcon = SR.Playing;
                    audioState = "Playing";
                    break;
                case SmIa.AudioState.astFastRewind:
                    audioIcon = SR.Rewinding;
                    audioState = "Rewinding";
                    break;
                case SmIa.AudioState.astFastForward:
                    audioIcon = SR.Forwarding;
                    audioState = "Forwarding";
                    break;
                case SmIa.AudioState.astInvalid:
                default:
                    audioIcon = SR.Invalid;
                    audioState = "Invalid";
                    break;
            }

            _audioState.Image = audioIcon.ToBitmap();
            _audioState.Text = "";
            _audioState.ToolTipText = string.Format("Audio State: {0}", audioState);
            
        }
        private void UpdateRecognizerMode(RecognizerType type)
        {
            switch (type)
            {
                case RecognizerType.Dictation:
                    _recognizerMode.ForeColor = Color.Red;
                    break;
                case RecognizerType.Command:
                    _recognizerMode.ForeColor = Color.Green;
                    break;
                case RecognizerType.Spelling:
                    _recognizerMode.ForeColor = Color.Orange;
                    break;
                case RecognizerType.None:
                default:
                    break;
            }

            _recognizerMode.Text = type.ToString();
            _recognizerMode.ToolTipText = "Recognizer Mode";            
        }
        private void UpdatePreview(string preview)
        {
            _preview.Text = preview;
        }
    }
}
