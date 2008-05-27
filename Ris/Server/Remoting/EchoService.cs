using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Remoting;

namespace ClearCanvas.Ris.Server.Remoting
{
	public class EchoService : MarshalByRefObject, IEchoService
	{
		#region IEchoService Members

		public string Echo(string text)
		{
			Console.WriteLine("Echo: " + text);
			return text;
		}

		#endregion
	}
}
