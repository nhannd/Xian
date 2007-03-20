using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public interface IWcfShred
    {
        int SharedHttpPort
        {
            get;
            set;
        }

		int SharedTcpPort
		{
			get;
			set;
		}
    }
}
