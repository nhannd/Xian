using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginProgressEventArgs.
	/// </summary>
	public class PluginProgressEventArgs : EventArgs
	{
		string m_Message;

		public PluginProgressEventArgs(string message)
		{
			m_Message = message;
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
	}
}
