using System;

namespace ClearCanvas.Common.Platform
{
	/// <summary>
	/// Summary description for PluginProgressEventArgs.
	/// </summary>
	public class PluginProgressEventArgs : EventArgs
	{
		string m_Message;
		bool m_Complete;

		public PluginProgressEventArgs(string message, bool complete)
		{
			m_Message = message;
			m_Complete = complete;
		}

		public string Message
		{
			get
			{
				return m_Message;
			}
			set
			{
				m_Message = value;
			}
		}

		public bool Complete
		{
			get
			{
				return m_Complete;
			}
			set
			{
				m_Complete = value;
			}
		}
	}
}
