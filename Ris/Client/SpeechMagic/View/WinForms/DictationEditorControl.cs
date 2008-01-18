using System;
using System.Windows.Forms;

using SmIa = Philips.PSP.SmIa;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public partial class DictationEditorControl : UserControl
    {
        public DictationEditorControl()
        {
            InitializeComponent();

            _initializeButton.Enabled = true;
            _uninitializeButon.Enabled = false;
            _recorder.Enabled = false;

            SmIaManager.Instance.OnCommandRecognized += _smIaManager_OnCommandRecognized;
            SmIaManager.Instance.OnAudioStateChanged += _smIaManager_OnAudioStateChanged;
            SmIaManager.Instance.OnStateChanged += _smIaManager_OnStateChanged;
            SmIaManager.Instance.OnSessionError += _smIaManager_OnSessionError;
            SmIaManager.Instance.OnSessionWarning += _smIaManager_OnSessionWarning;
            SmIaManager.Instance.OnRecognizerModeChanged += _smIaManager_OnRecognizerModeChanged;

            this.Disposed += DictationEditorControl_Disposed;
        }

        void DictationEditorControl_Disposed(object sender, EventArgs e)
        {
            SmIaManager.Instance.OnCommandRecognized -= _smIaManager_OnCommandRecognized;
            SmIaManager.Instance.OnAudioStateChanged -= _smIaManager_OnAudioStateChanged;
            SmIaManager.Instance.OnStateChanged -= _smIaManager_OnStateChanged;
            SmIaManager.Instance.OnSessionError -= _smIaManager_OnSessionError;
            SmIaManager.Instance.OnSessionWarning -= _smIaManager_OnSessionWarning;
            SmIaManager.Instance.OnRecognizerModeChanged -= _smIaManager_OnRecognizerModeChanged;
        }

        private void SpeechMagicControl_Load(object sender, EventArgs e)
        {
            _recorder.RecordButtonClicked += _recorder_RecordButtonClicked;
            _recorder.PlayButtonClicked += _recorder_PlayButtonClicked;
            _recorder.StopButtonClicked += _recorder_StopButtonClicked;
            _recorder.RewindButtonClicked += _recorder_RewindButtonClicked;
            _recorder.ForwardButtonClicked += _recorder_ForwardButtonClicked;
        }

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

            SmIaManager.Instance.SetDocumentEditor(_richTextBox.Handle);
        }

        private void CloseSpeechMagicSession()
        {
            SmIaManager.Instance.Close();
        }

        #region SmIaManager Events

        void _smIaManager_OnSessionWarning(int errorCode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void _smIaManager_OnSessionError(int errorCode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void _smIaManager_OnStateChanged(State state)
        {
            _appState.Text = string.Format("App State: {0}", state);
        }

        void _smIaManager_OnAudioStateChanged(SmIa.AudioState state)
        {
            _audioState.Text = string.Format("Audio State: {0}", state);
        }

        void _smIaManager_OnRecognizerModeChanged(RecognizerType type)
        {
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
            // TODO: do something when commands are recognized
        }

        #endregion SmIaManager Events

        #region Volume Meter Events

        private void _volumeTimer_Tick(object sender, EventArgs e)
        {
            SetVolume(SmIaManager.Instance.AudioSignalLevel);
        }

        private void SetVolume(int vol)
        {
            vol = vol - SmIaManager.Instance.SettingVorLevel;
            if (vol < 0)
                vol = 0;

            _volumeMeter.Value = vol;
        }

       
        #endregion

        #region Recorder Events

        private void _recorder_RecordButtonClicked(object sender, EventArgs e)
        {
            _uninitializeButon.Enabled = false;
            _volumeTimer.Start();
            SmIaManager.Instance.Record();
        }

        private void _recorder_PlayButtonClicked(object sender, EventArgs e)
        {
            _volumeTimer.Start();
            SmIaManager.Instance.Play();
        }

        private void _recorder_StopButtonClicked(object sender, EventArgs e)
        {
            SmIaManager.Instance.Stop();
            _volumeTimer.Stop();
            SetVolume(0);
            _uninitializeButon.Enabled = true;
        }

        private void _recorder_RewindButtonClicked(object sender, EventArgs e)
        {
            SmIaManager.Instance.Rewind();
        }

        private void _recorder_ForwardButtonClicked(object sender, EventArgs e)
        {
            SmIaManager.Instance.Forward();
        }

        #endregion Recorder Events

        #region Debugging Methods

        private void _initializeButton_Click(object sender, EventArgs e)
        {
            Cursor oldMPointer = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _initializeButton.Enabled = false;
                _uninitializeButon.Enabled = true;
                _recorder.Enabled = true;

                OpenSpeechMagicSession(true);
            }
            catch (Exception)
            {
                Cursor.Current = oldMPointer;
                throw;
            }

            Cursor.Current = oldMPointer;
        }
        private void _uninitializeButon_Click(object sender, EventArgs e)
        {
            Cursor oldMPointer = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                CloseSpeechMagicSession();

                _initializeButton.Enabled = true;
                _uninitializeButon.Enabled = false;
                _recorder.Enabled = false;

                Cursor.Current = oldMPointer;
            }
            catch (Exception)
            {
                Cursor.Current = oldMPointer;
                throw;
            }

            Cursor.Current = oldMPointer;
        }

        #endregion Debugging Methods
    }
}
