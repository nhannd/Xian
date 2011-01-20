#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Time;

namespace ClearCanvas.Enterprise.Core
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITimeService))]
    public class TimeService : CoreServiceLayer, ITimeService
    {
        #region ITimeService Members

		public GetTimeResponse GetTime(GetTimeRequest request)
        {
            return new GetTimeResponse(DateTime.Now);
        }

        #endregion
    }

}
