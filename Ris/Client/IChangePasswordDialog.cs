using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public interface IChangePasswordDialog : IDisposable
    {
        bool Show();

        string UserName { get; set; }
        string Password { get; set; }
        string NewPassword { get; }
    }
}
