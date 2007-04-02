using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public interface ILoginDialog : IDisposable
    {
        bool Show(out string userName, out string password);
    }
}
