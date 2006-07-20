using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public interface IServiceLayer
    {
        void SetSession(ISession session);
    }
}
