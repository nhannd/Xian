using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginProgressEventArgs.
	/// </summary>
	public class PluginLoadedEventArgs : EventArgs
	{
		string _Message;

		public PluginLoadedEventArgs(string message)
		{
			_Message = message;
		}

		public string Message
		{
			get
			{
				return _Message;
			}
			set
			{
				_Message = value;
			}
		}
	}
}
