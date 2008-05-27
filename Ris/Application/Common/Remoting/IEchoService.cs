using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Remoting
{
	[ServiceContract]
	public interface IEchoService
	{
		[OperationContract]
		string Echo(string text);
	}
}
