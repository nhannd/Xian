using System;
using System.Windows.Forms;

using SmIa = Philips.PSP.SmIa;
using SmXAudioLib = Philips.PSP.SpeechMagic.SmXAudioLib;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public partial class DictationEditorControl : UserControl
    {
        public DictationEditorControl()
        {
            InitializeComponent();

            SmIaManager.Instance.CommandRecognized += _smIaManager_OnCommandRecognized;
            SmIaManager.Instance.AudioStateChanged += _smIaManager_OnAudioStateChanged;
            SmIaManager.Instance.StateChanged += _smIaManager_OnStateChanged;
            SmIaManager.Instance.SessionError += _smIaManager_OnSessionError;
            SmIaManager.Instance.SessionWarning += _smIaManager_OnSessionWarning;
            SmIaManager.Instance.RecognizerModeChanged += _smIaManager_OnRecognizerModeChanged;
            SmIaManager.Instance.SpeechMikeButtonPressed += Instance_SpeechMikeButtonPressed;
            SmIaManager.Instance.ProtectDocumentRequested += Instance_ProtectDocumentRequested;
            SmIaManager.Instance.PreviewReceived += Instance_PreviewReceived;

            this.Disposed += DictationEditorControl_Disposed;

            _recorderPanel.Enabled = false;
        }

        void DictationEditorControl_Disposed(object sender, EventArgs e)
        {
            SmIaManager.Instance.PrintInfo("Disposing editor");
            CloseSpeechMagicSession();

            SmIaManager.Instance.CommandRecognized -= _smIaManager_OnCommandRecognized;
            SmIaManager.Instance.AudioStateChanged -= _smIaManager_OnAudioStateChanged;
            SmIaManager.Instance.StateChanged -= _smIaManager_OnStateChanged;
            SmIaManager.Instance.SessionError -= _smIaManager_OnSessionError;
            SmIaManager.Instance.SessionWarning -= _smIaManager_OnSessionWarning;
            SmIaManager.Instance.RecognizerModeChanged -= _smIaManager_OnRecognizerModeChanged;
            SmIaManager.Instance.ProtectDocumentRequested -= Instance_ProtectDocumentRequested;
        }

        #region SmIaManager Events

        void _smIaManager_OnSessionWarning(int errorCode)
        {
            // TODO: Handles warning
        }

        void _smIaManager_OnSessionError(int errorCode)
        {
            // TODO: Handles Error
        }

        void _smIaManager_OnStateChanged(State state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StateChangedCallback(_smIaManager_OnStateChanged), new object[] { state });
                return;
            }

            _appState.Text = string.Format("App State: {0}", state);

            UpdateEnableStatus(state);
        }

        void _smIaManager_OnAudioStateChanged(SmIa.AudioState state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AudioStateChangedCallback(_smIaManager_OnAudioStateChanged), new object[] { state });
                return;
            }

            _audioState.Text = string.Format("Audio State: {0}", state);
        }

        void _smIaManager_OnRecognizerModeChanged(RecognizerType type)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new RecognizerModeChangedCallback(_smIaManager_OnRecognizerModeChanged), new object[] { type });
                return;
            }

            _recognizerMode.Text = string.Format("Recognizer Mode: {0}", type);
        }

        private void _smIaManager_OnCommandRecognized(
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

        void Instance_SpeechMikeButtonPressed(SmXAudioLib.SmXAudioControlDeviceEvent controlDeviceEvent)
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

        private void Instance_ProtectDocumentRequested(bool protect)
        {
            // TODO: Protect Document
            // Whenever Protect document is requested, the component should not be closed
        }

        private void Instance_PreviewReceived(string preview)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new PreviewReceivedCallback(Instance_PreviewReceived), new object[] { preview });
                return;
            }

            _preview.Text = preview;
        }

        #endregion SmIaManager Events

        #region UI Events

        private void SpeechMagicControl_Load(object sender, EventArgs e)
        {
            SmIaManager.Instance.SetActiveDocument(_richTextBox.Handle);

            // Start the session asynchronously
            BackgroundTask startSessionTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        OpenSpeechMagicSession(true);
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

        #endregion Recorder Events

        private void OpenSpeechMagicSession(bool openAsAuthor)
        {
            if (openAsAuthor)
            {
                SmIaManager.Instance.OpenAsAuthor();
                _documentEditMode.Text = "Author Mode";
            }
            else
            {
                SmIaManager.Instance.OpenAsCorrectionist();
                _documentEditMode.Text = "Revision Mode";
            }
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
            UpdateVolumeMeter(0);
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

        private void UpdateEnableStatus(State state)
        {
            _recorderPanel.Enabled = state != State.Uninitialized;                

            switch (state)
            {
                case State.Uninitialized:
                    _trackBar.Enabled = false;
                    break;
                case State.Idle:
                case State.Initialized:
                    _recordButton.Enabled = true;
                    _playButton.Enabled = true;
                    _stopButton.Enabled = false;
                    _rewindButton.Enabled = true;
                    _forwardButton.Enabled = true;
                    _trackBar.Enabled = true;
                    break;
                case State.Recording:
                    _recordButton.Enabled = false;
                    _playButton.Enabled = false;
                    _stopButton.Enabled = true;
                    _rewindButton.Enabled = false;
                    _forwardButton.Enabled = false;
                    _trackBar.Enabled = false;
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
            _recordedTime.Text = String.Format("{0} of {1} ms", value/1000.0, maximum/1000.0);
        }

        private void UpdateVolumeMeter(int vol)
        {
            vol = vol - SmIaManager.Instance.SettingVorLevel;
            if (vol < 0)
                vol = 0;

            _volumeMeter.Value = vol;
        }
    }
}
