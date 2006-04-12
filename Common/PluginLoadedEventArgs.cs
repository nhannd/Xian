using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginProgressEventArgs.
	/// </summary>
	public class PluginLoadedEventArgs : EventArgs
	{
		string m_Message;

		public PluginLoadedEventArgs(string message)
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
