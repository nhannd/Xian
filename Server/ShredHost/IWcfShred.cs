using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public interface IWcfShred
    {
        int HttpPort
        {
            get;
            set;
        }

		int TcpPort
		{
			get;
			set;
		}
    }
}
