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
        private DocumentEditMode _editMode;
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

            // Set the control initial state
            UpdateApplicationState(State.Uninitialized);
            UpdateAudioState(SmIa.AudioState.astStop);
            UpdateRecognizerMode(RecognizerType.None);
            UpdateRecordedTime(0, 0);
            UpdatePreview("");

            // Set the SM active document to point to editor handle 
            // Once set, if the particular window is not in focus, the recognized speech should continue to update the editor
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

            // Since there is no 'unload' method, we resort to using this
            this.Disposed += DictationTextBox_Disposed;

            // Start the session asynchronously
            BackgroundTask startSessionTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        OpenSpeechMagicSession();
                        _sessionStarted = true;

                        // If user closes the control while the session is being started
                        // close the session now
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

            // Detach events.  Make sure this is matched up with the load event
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

        #region Public Properties
        public DocumentEditMode EditMode
        {
            get { return _editMode; }
            set { _editMode = value; }
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
        #endregion Design Properties

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
            // User should not be allow to edit the text either
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

        private void OpenSpeechMagicSession()
        {
            SmIaManager.Instance.Open(_editMode);
            UpdateDocumentEditMode(_editMode);
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
            _volumeTimer.Start();
            _trackBarTimer.Start();
            SmIaManager.Instance.Rewind();
        }
        private void Forward()
        {
            _volumeTimer.Start();
            _trackBarTimer.Start();
            SmIaManager.Instance.Forward();
        }

        private void UpdateApplicationState(State state)
        {
            _appState.Text = state.ToString();
            _appState.ToolTipText = "Application State";

            _trackBar.Enabled = state != State.Uninitialized && (state == State.Idle || state == State.Initialized);

            // Recorder button status
            _recorderPanel.Enabled = state != State.Uninitialized;

            _recordButton.Enabled = _playButton.Enabled = _rewindButton.Enabled = _forwardButton.Enabled
                = (state == State.Idle || state == State.Initialized);

            _stopButton.Enabled = (state == State.Recording || state == State.Playing);
        }
        private void UpdateTrackBar(int value, int maximum)
        {
            _trackBar.Value = value;
            _trackBar.Maximum = maximum;
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
        private void UpdateDocumentEditMode(DocumentEditMode mode)
        {
            if (mode == DocumentEditMode.Author)
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
