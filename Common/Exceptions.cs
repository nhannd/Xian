using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginException.
	/// </summary>
    [SerializableAttribute]
	public class PluginException : ApplicationException
	{
		public PluginException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		public PluginException(string message) : base(message) {}
		public PluginException(string message, Exception inner) : base(message, inner) {}
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
	}

    public class ExtensionException : Exception
    {
		public ExtensionException(string message) : base(message) {}
        public ExtensionException(string message, Exception inner) : base(message, inner) { }
    }
	
/*
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
*/
}
