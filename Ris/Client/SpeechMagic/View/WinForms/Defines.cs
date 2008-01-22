using System;
using Philips.PSP.SmIa;
using Philips.PSP.SpeechMagic.SmXAudioLib;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    public interface IErrorConsole
    {
        void PrintInfo(string text);
        void PrintWarning(string text, Exception ex);
        void PrintSmIaError(string text, Exception ex);
        void PrintAppError(string text, Exception ex);
    }

    struct SmIaGUIDs
    {
        public const string IID_IPhoneticTranscriber = "{1E0AE0B2-8DE6-11D8-A869-0002A58642EC}";
        public const string IID_IAlternativesGenerator = "{1E0AE0B4-8DE6-11D8-A869-0002A58642EC}";
        public const string IID_IDocument = "{1E0AE0B1-8DE6-11D8-A869-0002A58642EC}";
        public const string IID_ICommmandRecognizer = "{1E0AE091-8DE6-11D8-A869-0002A58642EC}";
        public const string IID_IDictationRecognizer = "{1E0AE092-8DE6-11D8-A869-0002A58642EC}";
        public const string IID_ISpellingRecognizer = "{1E0AE093-8DE6-11D8-A869-0002A58642EC}";
    }

    public enum SmIaErrorLevel { None = -1, Information = 0, Warning = 1, Error = 2, Fatal = 3, Lethal = 4 };
    public enum State { Uninitialized, Initialized, Idle, Stopping, Recording, Playing, Winding };
    
    public enum DocumentType { None, Word, Rtf, Pseudo, TOC, TX, Simple, TX12, TOC13, TX13 };
	public enum RecognizerType { None, Command, Dictation, Spelling };

	#region Delegates
    public delegate void LogUpdatedCallback(string message, SmIaErrorLevel severity);
    public delegate void RecognizerModeChangedCallback(RecognizerType type);
    public delegate void StateChangedCallback(State state);
    public delegate void CommandRecognizedCallback(string grammar, string symbol, string commandText, double confidence, ref SSemanticAttribute[] semanticAttributes, ref string[] textNonterminalTexts, ICommandManipulation commandManipulator);
    public delegate void SessionWarningCallback(int errorCode);
    public delegate void SessionErrorCallback(int errorCode);
    public delegate void ProtectDocumentRequestedCallback(bool protect);
    public delegate void AudioStateChangedCallback(AudioState state);
    public delegate void SpeechMikeButtonPressedCallback(SmXAudioControlDeviceEvent controlDeviceEvent);
    public delegate void PreviewReceivedCallback(string preview);
    #endregion

	public class NonVocalCommandData
	{
		public static string GetTimeString(DateTime time)
		{
			return time.ToString("HH:mm:ss.fff");
		}
		public NonVocalCommandData(int commandId, string commandDescription, DateTime timeStampQueued, DateTime timeStampProcessed) : this (commandId,commandDescription, timeStampQueued)
		{
			timeProcessed = timeStampProcessed.ToString("HH:mm:ss.fff");
		}
		public NonVocalCommandData(int commandId, string commandDescription, DateTime timeStampQueued)
		{
			id = commandId;
			description = commandDescription;
			timeQueued = GetTimeString(timeStampQueued);
			timeProcessed = "";
		}
		public override string ToString()
		{
			return String.Format("NVC: id={0}, descr={1}, queued={2}, processed={3}", id, description,timeQueued,timeProcessed);
		}
		public int Id
		{
			get { return id; }
		}
		public string Description
		{
			get { return description; }
		}
		public string TimeQueued
		{
			get { return timeQueued; }
		}
		public string TimeProcessed
		{
			set { timeProcessed = value; }
			get { return timeProcessed; }
		}

		private readonly int id;
        private readonly string description;
        private readonly string timeQueued;
        private string timeProcessed;
	}

    public class SmIaProfile
    {
		public LanguageId	Language;
        public SInputChannel InputChannel;
		public string Context;
		public string User;
        public string[] CommandGrammars;
        public string[] DictationGrammars;
        public string[] SpellingGrammars;
        public bool AlternativeGenerator;
        public bool CommandRecognizer;
        public bool DictationRecognizer;
        public bool SpellingRecognizer;
        public int ChannelVolume;
        public int MasterVolume;
        public int VORLevel = 0;
        public int WindingSpeed = 5;

        public SmIaProfile()
        {
            Language = LanguageId.lngNone;
            InputChannel.type = InputChannelId.icNone;
            InputChannel.bw = Bandwidth.bwNone;
            InputChannel.res = Resolution.reNone;
        }
    }
    
    public struct RecognizerData
	{
		public LanguageId	language;
        public SInputChannel inputChannel;
		public string context;
		public string user; 
		public string[] grammars;
		public string name;
	}
	public struct AlternativesGenerator
	{
		public AlternativesGenerator(string name, IAlternativesGenerator iAltGen)
		{
			Name = name;
			IAltGenerator = iAltGen;
		}
		public override string ToString() { return Name; }
		public string Name;
		public IAlternativesGenerator IAltGenerator;
	}
	public struct Recognizer
	{
		public Recognizer(string id, IRecognizer iRcg, RecognizerType type,RecognizerData data)
		{
			Id = id;
			IRecognizer  = iRcg;
			Type = type;
			Data = data;
				
			if (type==RecognizerType.Dictation)
				overallAdaptationTime = ((IAcousticAdaptation)iRcg).AcousticAdaptationTime;
			else
				overallAdaptationTime = 0;

			sessionAdaptationTime = 0;
		}
		public override string ToString() { return Id; }
		public string Id;
		public RecognizerType Type;
		public RecognizerData Data;
		public IRecognizer IRecognizer;
		public int overallAdaptationTime;
		public int sessionAdaptationTime;
	}
}