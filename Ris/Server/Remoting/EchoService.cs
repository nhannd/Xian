#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
