using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class RequestAttribute : ServerEntity
    {
        #region Constructors
        public RequestAttribute()
            : base("RequestAttribute")
        {
        }
        #endregion

    }
}
