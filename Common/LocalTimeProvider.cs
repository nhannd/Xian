using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    internal class LocalTimeProvider : ITimeProvider
    {
        #region ITimeProvider Members

        public DateTime CurrentTime
        {
            get { return DateTime.Now; }
        }

        #endregion
    }
}
