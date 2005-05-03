using System;


namespace Xian.Common.Platform
{
	/// <summary>
	/// Summary description for PluginException.
	/// </summary>
	public class PluginException : ApplicationException
	{
		public PluginException() {}
		public PluginException(string message) : base(message) {}
		public PluginException(string message, Exception inner) : base(message, inner) {}
	}

	public class PluginErrorException : PluginException
	{
		public PluginErrorException() {}
		public PluginErrorException(string message) : base(message) {}
		public PluginErrorException(string message, Exception inner) : base(message, inner) {}
	}

	public class PluginWarningException : PluginException
	{
		public PluginWarningException() {}
		public PluginWarningException(string message) : base(message) {}
		public PluginWarningException(string message, Exception inner) : base(message, inner) {}
	}

	public class DuplicateObjectFoundException : ApplicationException
	{
		public DuplicateObjectFoundException() {}
		public DuplicateObjectFoundException(string message) : base(message) {}
		public DuplicateObjectFoundException(string message, Exception inner) : base(message, inner) {}
	}
}
