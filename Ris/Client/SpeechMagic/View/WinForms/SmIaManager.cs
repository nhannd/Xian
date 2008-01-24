using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;

using SmIa = Philips.PSP.SmIa;
using SmIaCore = Philips.PSP.SmIaCore;
using SmIaEditor = Philips.PSP.SmIa.Editor;
using SmIaTextInterface = Philips.PSP.SmIa.Editor.TextInterface;
using SmXAudioLib = Philips.PSP.SpeechMagic.SmXAudioLib;
using SmXAudWizLib = Philips.PSP.SpeechMagic.SmXAudWizLib;
using SmIaDiskCacheManager = Philips.PSP.SmIaDiskCacheManager;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public class SmIaManager : IErrorConsole
    {
        public event LogUpdatedCallback LogUpdated;
        public event StateChangedCallback StateChanged;
        public event SessionWarningCallback SessionWarning;
        public event SessionErrorCallback SessionError;
        public event CommandRecognizedCallback CommandRecognized;
        public event RecognizerModeChangedCallback RecognizerModeChanged;
        public event AudioStateChangedCallback AudioStateChanged;
        public event SpeechMikeButtonPressedCallback SpeechMikeButtonPressed;
        public event ProtectDocumentRequestedCallback ProtectDocumentRequested;
        public event PreviewReceivedCallback PreviewReceived;

        //user settings
        private readonly SmIaProfile _profile;

        // session object
        private CoreStateHandler _coreStateHandler;
        private SmIaCore.SessionClass _coreSession;
        //private SmIaCore.InterActiveCoreClass _interactiveCore; //TODO: interactiveCore
        private SmIa.IAudio _coreSessionAudio;
        private SmXAudioLib.SmAudioClass _smXAudio;
        private SmXAudWizLib.SmAudioWizardClass _smXAudioWizard;

        // List of recognizers and alternate generators created
        private readonly ArrayList _recognizers;
        private readonly ArrayList _alternativeGenerators;
        private SmIa.IAlternativesGenerator _activeAlternativesGenerator;

        // Documents
        private readonly SmIaEditor.IEditor _editor;
        private object _documentHandle;
        private SmIaTextInterface.ITextDocument _activeTextDocument;

        // States
        private SmIa.AudioState _currentAudioState;
        private State _currentAppState;
        private RecognizerType _currentRecognizerMode;

        // Settings
        private bool _settingAlternativeGenerators;
        private bool _settingAcousticAdaptation;
        private bool _settingAutoPunctuation;
        private bool _settingAcousticFeedback;
        private bool _settingSynchronousPlayback;
        private int _settingVORLevel;
        private int _settingWindingSpeed;
        private int _settingAutoBackspaceInterval;

        private static SmIaManager _instance;

        public static SmIaManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SmIaManager();

                return _instance;
            }
        }

        private SmIaManager()
        {
            _profile = CreateTestProfile();

            _coreStateHandler = null;
            _coreSession = null;
            //_interactiveCore = null;
            _coreSessionAudio = null;
            _smXAudio = null;
            _smXAudioWizard = null;

            _activeAlternativesGenerator = null;
            _activeTextDocument = null;

            _recognizers = new ArrayList();
            _alternativeGenerators = new ArrayList();

            _editor = new SmIaEditor.Editor();

            _settingAlternativeGenerators = false;
            _settingAcousticAdaptation = false;
            _settingAutoPunctuation = false;
            _settingAcousticFeedback = false;
            _settingSynchronousPlayback = true;
        }

        public State SessionState
        {
            get { return _currentAppState; }
        }

        public RecognizerType RecognizerType
        {
            get { return _currentRecognizerMode; }
        }

        public int AudioSignalLevel
        {
            get { return _coreSessionAudio != null && _currentAppState != State.Uninitialized ? _coreSessionAudio.SignalLevel : 0; }
        }

        public int AudioFileLength
        {
            get { return _coreSession != null && _coreSession.ActiveDocument != null ? _coreSession.ActiveDocument.AudioFileLength : 0; }
        }

        public int AudioPosition
        {
            get { return _coreSession != null && _coreSession.ActiveDocument != null ? _coreSession.ActiveDocument.AudioPosition : 0; }
            set
            {
                if (_coreSession != null && _coreSession.ActiveDocument != null)
                {
                    _coreSession.ActiveDocument.MoveTextCursorTo(value);
                    _coreSession.ActiveDocument.AudioPosition = value;
                }
            }
        }

        #region Public Methods
        public void Open(DocumentEditMode mode)
        {
            StartSpeechMike();
            OpenSession(mode == DocumentEditMode.Author, true);
        }
        public void SetActiveDocument(object editorHandle)
        {
            LinkDocument(editorHandle);
        }
        public void Close()
        {
            if (_currentAppState == State.Uninitialized)
                return;

            CloseSession();
            StopSpeechMike();
        }

        public void Record()
        {
            if (_currentAppState == State.Recording)
                return;

            if (_currentRecognizerMode == RecognizerType.Dictation)
            {
                try
                {
                    ((SmIa.IDictationRecognizer)_coreSession.ActiveRecognizer).AutoPunctuation = _settingAutoPunctuation;
                }
                catch (Exception e)
                {
                    PrintSmIaError("Record: Set AutoPunctuation failed", e);
                    // go on ...
                }
            }

            try
            {
                _coreStateHandler.Record();
            }
            catch (Exception e)
            {
                PrintSmIaError("Record", e);
                return;
            }

            if (_currentRecognizerMode == RecognizerType.Dictation || _currentRecognizerMode == RecognizerType.Spelling)
            {
                ProtectDocument(true);
            }

            SetState(State.Recording);
            SetSpeechMikeLedState();
            PrintInfo("Recording");
        }
        public void Play()
        {
            if (_currentAppState == State.Stopping || _currentAppState == State.Winding ||
                _currentAppState == State.Playing || _currentAppState == State.Recording)
                return;

            try
            {
                _coreSessionAudio.SynchronousPlayback = _settingSynchronousPlayback;
                _coreSessionAudio.Play();
            }
            catch (Exception e)
            {
                PrintSmIaError("Play", e);
            }

            SetState(State.Playing);
            SetSpeechMikeLedState();
            PrintInfo("Playback started");
        }
        public void Forward()
        {
            if (_currentAppState == State.Stopping || _currentAppState == State.Winding ||
                _currentAppState == State.Playing || _currentAppState == State.Recording)
                return;

            try
            {
                _coreSessionAudio.SynchronousPlayback = _settingSynchronousPlayback;
                _coreSessionAudio.FastForward();
            }
            catch (Exception e)
            {
                PrintSmIaError("FastForward", e);
            }

            SetState(State.Winding);
            SetSpeechMikeLedState();
            PrintInfo("FastForward started");
        }
        public void Rewind()
        {
            if (_currentAppState == State.Stopping || _currentAppState == State.Winding ||
                _currentAppState == State.Playing || _currentAppState == State.Recording)
                return;

            try
            {
                _coreSessionAudio.SynchronousPlayback = _settingSynchronousPlayback;
                _coreSessionAudio.FastRewind();
            }
            catch (Exception e)
            {
                PrintSmIaError("FastRewind", e);
                return;
            }

            SetState(State.Winding);
            SetSpeechMikeLedState();
            PrintInfo("FastRewind started");
        }
        public void Stop()
        {
            Stop(false);
        }
        private void Stop(bool fromEvent)
        {
            if (!fromEvent && _currentAppState != State.Winding && _currentAppState != State.Playing && _currentAppState != State.Recording)
                return;

            if (!fromEvent && _coreSession.State == Philips.PSP.SmIaCore.SessionState.sstIdle)
                return;

            if (!fromEvent)
            {
                try
                {
                    _coreSessionAudio.Stop();
                }
                catch (Exception e)
                {
                    PrintSmIaError("Stop failed", e);
                    _currentAppState = State.Idle;
                }

                SetState(State.Stopping);
                SetSpeechMikeLedState();
                PrintInfo("Stopping");
            }
        }
        #endregion Public Methods

        #region Public Settings

        /// <summary>
        /// TODO: Comment
        /// </summary>
        public bool SettingAlternative
        {
            // TODO: alternativeGenerators are not implemented
            get { return _settingAlternativeGenerators; }
            set { EnableAlternativeGenerators(value); }
        }

        /// <summary>
        /// This SmIa.IAcousticAdaptation property enables or disables adaptation of the user's acoustic reference file (ARF).
        /// </summary>
        public bool SettingAcousticAdaptation
        {
            get { return _settingAcousticAdaptation; }
            set { EnableAcousticAdaptation(value); }
        }

        /// <summary>
        /// This SmIa.IDictationRecognizer property enables or disables the automatic punctuation generation of the recognizer. 
        /// This feature automatically inserts the missing punctuation characters comma (,) and period (.) into the document. 
        /// Automatic punctuation is disabled by default.
        /// </summary>
        public bool SettingAutoPunctuation
        {
            get { return _settingAutoPunctuation; }
            set { _settingAutoPunctuation = value; }
        }

        /// <summary>
        /// This SmIa.IAudio property enables or disables the playback of sound during winding operations 
        /// (see: SmIa.IAudio.FastForward and SmIa.IAudio.FastRewind).
        /// </summary>
        public bool SettingAcousticFeedback
        {
            get { return _settingAcousticFeedback; }
            set { EnableAcousticFeedback(value); }
        }

        /// <summary>
        /// This SmIa.IAudio property enables or disables synchronization of playback operations. 
        /// When set to TRUE, the text corresponding to the currently audible sound is highlighted in the text document; 
        /// if set to FALSE, no highlighting will be performed.
        /// </summary>
        public bool SettingSynchronousPlayback
        {
            // TODO: synchronize playback is on by default... we may remove this setting in the future
            get { return _settingSynchronousPlayback; }
            set { EnableSynchronousPlayback(value); }
        }

        /// <summary>
        /// This SmIa.IAudio property retrieves or sets the VOR (voice-operated recording) level of the audio source. 
        /// The VOR level indicates the level for silence detection during recording. Setting this property 
        /// will affect all open recognizers and documents.
        /// </summary>
        public int SettingVorLevel
        {
            get { return _settingVORLevel; }
            set { SetVorLevel(value); }
        }

        /// <summary>
        /// This SmIa.IAudio property controls the speed factor for winding operations (see: SmIa.IAudio.FastForward and SmIa.IAudio.FastRewind).
        /// </summary>
        public int SettingWindingSpeed
        {
            get { return _settingWindingSpeed; }
            set { SetWindingSpeed(value); }
        }

        /// <summary>
        /// This SmIa.IAudio property controls the maximum amount of audio data replayed after continuing playback.  Unit is in (ms)
        /// </summary>
        public int SettingAutoBackspaceInterval
        {
            get { return _settingAutoBackspaceInterval; }
            set { SetAutoBackspaceInterval(value); }
        }

        #endregion Public Settings

        #region Private Setting Helpers
        private void EnableAlternativeGenerators(bool enable)
        {
            _settingAlternativeGenerators = enable;
        }
        private void EnableAcousticAdaptation(bool enable)
        {
            _settingAcousticAdaptation = enable;

		    if (_coreSession!=null && _coreSession.ActiveRecognizer!=null && _coreSession.ActiveRecognizer is SmIa.IDictationRecognizer)
                ((SmIa.IAcousticAdaptation)_coreSession.ActiveRecognizer).AcousticAdaptation = _settingAcousticAdaptation;
        }
        private void EnableAcousticFeedback(bool enable)
        {
            try
            {
                _settingAcousticFeedback = enable;

                if (_coreSessionAudio != null)
                    _coreSessionAudio.AcousticFeedback = enable;

                PrintInfo(string.Format("AcousticFeedback set to {0}", enable));
            }
            catch (Exception e)
            {
                PrintSmIaError("EnableAcousticFeedback", e);
            }
        }
        private void EnableSynchronousPlayback(bool enable)
        {
            try
            {
                _settingSynchronousPlayback = enable;

                if (_currentAppState != State.Idle && _currentAppState != State.Initialized)
                    return;

                if (_coreSessionAudio != null)
                    _coreSessionAudio.SynchronousPlayback = enable;

                //if (_coreSessionAudio.SynchronousPlayback)
                //{
                //    if (_coreSession.ActiveDocument != null)
                //        _coreSession.ActiveDocument.MoveTextCursorTo(barSFPosition.Value);
                //}
                //else
                //{
                //    // update the slider 
                //    barSFPosition.Maximum = _coreSession.ActiveDocument.AudioFileLength;
                //    barSFPosition.Value = _coreSession.ActiveDocument.GetAudioPositionOfTextCursor();
                //}
            }
            catch (Exception e)
            {
                PrintSmIaError("EnableSyncPlayback", e);
            }
        }
        private void SetVorLevel(int level)
        {
            try
            {
                _settingVORLevel = level;
                _profile.VORLevel = level;

                if (_coreSessionAudio != null)
                    _coreSessionAudio.VORLevel = level;

                PrintInfo(string.Format("VorLevel set to {0}", level));
            }
            catch (Exception e)
            {
                PrintSmIaError("SetVorLevel", e);
            }            
        }
        private void SetWindingSpeed(int speed)
        {
            try
            {
                _settingWindingSpeed = speed;
                _profile.WindingSpeed = speed;

                if (_coreSessionAudio != null)
                    _coreSessionAudio.WindingSpeed = speed;

                PrintInfo(string.Format("WindingSpeed set to {0}", speed));
            }
            catch (Exception e)
            {
                PrintSmIaError("SetWindingSpeed", e);
            }
        }
        private void SetAutoBackspaceInterval(int interval)
        {
            try
            {
                _settingAutoBackspaceInterval = interval;

                if (_coreSessionAudio != null)
                    _coreSessionAudio.AutoBackspaceInterval = interval;

                PrintInfo(string.Format("AutoBackspaceInterval set to {0}", interval));
            }
            catch (Exception e)
            {
                PrintSmIaError("SetAutoBackspaceInterval", e);
            }
        }
        #endregion Private Setting Helpers

        #region Session Methods
        private void OpenSession(bool fullAccess, bool synch)
        {
            SetState(State.Uninitialized);
            SetRecognizerMode(RecognizerType.None);

            if (_coreSession != null)
            {
                // already initialized
                SetState(State.Initialized);
            }
            else
            {
                // create session object
                _coreSession = new SmIaCore.SessionClass();
                _coreStateHandler = new CoreStateHandler(_coreSession, this);
                _coreStateHandler.StateChanged += coreSession_OnStateChanged;
                _coreStateHandler.OnError += coreSession_OnError;

                _coreSession.OnNonVocalCommand += coreSession_OnNonVocalCommand;
                _coreSession.OnCommand += coreSession_OnCommand;
                _coreSession.OnCommandFragment += coreSession_OnCommandFiller;
                _coreSession.OnNonSpeech += coreSession_OnNonSpeech;
                _coreSession.OnPreview += coreSession_OnPreview;
                _coreSession.OnWarning += coreSession_OnWarning;
                _coreSession.OnAudioStateChanged += coreSession_OnAudioStateChanged;

                // fetch the audio interface to remember 
                _coreSessionAudio = _coreSession; // as SmIa.IAudio;

                if (synch)
                {
                    try
                    {
                        _coreSession.Open();
                        CoreSessionOpened(fullAccess);
                    }
                    catch (Exception ex)
                    {
                        PrintSmIaError("Failed to open a session", ex);
                        return;
                    }
                }
                else // try to start session asynchronously
                {
                    BackgroundTask startSessionTask = new BackgroundTask(
                        delegate(IBackgroundTaskContext taskContext)
                        {
                            try
                            {
                                _coreSession.Open();
                                taskContext.Complete(fullAccess);
                            }
                            catch (Exception e)
                            {
                                _coreSession = null;

                                COMException comEx = e as COMException;
                                if (comEx != null && comEx.ErrorCode == (int)SmIa.AudioError.eaudLoadingDriversFailed)
                                    taskContext.Error(new Exception("Failed to load audio drivers.\nInstall at least the SpeechMagic Recorder.", e));
                                else
                                    taskContext.Error(new Exception("Failed to open session", e));

                                taskContext.Error(e);
                            }
                        }, false);

                    startSessionTask.Terminated +=
                        delegate(object sender, BackgroundTaskTerminatedEventArgs args)
                            {
                                if (args.Reason == BackgroundTaskTerminatedReason.Completed)
                                {
                                    CoreSessionOpened(fullAccess);
                                }
                                else if (args.Reason == BackgroundTaskTerminatedReason.Exception)
                                {
                                    PrintSmIaError(args.Exception.Message, args.Exception);
                                }

                                // dispose of the task
                                startSessionTask.Dispose();
                                startSessionTask = null;
                            };

                    startSessionTask.Run();
                }
            }
        }
        private void CoreSessionOpened(bool fullAccess)
        {
            _coreSession.DocumentEditMode = fullAccess 
                ? SmIaCore.DocumentEditMode.emAuthorMode 
                : SmIaCore.DocumentEditMode.emCorrectionistMode;

            AutoCreateRecognizer();

            // synchronize settings 
            EnableAlternativeGenerators(_settingAlternativeGenerators);
            EnableAcousticAdaptation(_settingAcousticAdaptation);
            EnableAcousticFeedback(_settingAcousticFeedback);
            EnableSynchronousPlayback(_settingSynchronousPlayback);
            SetVorLevel(_settingVORLevel);
            SetWindingSpeed(_settingWindingSpeed);
            SetAutoBackspaceInterval(_settingAutoBackspaceInterval);
            SetState(State.Initialized);
            LinkDocument(_documentHandle);
        }
        private void CloseSession()
        {
            try
            {
                if (_coreSession == null)
                    return;

                DeactivateRecognizer();

                // deactivate all documents
                UnLinkDocument();

                // delete all the recognizers we had initialized. and yes, there is a reason we do not use an 
                // enumerator here: when using an enumerator, like foreach does, we will modify the list the 
                // enumerator is based on and since this is not allowed for .net enumerators, we will fail 
                // without deleting anything. 
                while (_recognizers.Count > 0)
                    DeleteRecognizer((Recognizer) _recognizers[0]);

                // delete all alternative generators we had initialized 
                while (_alternativeGenerators.Count > 0)
                {
                    AlternativesGenerator alternativesGenerator = (AlternativesGenerator) _alternativeGenerators[0];

                    alternativesGenerator.IAltGenerator.Close();

                    Marshal.ReleaseComObject(alternativesGenerator.IAltGenerator);
                    alternativesGenerator.IAltGenerator = null;

                    _alternativeGenerators.RemoveAt(0);
                }

                SynchAudioSettings(true);

                // close session
                _coreSession.Close();

                // delete the session
                Marshal.ReleaseComObject(_coreSession);
                _coreSession = null;

                SetState(State.Uninitialized);
                PrintInfo("Close Session");
            }
            catch (Exception ex)
            {
                PrintSmIaError("Failed to close a session", ex);
            }
        }
        private void LinkDocument(object documentHandle)
        {
            try
            {
                UnLinkDocument();

                _documentHandle = documentHandle;
                if (_coreSession != null && _currentAppState != State.Uninitialized)
                {
                    _activeTextDocument = _editor.NewDocument("doc", _documentHandle, "");

                    Guid guid = typeof(SmIa.IStandardDocument).GUID;
                    SmIa.IDocument tempDocument = (SmIa.IDocument)_coreSession.CreateComponent(ref guid);
                    tempDocument.Open(_activeTextDocument, Path.GetTempPath());
                    _coreSession.ActiveDocument = tempDocument;

                    PrintInfo("Linking document");
                }
            }
            catch (Exception ex)
            {
                PrintSmIaError("Failed to link document", ex);
            }
        }
        private void UnLinkDocument()
        {
            try
            {
                if (_coreSession != null && _currentAppState != State.Uninitialized)
                {
                    if (_coreSession.ActiveDocument != null)
                    {
                        SmIa.IDocument tempDoc = _coreSession.ActiveDocument;
                        _coreSession.ActiveDocument = null;
                        tempDoc.Close();
                        Marshal.ReleaseComObject(tempDoc);
                        tempDoc = null;
                        PrintInfo("Unlink Active document");
                    }

                    if (_activeTextDocument != null)
                    {
                        _editor.RemoveDocument(_activeTextDocument.Id);
                        Marshal.ReleaseComObject(_activeTextDocument);
                        _activeTextDocument = null;
                        PrintInfo("Unlinking active text document");
                    }

                    _documentHandle = null;
                }
                else
                {
                    PrintInfo(string.Format("Skipping unlinking: State={0}", _currentAppState));
                }
            }
            catch (Exception ex)
            {
                PrintSmIaError("Failed to close documents", ex);
            }
        }
        private void ProtectDocument(bool protect)
        {
            if (ProtectDocumentRequested != null)
            {
                PrintInfo(string.Format("ProtectDocument requested {0}", protect));
                ProtectDocumentRequested(protect);
            }
        }
        private void SetState(State newState)
        {
            if (_currentAppState != newState)
            {
                _currentAppState = newState;

                if (StateChanged != null)
                    StateChanged(newState);

                PrintInfo(string.Format("State change to {0}", newState));
            }
        }
        #endregion Session Methods

        #region SmIaCore events
        private void coreSession_OnStateChanged(object sender, SmIaCore.SessionState state)
        {
            PrintInfo(String.Format("coreSession_OnStateChanged - State:{0}", state.ToString("G")));

            if (state == SmIaCore.SessionState.sstIdle)
            {
                SetState(_coreSession.ActiveRecognizer != null ? State.Idle : State.Initialized);
                Stop(true);
                ProtectDocument(false);
            }
        }
        private void coreSession_OnError(object sender, int errorCode)
        {
            if (SessionError != null)
                SessionError(errorCode);

            PrintSmIaError(string.Format("SmIa Core sends error code: {0}", errorCode), null);
        }
        private void coreSession_OnWarning(int errorCode)
        {
            if (SessionWarning != null)
                SessionWarning(errorCode);

            PrintWarning(string.Format("Core sends warning: {0}", errorCode), null);
        }
        private void coreSession_OnPreview(string text)
        {
            if (PreviewReceived != null)
                PreviewReceived(text);
        }
        private void coreSession_OnAudioStateChanged(SmIa.AudioState state)
        {
            if (_currentAudioState != state)
            {
                _currentAudioState = state;

                SetSpeechMikeLedState();

                if (AudioStateChanged != null)
                    AudioStateChanged(state);

                PrintInfo(string.Format("Audio State change to {0}", state));
            }
        }
        private void coreSession_OnNonVocalCommand(int eventIdentifier)
        {
            PrintInfo(String.Format("NonVocalCommand {0}", eventIdentifier));

            //TODO: OnNonVocalComamnd
            //NonVocalCommandProcessed(eventIdentifier);

            //switch (eventIdentifier)
            //{
            //    case cmdId_NextField: NavigateToField(Direction.Forward); break;
            //    case cmdId_PreviousField: NavigateToField(Direction.Reverse); break;
            //    case cmdId_CmdButtonPressed:
            //        {
            //            switchRecognizerType = _currentRecognizerMode;
            //            switchRecordMode = btRecord.Pushed;

            //            SetActiveRecognizer((Recognizer)GetRecognizerByType(RecognizerType.Command)[0]);
            //            if (!switchRecordMode)
            //                Record();
            //            break;
            //        }

            //    case cmdId_CmdButtonReleased:
            //        {
            //            if (switchRecognizerType != RecognizerType.Command)
            //                SetActiveRecognizer((Recognizer)GetRecognizerByType(switchRecognizerType)[0]);

            //            if (!switchRecordMode)
            //                Stop(false);

            //            break;
            //        }

            //    default: PrintWarning(String.Format("UserEvent {0} not handled", eventIdentifier), null); break;
            //}
        }
        private void coreSession_OnCommand(string grammar, string symbol, string commandText, double confidence, ref SmIa.SSemanticAttribute[] semanticAttributes, ref string[] textNonterminalTexts, SmIa.ICommandManipulation commandManipulator)
        {
            if (CommandRecognized != null)
                CommandRecognized(grammar, symbol, commandText, confidence, ref semanticAttributes, ref textNonterminalTexts, commandManipulator);

            PrintInfo(String.Format("OnCommand - Grammar:{0}; Symbol:{1}; Command:{2}; Confidence:{3}", grammar, symbol, commandText, confidence));

            //TODO: OnCommand
            //ShowBubble(commandText);

            //DocumentForm form = GetActiveDocumentForm();

            //foreach (SSemanticAttribute iaAttribute in semanticAttributes)
            //{
            //    PrintInfo(String.Format("OnCommand - {0}={1}", iaAttribute.name, iaAttribute.value));
            //}

            //object audible = Helper.GetAttributeValue("audible", semanticAttributes);
            //if ((audible != null) && ((int)audible == 0))
            //    commandManipulator.DiscardAudioInformation();

            //int target = (int)Helper.GetAttributeValue("target", semanticAttributes);
            //if (target == 0)
            //    HandleCommand(semanticAttributes, textNonterminalTexts);
            //else if (target == 1)
            //    HandleWordCommand(semanticAttributes);
        }
        private void coreSession_OnCommandFiller(string text)
        {
            //Not implemented
            //PrintInfo(String.Format("coreSession_OnCommandFiller - Text:{0}", text));
        }
        private void coreSession_OnNonSpeech()
        {
            //Not implemented
            //PrintInfo("coreSession_OnNonSpeech");
        }
        #endregion SmIaCore events

        #region Recognizer Methods
        private void AutoCreateRecognizer()
        {
            RecognizerData data = new RecognizerData();
            data.user = _profile.User;
            data.language = _profile.Language;
            data.context = _profile.Context;
            data.inputChannel = _profile.InputChannel;

            Recognizer activeRcg = new Recognizer();
            activeRcg.IRecognizer = null;

            if (_profile.AlternativeGenerator)
            {
                PrintInfo("AutoCreate: Create and activate Alternatives Generator");
                data.name = "AltGen";
                _activeAlternativesGenerator = CreateAltGen(data, false);
            }

            if (_profile.CommandRecognizer)
            {
                // create command recognizer
                PrintInfo("AutoCreate: Create and activate CommandRecognizer");
                data.name = "Command";
                data.grammars = _profile.CommandGrammars;
                Recognizer rcg = CreateRecognizer(RecognizerType.Command, data);
                activeRcg = rcg;
            }
            if (_profile.DictationRecognizer)
            {
                // create dictation recognizer
                PrintInfo("AutoCreate: Create DictationRecognizer");
                data.name = "Dictation";
                data.grammars = _profile.DictationGrammars;
                Recognizer rcg = CreateRecognizer(RecognizerType.Dictation, data);
                if (!_profile.CommandRecognizer)
                    activeRcg = rcg;
            }
            if (_profile.SpellingRecognizer)
            {
                // create spelling recognizer
                PrintInfo("AutoCreate: Create SpellingRecognizer");
                data.name = "Spelling";
                data.grammars = _profile.SpellingGrammars;
                Recognizer rcg = CreateRecognizer(RecognizerType.Spelling, data);
                if (!_profile.CommandRecognizer && !_profile.DictationRecognizer)
                    activeRcg = rcg;
            }

            if (activeRcg.IRecognizer != null)
                SetActiveRecognizer(activeRcg);
        }
        private void SetRecognizerMode(RecognizerType type)
        {
            if (_currentRecognizerMode != type)
            {
                _currentRecognizerMode = type;

                if (RecognizerModeChanged != null)
                    RecognizerModeChanged(type);

                PrintInfo(string.Format("Recognizer Mode change to {0}", type));
            }
        }
        private void CreateRecognizer(RecognizerType type, RecognizerData data, bool activate, bool restricted)
        {
            BackgroundTask createRecognizerTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        Recognizer recognizer = new Recognizer();
                        if (type == RecognizerType.None)
                        {
                            SmIa.IAlternativesGenerator altGen = CreateAltGen(data, restricted);

                            if (activate)
                                _activeAlternativesGenerator = altGen;
                        }
                        else
                            recognizer = CreateRecognizer(type, data);

                        taskContext.Complete(recognizer);
                    }
                    catch (Exception e)
                    {
                        taskContext.Error(new Exception("Failed to create recognizer", e));
                    }
                }, false);

            createRecognizerTask.Terminated +=
                delegate(object sender, BackgroundTaskTerminatedEventArgs args)
                {
                    if (args.Reason == BackgroundTaskTerminatedReason.Completed)
                    {
                        if (activate)
                            SetActiveRecognizer((Recognizer)args.Result);
                    }
                    else if (args.Reason == BackgroundTaskTerminatedReason.Exception)
                    {
                        PrintSmIaError(args.Exception.Message, args.Exception);
                    }

                    // dispose of the task
                    createRecognizerTask.Dispose();
                    createRecognizerTask = null;
                };

            createRecognizerTask.Run();
        }
        private Recognizer CreateRecognizer(RecognizerType type, RecognizerData data)
        {
            SmIa.IRecognizer recognizer;
            Guid guid;

            if ((data.user != null && data.user.Length != 0))
                _profile.User = data.user;
            if (data.language != SmIa.LanguageId.lngNone
                && data.language != SmIa.LanguageId.lngAny)
                _profile.Language = data.language;
            if (data.inputChannel.type != SmIa.InputChannelId.icNone
                && data.inputChannel.type != SmIa.InputChannelId.icAny)
                _profile.InputChannel = data.inputChannel;
            if ((data.context != null && data.context.Length != 0))
                _profile.Context = data.context;

            switch (type)
            {
                case RecognizerType.Command:
                    {
                        guid = typeof(SmIa.ICommandRecognizer).GUID;

                        SmIa.ICommandRecognizer cmdRecognizer = (SmIa.ICommandRecognizer)CreateComponent(guid);
                        SmIa.SGrammarErrorInfo[] errors = cmdRecognizer.Open2(data.user, ref data.inputChannel, data.language, ref data.grammars);
                        _profile.CommandGrammars = data.grammars;
                        if (errors != null)
                        {
                            foreach (SmIa.SGrammarErrorInfo error in errors)
                                PrintSmIaError(String.Format("CommandRecognizer.Open error: [grm_name:{0} grm_file:{1} sym:{2} err:{3} msg:{4}]", error.grammarName, error.grammarFile, error.symbolName, error.errorCode, error.errorMessage), null);
                        }

                        if ((data.context != null && data.context.Length != 0))
                            cmdRecognizer.FormattingConText = data.context;

                        recognizer = (SmIa.IRecognizer)cmdRecognizer;
                        break;
                    }
                case RecognizerType.Dictation:
                    {
                        guid = typeof(SmIa.IDictationRecognizer).GUID;

                        SmIa.IDictationRecognizer dictRecognizer = (SmIa.IDictationRecognizer) CreateComponent(guid);
                        SmIa.SGrammarErrorInfo[] errors = dictRecognizer.Open2(data.user, ref data.inputChannel, data.language, data.context, ref data.grammars);
                        _profile.DictationGrammars = data.grammars;
                        if (errors != null)
                        {
                            foreach (SmIa.SGrammarErrorInfo error in errors)
                                PrintSmIaError(String.Format("DictationRecognizer.Open error: [grm_name:{0} grm_file:{1} sym:{2} err:{3} msg:{4}]", error.grammarName, error.grammarFile, error.symbolName, error.errorCode, error.errorMessage), null);
                        }
                        recognizer = (SmIa.IRecognizer)dictRecognizer;
                        break;
                    }
                case RecognizerType.Spelling:
                    {
                        guid = typeof(SmIa.ISpellingRecognizer).GUID;

                        SmIa.ISpellingRecognizer spellRecognizer = (SmIa.ISpellingRecognizer) CreateComponent(guid);
                        SmIa.SGrammarErrorInfo[] errors = spellRecognizer.Open2(data.user, ref data.inputChannel, data.language, ref data.grammars);
                        _profile.SpellingGrammars = data.grammars;
                        if (errors != null)
                        {
                            foreach (SmIa.SGrammarErrorInfo error in errors)
                                PrintSmIaError(String.Format("SpellingRecognizer.Open error: [grm_name:{0} grm_file:{1} sym:{2} err:{3} msg:{4}]", error.grammarName, error.grammarFile, error.symbolName, error.errorCode, error.errorMessage), null);
                        }

                        if ((data.context != null && data.context.Length != 0))
                            spellRecognizer.FormattingConText = data.context;

                        recognizer = (SmIa.IRecognizer)spellRecognizer;
                        break;
                    }
                default:
                    throw new ApplicationException("Unknown RecognizerType");
            }

            Recognizer rcg = new Recognizer(data.name, recognizer, type, data);

            _recognizers.Add(rcg);

            return rcg;
        }
        private void DeleteRecognizer(Recognizer recognizer)
        {
            recognizer.IRecognizer.Close();
            Marshal.ReleaseComObject(recognizer.IRecognizer);
            _recognizers.Remove(recognizer);
            recognizer.IRecognizer = null;
        }
        private void DeactivateRecognizer()
        {
            if (_coreSession.State != SmIaCore.SessionState.sstIdle)
            {
                PrintAppError(string.Format("Cannot DeactivateRecognizer when SessionState=={0}", _coreSession.State.ToString("G")), null);
                return;
            }

            _coreSession.ActiveRecognizer = null;
            SetState(State.Initialized);
            SetRecognizerMode(RecognizerType.None);
            // TODO: SetResourcePaths(defaultLanguageId);
        }
        private void SetActiveRecognizer(RecognizerType type)
        {
            foreach (Recognizer rcg in _recognizers)
            {
                if (rcg.Type == type)
                {
                    try
                    {
                        SetActiveRecognizer(rcg);
                    }
                    catch (Exception ex)
                    {
                        PrintSmIaError(ex.Message, ex);
                    }
                    break;
                }
            }

            SetSpeechMikeLedState();
        }
        private void SetActiveRecognizer(Recognizer recognizer)
        {
            PrintInfo(string.Format("Set Active Recognizer: id={0}", recognizer.Id));
            try
            {
                _coreSession.ActiveRecognizer = recognizer.IRecognizer;
            }
            catch (Exception ex)
            {
                PrintSmIaError("Switching recognizer FAILED", ex);
                return;
            }

            bool recording = _currentAppState == State.Recording;

            if (_currentAppState == State.Initialized)
                SetState(State.Idle);

            SetRecognizerMode(recognizer.Type);
            if (recognizer.Type == RecognizerType.Dictation)
            {
                ((SmIa.IAcousticAdaptation)recognizer.IRecognizer).AcousticAdaptation = _settingAcousticAdaptation;
                PrintInfo(String.Format("Adaptation time:\noverall:{0}ms\nsession:{1}ms",
                                  recognizer.overallAdaptationTime + recognizer.sessionAdaptationTime,
                                  recognizer.sessionAdaptationTime));
                
                // TODO: update adaptation time
                // toolTip1.SetToolTip(picAcadState, String.Format("Adaptation time:\noverall:{0}ms\nsession:{1}ms", recognizer.overallAdaptationTime + recognizer.sessionAdaptationTime, recognizer.sessionAdaptationTime));
                if (recording)
                    ProtectDocument(true);

            }
            else if (recognizer.Type == RecognizerType.Command)
            {
                ProtectDocument(false);
            }
            else if (recognizer.Type == RecognizerType.Spelling)
            {
                if (recording)
                    ProtectDocument(true);
            }
            else if (recognizer.Type == RecognizerType.None)
            {
                ProtectDocument(false);
            }

            InitCurrentProfile(recognizer.Data);
        }
        private void InitCurrentProfile(RecognizerData data)
        {
            _profile.User = data.user;
            _profile.Language = data.language;
            _profile.InputChannel = data.inputChannel;
            _profile.Context = data.context;
            // TODO: SetResourcePaths((int)data.language);
        }
        #endregion Recognizer Methods

        #region Audio Methods

        private void StartSpeechMike()
        {
            InitializeAudio();

            SynchAudioSettings(false);

            if (_settingVORLevel < 0)
            {
                // TODO: show AudioWizard when necessary
                //if (Platform.ShowMessageBox("Would you like to run the AudioWizard to calibrate the speech mike?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                //    StartAudioWizard();
                //else
                    _settingVORLevel = 0;
            }
        }
        private void StopSpeechMike()
        {
            DeInitializeAudio();
        }
        private void InitializeAudio()
        {
            try
            {
                _smXAudio = new SmXAudioLib.SmAudioClass();
                _smXAudio.Initialize();
                _smXAudio.ControlDevice += smAudio_ControlDevice;
                _smXAudio.ActivateControlDevice();
                PrintInfo("SmAudio initialized");
            }
            catch (Exception ex)
            {
                PrintSmIaError("SmAudio initialization error", ex);
            }

            try
            {
                _smXAudioWizard = new SmXAudWizLib.SmAudioWizardClass();
                _smXAudioWizard.AudioWizardComplete += OnAudioWizardComplete;
            }
            catch (Exception ex)
            {
                PrintWarning("Failed to create AudioWizard", ex);
            }
        }

        private void DeInitializeAudio()
        {
            try
            {
                if (_smXAudio != null)
                {
                    _smXAudio.Deinitialize();
                    Marshal.ReleaseComObject(_smXAudio);
                    _smXAudio = null;

                    PrintInfo("SmAudio deinitialized");
                }

                if (_smXAudioWizard != null)
                {
                    Marshal.ReleaseComObject(_smXAudioWizard);
                    _smXAudioWizard = null;

                    PrintInfo("SmAudioWizard deinitialized");
                }

            }
            catch (Exception ex)
            {
                PrintSmIaError("SmAudio deinitialization error", ex);
            }
        }

        private void SetSpeechMikeLedState()
        {
            try
            {
                if (_smXAudio == null)
                    return;

                if (_currentAppState == State.Recording)
                {
                    if (_currentRecognizerMode == RecognizerType.Dictation)
                        _smXAudio.LEDState = SmXAudioLib.SmXAudioLEDState.smxaudRecordOverwrite;
                    else if (_currentRecognizerMode == RecognizerType.Command)
                        _smXAudio.LEDState = SmXAudioLib.SmXAudioLEDState.smxaudCmd;
                    else if (_currentRecognizerMode == RecognizerType.Spelling)
                        _smXAudio.LEDState = SmXAudioLib.SmXAudioLEDState.smxaudRecordOverwrite;
                }
                //else if (sessionState == State.Playing)
                //else if (sessionState == State.Winding)
                else
                    _smXAudio.LEDState = SmXAudioLib.SmXAudioLEDState.smxaudStop;
            }
            catch (Exception ex)
            {
                PrintWarning("Failed to set LED state", ex);
            }
        }

        private void smAudio_ControlDevice(SmXAudioLib.SmXAudioControlDeviceEvent controlDeviceEvent, SmXAudioLib.SmXAudioControlDevice controlSource)
        {
            switch (controlDeviceEvent)
            {
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudCommandPressed:
                    PrintInfo("AudioControlEvent smxaudCommandPressed");

                    //if (!btRecord.Pushed)
                    //{
                    //    SetActiveRecognizer((Recognizer)GetRecognizerByType(RecognizerType.Command)[0]);
                    //    switchRecordMode = false;
                    //    switchRecognizerType = RecognizerType.Command;
                    //    Record();
                    //    break;
                    //}
                    //PrintInfo("Queuing cmdId_CmdButtonPressed");
                    //QueueNonVocalCommand(cmdId_CmdButtonPressed, "Queuing cmdId_CmdButtonPressed");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudCommandReleased:
                    PrintInfo("AudioControlEvent smxaudCommandReleased");
                    //PrintInfo("Queuing cmdId_CmdButtonReleased");
                    //QueueNonVocalCommand(cmdId_CmdButtonReleased, "Queuing cmdId_CmdButtonReleased");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudInsertReleased:
                    {
                        PrintInfo("AudioControlEvent smxaudInsertReleased");

                        //if (menuSwitchCommand.Checked)
                        //{
                        //    if (GetRecognizerByType(RecognizerType.Dictation).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Dictation);
                        //    else if (GetRecognizerByType(RecognizerType.Spelling).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Spelling);
                        //}
                        //else if (menuSwitchDictation.Checked)
                        //{
                        //    if (GetRecognizerByType(RecognizerType.Spelling).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Spelling);
                        //    else if (GetRecognizerByType(RecognizerType.Command).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Command);

                        //}
                        //else if (menuSwitchSpelling.Checked)
                        //{
                        //    if (GetRecognizerByType(RecognizerType.Command).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Command);
                        //    else if (GetRecognizerByType(RecognizerType.Dictation).Count > 0)
                        //        SetActiveRecognizer(RecognizerType.Dictation);
                        //}
                        break;
                    }
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastForwardPressed:
                    {
                        PrintInfo("AudioControlEvent smxaudFastForwardPressed");
                        //if (_currentAppState == State.Recording && menuSwitchDictation.Checked) // dictating
                        //{
                        //    QueueNonVocalCommand(cmdId_NextField, "Queuing cmdId_NextField");
                        //}
                        //else
                        //    Fwd();

                        break;
                    }
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastForwardReleased:
                    PrintInfo("AudioControlEvent smxaudFastForwardReleased");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastRewindPressed:
                    {
                        PrintInfo("AudioControlEvent smxaudFastRewindPressed");
                        //if (_currentAppState == State.Recording && menuSwitchDictation.Checked) // dictating
                        //{
                        //    QueueNonVocalCommand(cmdId_PreviousField, "Queuing cmdId_PreviousField");
                        //}
                        //else
                        //    Rwd();

                        break;
                    }
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudFastRewindReleased:
                    PrintInfo("AudioControlEvent smxaudFastRewindReleased");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayPressed:
                    PrintInfo("AudioControlEvent smxaudPlayPressed");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudStopPressed:
                    PrintInfo("AudioControlEvent smxaudPlayPressed");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayStopTogglePressed:
                    PrintInfo("AudioControlEvent smxaudPlayStopTogglePressed");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudPlayStopToggleReleased:
                    PrintInfo("AudioControlEvent smxaudPlayStopToggleReleased");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudRecordPressed:
                    PrintInfo("AudioControlEvent smxaudRecordPressed");
                    break;
                case SmXAudioLib.SmXAudioControlDeviceEvent.smxaudEOLReleased:
                    PrintInfo("AudioControlEvent smxaudEOLReleased");
                    break;
            }

            if (SpeechMikeButtonPressed != null)
                SpeechMikeButtonPressed(controlDeviceEvent);
        }

        private void OnAudioWizardComplete(SmXAudWizLib.SmXAudioWizardError error)
        {
            if (error == SmXAudWizLib.SmXAudioWizardError.smxaudwdOK || 
                error == SmXAudWizLib.SmXAudioWizardError.smxaudwdAudioBadUsable)
            {
                try
                {
                    SetVorLevel(_smXAudioWizard.SilenceDetectionLevel);
                    _profile.ChannelVolume = _smXAudioWizard.InVolumeChannel;
                    _profile.MasterVolume = _smXAudioWizard.InVolumeMaster;
                }
                catch (Exception e)
                {
                    PrintAppError("Failed to get SilenceDetectionLevel:", e);
                }
            }
        }

        private void StartAudioWizard()
        {
            try
            {
                _smXAudioWizard.InputChannelDisplayName = String.Format("{0} ({1}, {2}) ", _profile.InputChannel.type, _profile.InputChannel.bw, _profile.InputChannel.res);
                _smXAudioWizard.StartAudioWizard(Philips.PSP.SpeechMagic.SmXAudWizLib.SmXAudioWizardPages.smxaudwdAll);
            }
            catch (Exception ex)
            {
                PrintAppError("Failed to create/start audioWizard:", ex);
            }
        }

        #endregion Audio Methods

        #region Alternative Generator Method
        private SmIa.IAlternativesGenerator CreateAltGen(RecognizerData data, bool restricted)
        {
            Guid guid = typeof(SmIa.IAlternativesGenerator).GUID;
            SmIa.IAlternativesGenerator altGen = (SmIa.IAlternativesGenerator)_coreSession.CreateComponent(ref guid);
            try
            {
                if (restricted)
                    altGen.OpenBasic(data.language, data.context);
                else
                    altGen.Open2(data.user, ref data.inputChannel, data.language, data.context);
            }
            catch (Exception e)
            {
                PrintSmIaError("CreateAltGenerator", e);
                return null;
            }

            _alternativeGenerators.Add(new AlternativesGenerator(data.name, altGen));

            return altGen;
        }
        private void DeleteAltGen(string name)
        {
            foreach (AlternativesGenerator altGen in _alternativeGenerators)
            {
                if (altGen.Name == name)
                {
                    altGen.IAltGenerator.Close();
                    _alternativeGenerators.Remove(altGen);
                    return;
                }
            }
        }
        private void SetActiveAltGen(string name)
        {
            if (_alternativeGenerators == null || _alternativeGenerators.Count == 0)
                return;

            foreach (AlternativesGenerator altGen in _alternativeGenerators)
            {
                if (altGen.Name == name)
                {
                    _activeAlternativesGenerator = altGen.IAltGenerator;
                    return;
                }
            }
        }
        private void SelectAlternative(int id)
        {
            try
            {
                SmIa.IAlternatives alts = _activeAlternativesGenerator.GenerateAlternatives();
                if (alts == null)
                    return;

                string[] list = alts.GetAlternatives();
                if (list != null && list.Length > id)
                    alts.SelectAndClose(id);
                else
                {
                    PrintInfo("No alternatives anymore :(");
                    alts.Close();
                }
            }
            catch (Exception ex)
            {
                PrintSmIaError("altFormAlternativeSelected", ex);
            }
        }
        private void SelectAlternative(string text)
        {
            try
            {
                SmIa.IAlternatives alts = _activeAlternativesGenerator.GenerateAlternatives();
                if (alts == null)
                    return;

                string[] list = alts.GetAlternatives();
                if (list != null)
                {
                    int index = Array.IndexOf(list, text);
                    if (index >= 0)
                    {
                        PrintInfo("replacing text with alternative");
                        alts.SelectAndClose(index);
                    }
                    else
                    {
                        PrintInfo("Alternative not in list anymore :(");
                        alts.Close();
                    }
                }
                else
                {
                    PrintInfo("No alternatives anymore :(");
                    alts.Close();
                }
            }
            catch (Exception ex)
            {
                PrintSmIaError("altFormAlternativeSelected", ex);
            }
        }
        private SmIa.IRecognizer CreateComponent(Guid guid)
        {
            Guid rc = guid;
            return (SmIa.IRecognizer)_coreSession.CreateComponent(ref rc);
        }
        #endregion Alternative Generator Method

        #region IErrorConsole Members
        public void PrintInfo(string text)
        {
            UpdateLog(text, SmIaErrorLevel.Information, null);
        }
        public void PrintWarning(string text, Exception ex)
        {
            UpdateLog(text, SmIaErrorLevel.Warning, ex);
        }
        public void PrintSmIaError(string text, Exception ex)
        {
            string sError = "Unknown";

            if (ex is COMException)
                sError = ((COMException)ex).ErrorCode.ToString("x");

            text = String.Format("{0}: {1}", text, sError);

            UpdateLog(text, SmIaErrorLevel.Fatal, ex);
        }
        public void PrintAppError(string text, Exception ex)
        {
            UpdateLog(text, SmIaErrorLevel.Error, ex);
        }
        private void UpdateLog(string text, SmIaErrorLevel severity, Exception ex)
        {
            string message = ex == null ? text : string.Format("{0}\r\n{1}", text, ex);
            if (LogUpdated != null)
                LogUpdated(message, severity);
        }
        #endregion IErrorConsole Members

        #region Helper Methods

        public void SynchAudioSettings(bool write)
        {
            if (write)
            {
                _profile.VORLevel = _settingVORLevel;
                _profile.WindingSpeed = _settingWindingSpeed;
            }
            else
            {
                _settingVORLevel = _profile.VORLevel;
                _settingWindingSpeed = _profile.WindingSpeed;
            }
        }

        #endregion


        // TODO: for debug only.  DiskCacheManager should be removed for production
        // Still have to figure out where to get the profile from, as well as the grammar files
        // the user, language and input channel should be pre-loaded somewhere else and pass in
        private SmIaProfile CreateTestProfile()
        {
            SmIaDiskCacheManager.DiskCacheManager dataStore = new SmIaDiskCacheManager.DiskCacheManagerClass();

            SmIaProfile profile = new SmIaProfile();

            profile.User = dataStore.QueryAllUsers()[0];
            profile.Language = dataStore.QueryAllLanguages()[0];
            profile.InputChannel = ((SmIa.IDataQuery2)dataStore).QueryInputChannels(profile.User, profile.Language)[0];
            profile.Context = dataStore.QueryAllConTexts()[0];

            List<string> grammarFiles = new List<string>();
            foreach (string file in Directory.GetFiles(".", "*.sgf", SearchOption.TopDirectoryOnly))
            {
                grammarFiles.Add(Path.GetFullPath(file));
            }

            profile.DictationGrammars = grammarFiles.ToArray();

            profile.AlternativeGenerator = false;
            profile.DictationRecognizer = true;
            profile.CommandRecognizer = false;
            profile.SpellingRecognizer = false;

            return profile;
        }
    }
}
