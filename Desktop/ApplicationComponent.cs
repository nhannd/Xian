using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public abstract class ApplicationComponent : IApplicationComponent
    {
        private IApplicationComponentHost _host;

        public ApplicationComponent()
        {
        }

        protected IApplicationComponentHost Host
        {
            get { return _host; }
        }


        #region IApplicationComponent Members

        public void SetHost(IApplicationComponentHost host)
        {
            _host = host;
        }

        #endregion
    }
}
