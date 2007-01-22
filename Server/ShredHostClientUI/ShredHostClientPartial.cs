using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    public partial class ShredHostClient : System.ServiceModel.ClientBase<IShredHost>, IDisposable
    {

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
