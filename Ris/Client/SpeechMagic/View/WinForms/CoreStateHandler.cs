using System;
using SmIa = Philips.PSP.SmIa;
using SmIaCore = Philips.PSP.SmIaCore;

namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    /// <summary>
	/// Summary description for CoreEventHandler.
	/// </summary>
	public class CoreStateHandler
	{
		public delegate void StateChangedDelegate(object sender, SmIaCore.SessionState state);
		public delegate void OnErrorDelegate(object sender, int errorCode);

        private readonly SmIaCore.SessionClass _coreSession = null;
		private readonly IErrorConsole _errorConsole;

		public event StateChangedDelegate StateChanged;
		public event OnErrorDelegate OnError;

		private int idleCounter = 0;
		private int stoppingCounter = 0;

        private SmIaCore.SessionState lastState = SmIaCore.SessionState.sstInvalid;

        public CoreStateHandler(SmIaCore.SessionClass session, IErrorConsole console)
		{
			_errorConsole = console;
			_coreSession = session;
			_coreSession.OnStateChanged += coreSession_OnStateChanged;
			_coreSession.OnError += coreSession_OnError;
		}

		public void Record()
		{
			try
			{
				// the state change could be received almost immediately before
				// there is a chance to increase the counter -> so raise it before
				++stoppingCounter;
				((SmIa.IAudio)_coreSession).Record();
			}
			catch
			{
				--stoppingCounter;
				throw;
			}
		}
		public int RecordFromFile(string filename)
		{
			int ret;
			try
			{
				// the state change could be received almost immediately before
				// there is a chance to increase the counter -> so raise it before
				++idleCounter;
				ret = ((SmIa.IAudio)_coreSession).RecordFromFile(filename);
			}
			catch
			{
				--idleCounter;
				throw;
			}
			return ret;
		}
		public void Reset()
		{
			idleCounter = 0;
			stoppingCounter = 0;
		}

        private void coreSession_OnStateChanged(SmIaCore.SessionState state)
		{
			_errorConsole.PrintInfo(String.Format("CoreStateHandler - OnStateChanged({0})", state.ToString("G")));

			bool fire = true;

			switch (state)
			{
                case SmIaCore.SessionState.sstRecording:
				{
                    if (lastState == SmIaCore.SessionState.sstStopping) // there won't be an Idle state
					{
						--idleCounter;
					}
					break;
				}
                case SmIaCore.SessionState.sstPlaying: ++idleCounter; break;
                case SmIaCore.SessionState.sstStopping: 
				{
					++idleCounter;
					--stoppingCounter;
					if (stoppingCounter>0)
						fire = false;
					break;
				}
                case SmIaCore.SessionState.sstIdle:
				{
					--idleCounter;
					if (idleCounter>0 || stoppingCounter>0)
						fire = false;
					break;
				}
			}

			lastState = state;

			string text = String.Format("CoreStateHandler - new counter values: idleCounter={0}, stoppingCounter={1}", idleCounter,stoppingCounter);
			if (stoppingCounter<0 || idleCounter<0)
				_errorConsole.PrintWarning(text, null);
			else
				_errorConsole.PrintInfo(text);
			
			if (fire)
				StateChanged(this, state);
		}
		private void coreSession_OnError(int errorCode)
		{
            if (stoppingCounter > 0)
				--stoppingCounter;

			++idleCounter;

			OnError(this, errorCode);
		}
	}
}
