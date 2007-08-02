using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class Series: ServerEntity
    {
        #region Constructors
        public Series()
            : base("Series")
        {
        }
        #endregion

    }
}
